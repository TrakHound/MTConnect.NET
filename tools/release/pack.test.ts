#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `pack.ts`. The `dotnet
 * pack` shell-out is out of scope for a pure-unit suite. Run with:
 *
 *   tsx tools/release/pack.test.ts
 */

import { strict as assert } from 'node:assert';
import { parseOptions } from './pack.ts';

const cases: Array<{ name: string; fn: () => void }> = [];
const test = (name: string, fn: () => void): void => {
  cases.push({ name, fn });
};

// ─── parseOptions ───────────────────────────────────────────────
test('parseOptions: happy path — every field populated', () => {
  const o = parseOptions(['--version', '7.0.0-dev.42', '--output', '/tmp/out']);
  assert.equal(o.version, '7.0.0-dev.42');
  assert.equal(o.output, '/tmp/out');
  assert.equal(o.dryRun, false);
});

test('parseOptions: --output defaults to <repo>/build/output/nupkg', () => {
  const o = parseOptions(['--version', '1.0.0']);
  assert.ok(o.output.endsWith('/build/output/nupkg'), o.output);
});

test('parseOptions: --dry-run consumed', () => {
  const o = parseOptions(['--dry-run', '--version', '1.0.0']);
  assert.equal(o.dryRun, true);
  assert.equal(o.version, '1.0.0');
});

test('parseOptions: missing --version throws with example hint', () => {
  // Docstring: "Fails fast on missing `--version`". Also assert the
  // error hint text so the CLI ergonomics do not silently drift.
  assert.throws(
    () => parseOptions([]),
    /--version is required.*7\.0\.0-dev\.42/,
  );
});

// ─── main runner ────────────────────────────────────────────────
let passed = 0;
for (const c of cases) {
  try {
    c.fn();
  } catch (err) {
    process.stderr.write(`  FAIL  ${c.name}\n`);
    throw err;
  }
  passed += 1;
  process.stdout.write(`  ok  ${c.name}\n`);
}
process.stdout.write(`\n${passed} assertions passed.\n`);
