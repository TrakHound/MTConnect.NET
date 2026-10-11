#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers in `shell.ts` — the shared shell-out
 * layer every `tools/release/*` script imports. Run with:
 *
 *   tsx tools/release/shell.test.ts
 *
 * Exits 0 when every assertion passes, non-zero otherwise. Uses an
 * in-house `test(name, fn)` collect-then-await shim so both sync and
 * async cases can share the same runner without pulling in Jest/Vitest.
 *
 * The `run()` export is exercised via its `dryRun` mode — that path
 * returns without spawning a subprocess, and its stdout side-effect is
 * asserted by patching `process.stdout.write`. The live-spawn path is
 * out of scope for a pure-unit suite (belongs to the integration
 * matrix that already runs `--dry-run` end-to-end).
 */

import { strict as assert } from 'node:assert';
import {
  SECRET_ARG_NAMES,
  optionalEnv,
  parseDryRun,
  renderCmd,
  requireEnv,
  run,
} from './shell.ts';

const cases: Array<{ name: string; fn: () => void | Promise<void> }> = [];
const test = (name: string, fn: () => void | Promise<void>): void => {
  cases.push({ name, fn });
};

// stdout-capture helper — patches `process.stdout.write` for the
// duration of `fn`, returns everything written. Used to assert the
// dry-run log format without inventing a mock framework.
const captureStdout = async (fn: () => void | Promise<void>): Promise<string> => {
  const original = process.stdout.write.bind(process.stdout);
  let buf = '';
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (process.stdout as any).write = (chunk: string | Uint8Array): boolean => {
    buf += typeof chunk === 'string' ? chunk : Buffer.from(chunk).toString('utf8');
    return true;
  };
  try {
    await fn();
  } finally {
    (process.stdout as unknown as { write: typeof original }).write = original;
  }
  return buf;
};

// ─── renderCmd ──────────────────────────────────────────────────
test('renderCmd: simple args stay unquoted', () => {
  assert.equal(renderCmd('git', ['log', '--oneline']), 'git log --oneline');
  assert.equal(renderCmd('dotnet', ['pack', '-c', 'Release']), 'dotnet pack -c Release');
});

test('renderCmd: safe-charset args stay unquoted', () => {
  // `A-Za-z0-9._-/=:` is the whitelist — a NuGet-style path is safe.
  assert.equal(
    renderCmd('dotnet', ['nuget', 'push', '/out/foo.1.2.3.nupkg', '--source', 'https://api.nuget.org/v3/index.json']),
    'dotnet nuget push /out/foo.1.2.3.nupkg --source https://api.nuget.org/v3/index.json',
  );
});

test('renderCmd: args with whitespace get double-quoted', () => {
  assert.equal(
    renderCmd('gh', ['release', 'create', '--title', 'MTConnect.NET 7.0.0-dev.42']),
    'gh release create --title "MTConnect.NET 7.0.0-dev.42"',
  );
});

test('renderCmd: interior double quotes are backslash-escaped', () => {
  assert.equal(
    renderCmd('sh', ['-c', 'echo "hi"']),
    'sh -c "echo \\"hi\\""',
  );
});

test('renderCmd: shell-special chars force quoting', () => {
  // `$`, `` ` ``, `*`, `;`, `&`, `|`, `(`, `)`, `<`, `>` are all outside
  // the whitelist, so each must round-trip as a quoted arg.
  assert.equal(renderCmd('e', ['a$b']), 'e "a$b"');
  assert.equal(renderCmd('e', ['a;b']), 'e "a;b"');
  assert.equal(renderCmd('e', ['a|b']), 'e "a|b"');
  assert.equal(renderCmd('e', ['a b']), 'e "a b"');
  assert.equal(renderCmd('e', ['a*b']), 'e "a*b"');
  assert.equal(renderCmd('e', ['']), 'e ""');
});

test('renderCmd: empty argv renders as the bare cmd', () => {
  assert.equal(renderCmd('gh', []), 'gh');
});

// ─── renderCmd: secret redaction ───────────────────────────────
test('renderCmd: `--api-key value` value is redacted, flag preserved', () => {
  // Documented contract: the value AFTER any SECRET_ARG_NAMES entry is
  // replaced by `<redacted>` in the log line only. Pin the exact
  // rendering so a future refactor cannot silently leak the key.
  assert.equal(
    renderCmd('dotnet', ['nuget', 'push', '--api-key', 'topsecret']),
    'dotnet nuget push --api-key <redacted>',
  );
});

test('renderCmd: `--password value` and `-p value` values are redacted', () => {
  assert.equal(
    renderCmd('mysql', ['-u', 'root', '--password', 'hunter2']),
    'mysql -u root --password <redacted>',
  );
  // `-p` alone; the arg that follows is the secret.
  assert.equal(
    renderCmd('curl', ['-u', 'user', '-p', 'hunter2', 'https://x']),
    'curl -u user -p <redacted> https://x',
  );
});

test('renderCmd: `--token value` is redacted', () => {
  assert.equal(
    renderCmd('gh', ['auth', 'login', '--token', 'ghp_xxx']),
    'gh auth login --token <redacted>',
  );
});

test('renderCmd: `--api-key=value` (equals form) is redacted, key preserved', () => {
  // The equals-form arg is a single argv token — the redactor must
  // detect it and rewrite only the RHS.
  assert.equal(
    renderCmd('dotnet', ['nuget', 'push', '--api-key=topsecret']),
    'dotnet nuget push --api-key=<redacted>',
  );
  assert.equal(
    renderCmd('gh', ['--token=ghp_xxx', 'auth']),
    'gh --token=<redacted> auth',
  );
});

test('renderCmd: multiple secret args each redact only their own value', () => {
  // Guards against a "sticky" redactNext state that would drop
  // non-secret trailing args.
  assert.equal(
    renderCmd('cli', [
      '--api-key', 'k1', '--source', 'https://x',
      '--token', 'k2', '--verbose',
    ]),
    'cli --api-key <redacted> --source https://x --token <redacted> --verbose',
  );
});

test('renderCmd: secret arg at end-of-argv (no value) does not crash', () => {
  // A malformed invocation where a secret flag is the last arg with no
  // value. `redactNext` is set but never consumed — must not throw and
  // must render the flag alone.
  assert.equal(renderCmd('dotnet', ['nuget', 'push', '--api-key']), 'dotnet nuget push --api-key');
});

test('renderCmd: SECRET_ARG_NAMES export lists the documented four names', () => {
  // Pins the API contract so a rename / deletion is a test failure
  // rather than a silent regression.
  assert.deepEqual(
    [...SECRET_ARG_NAMES].sort(),
    ['--api-key', '--password', '--token', '-p'].sort(),
  );
});

// ─── parseDryRun ────────────────────────────────────────────────
test('parseDryRun: --dry-run flag detected, stripped from rest', () => {
  const r = parseDryRun(['--version', '1.0.0', '--dry-run']);
  assert.equal(r.dryRun, true);
  assert.deepEqual(r.rest, ['--version', '1.0.0']);
});

test('parseDryRun: absent flag → dryRun false', () => {
  const r = parseDryRun(['--version', '1.0.0']);
  assert.equal(r.dryRun, false);
  assert.deepEqual(r.rest, ['--version', '1.0.0']);
});

test('parseDryRun: empty argv → dryRun false, empty rest', () => {
  const r = parseDryRun([]);
  assert.equal(r.dryRun, false);
  assert.deepEqual(r.rest, []);
});

test('parseDryRun: flag in any position, single occurrence', () => {
  // Guard the reduce order — a `--dry-run` at the front, middle, or
  // end must produce the same result.
  const a = parseDryRun(['--dry-run', '--x', 'y']);
  const b = parseDryRun(['--x', '--dry-run', 'y']);
  const c = parseDryRun(['--x', 'y', '--dry-run']);
  assert.deepEqual([a.dryRun, a.rest], [true, ['--x', 'y']]);
  assert.deepEqual([b.dryRun, b.rest], [true, ['--x', 'y']]);
  assert.deepEqual([c.dryRun, c.rest], [true, ['--x', 'y']]);
});

test('parseDryRun: multiple --dry-run flags remain truthy, none leak into rest', () => {
  const r = parseDryRun(['--dry-run', '--x', '--dry-run']);
  assert.equal(r.dryRun, true);
  assert.deepEqual(r.rest, ['--x']);
});

test('parseDryRun: --dry-run=true is NOT recognized (equality-only match)', () => {
  // Documented behavior — the parser is `arg === '--dry-run'`, not
  // `.startsWith`. `--dry-run=true` therefore lands in `rest` and
  // `parseArgs` downstream will accept it separately if declared.
  // Pin the current shape so a future author does not loosen it.
  const r = parseDryRun(['--dry-run=true']);
  assert.equal(r.dryRun, false);
  assert.deepEqual(r.rest, ['--dry-run=true']);
});

// ─── requireEnv ─────────────────────────────────────────────────
test('requireEnv: present var returned as-is', () => {
  const key = '__MTC_TEST_REQUIRE_ENV_PRESENT';
  process.env[key] = 'hello';
  try {
    assert.equal(requireEnv(key), 'hello');
  } finally {
    delete process.env[key];
  }
});

test('requireEnv: missing var throws with the var name in the message', () => {
  const key = '__MTC_TEST_REQUIRE_ENV_MISSING';
  delete process.env[key];
  assert.throws(() => requireEnv(key), new RegExp(key));
});

test('requireEnv: empty and whitespace-only values treated as missing', () => {
  const key = '__MTC_TEST_REQUIRE_ENV_EMPTY';
  process.env[key] = '';
  try {
    assert.throws(() => requireEnv(key), /required but not set/);
  } finally {
    delete process.env[key];
  }
  process.env[key] = '   ';
  try {
    assert.throws(() => requireEnv(key), /required but not set/);
  } finally {
    delete process.env[key];
  }
});

// ─── optionalEnv ────────────────────────────────────────────────
test('optionalEnv: present var returned as-is', () => {
  const key = '__MTC_TEST_OPTIONAL_ENV_PRESENT';
  process.env[key] = 'value';
  try {
    assert.equal(optionalEnv(key), 'value');
  } finally {
    delete process.env[key];
  }
});

test('optionalEnv: missing var returns undefined (no throw)', () => {
  const key = '__MTC_TEST_OPTIONAL_ENV_MISSING';
  delete process.env[key];
  assert.equal(optionalEnv(key), undefined);
});

test('optionalEnv: empty and whitespace-only values treated as undefined', () => {
  const key = '__MTC_TEST_OPTIONAL_ENV_EMPTY';
  process.env[key] = '';
  try {
    assert.equal(optionalEnv(key), undefined);
  } finally {
    delete process.env[key];
  }
  process.env[key] = '   ';
  try {
    assert.equal(optionalEnv(key), undefined);
  } finally {
    delete process.env[key];
  }
});

// ─── run (dry-run path only) ────────────────────────────────────
test('run: dry-run logs the rendered cmd and does not spawn', async () => {
  const out = await captureStdout(async () => {
    await run('docker', ['push', 'x:1.0'], { dryRun: true });
  });
  assert.equal(out, '[dry-run] docker push x:1.0\n');
});

test('run: dry-run preserves argument quoting from renderCmd', async () => {
  const out = await captureStdout(async () => {
    await run('gh', ['release', 'create', '--title', 'MTConnect.NET 7.0.0-dev.42'], {
      dryRun: true,
    });
  });
  assert.equal(out, '[dry-run] gh release create --title "MTConnect.NET 7.0.0-dev.42"\n');
});

// ─── main runner ────────────────────────────────────────────────
const main = async (): Promise<void> => {
  let passed = 0;
  for (const c of cases) {
    try {
      await c.fn();
    } catch (err) {
      process.stderr.write(`  FAIL  ${c.name}\n`);
      throw err;
    }
    passed += 1;
    process.stdout.write(`  ok  ${c.name}\n`);
  }
  process.stdout.write(`\n${passed} assertions passed.\n`);
};

main().catch((err) => {
  process.stderr.write(`${(err as Error).stack ?? err}\n`);
  process.exit(1);
});
