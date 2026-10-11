#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `nuget-push.ts`. The
 * `dotnet nuget push` shell-out is out of scope for a pure-unit
 * suite. Run with:
 *
 *   tsx tools/release/nuget-push.test.ts
 */

import { strict as assert } from 'node:assert';
import { parseOptions } from './nuget-push.ts';

const cases: Array<{ name: string; fn: () => void }> = [];
const test = (name: string, fn: () => void): void => {
  cases.push({ name, fn });
};

// ─── parseOptions ───────────────────────────────────────────────
test('parseOptions: happy path — flags override defaults', () => {
  const o = parseOptions([
    '--input', '/tmp/nupkg',
    '--source', 'https://my.feed/index.json',
    '--api-key', 'topsecret',
  ]);
  assert.equal(o.input, '/tmp/nupkg');
  assert.equal(o.source, 'https://my.feed/index.json');
  assert.equal(o.apiKey, 'topsecret');
  assert.equal(o.dryRun, false);
});

test('parseOptions: --input defaults to <repo>/build/output/nupkg', () => {
  // Wipe NUGET_API_KEY so the apiKey default resolution stays inert.
  const prev = process.env.NUGET_API_KEY;
  delete process.env.NUGET_API_KEY;
  try {
    const o = parseOptions([]);
    assert.ok(o.input.endsWith('/build/output/nupkg'), o.input);
  } finally {
    if (prev !== undefined) process.env.NUGET_API_KEY = prev;
  }
});

test('parseOptions: --source defaults to nuget.org v3 index', () => {
  const prev = process.env.NUGET_API_KEY;
  delete process.env.NUGET_API_KEY;
  try {
    const o = parseOptions([]);
    assert.equal(o.source, 'https://api.nuget.org/v3/index.json');
  } finally {
    if (prev !== undefined) process.env.NUGET_API_KEY = prev;
  }
});

test('parseOptions: --api-key falls back to NUGET_API_KEY env var', () => {
  const prev = process.env.NUGET_API_KEY;
  process.env.NUGET_API_KEY = 'from-env';
  try {
    const o = parseOptions([]);
    assert.equal(o.apiKey, 'from-env');
  } finally {
    if (prev === undefined) delete process.env.NUGET_API_KEY;
    else process.env.NUGET_API_KEY = prev;
  }
});

test('parseOptions: explicit --api-key beats NUGET_API_KEY env var', () => {
  const prev = process.env.NUGET_API_KEY;
  process.env.NUGET_API_KEY = 'from-env';
  try {
    const o = parseOptions(['--api-key', 'from-flag']);
    assert.equal(o.apiKey, 'from-flag');
  } finally {
    if (prev === undefined) delete process.env.NUGET_API_KEY;
    else process.env.NUGET_API_KEY = prev;
  }
});

test('parseOptions: --api-key absent + env unset → undefined', () => {
  const prev = process.env.NUGET_API_KEY;
  delete process.env.NUGET_API_KEY;
  try {
    const o = parseOptions([]);
    assert.equal(o.apiKey, undefined);
  } finally {
    if (prev !== undefined) process.env.NUGET_API_KEY = prev;
  }
});

test('parseOptions: --dry-run consumed', () => {
  const o = parseOptions(['--dry-run']);
  assert.equal(o.dryRun, true);
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
