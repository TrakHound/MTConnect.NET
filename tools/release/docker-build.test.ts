#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `docker-build.ts`. The
 * spawning half (`main` shelling out to `docker buildx`) is out of
 * scope for a pure-unit suite — its behavior is covered by the
 * integration matrix that runs `--dry-run` end-to-end. Run with:
 *
 *   tsx tools/release/docker-build.test.ts
 */

import { strict as assert } from 'node:assert';
import { archSuffixFor, parseOptions } from './docker-build.ts';

const cases: Array<{ name: string; fn: () => void }> = [];
const test = (name: string, fn: () => void): void => {
  cases.push({ name, fn });
};

// ─── archSuffixFor ──────────────────────────────────────────────
test('archSuffixFor: linux/amd64 → amd64', () => {
  assert.equal(archSuffixFor('linux/amd64'), 'amd64');
});

test('archSuffixFor: linux/arm64 → arm64', () => {
  assert.equal(archSuffixFor('linux/arm64'), 'arm64');
});

// ─── parseOptions ───────────────────────────────────────────────
test('parseOptions: happy path — every field populated', () => {
  const o = parseOptions([
    '--version', '7.0.0-dev.42',
    '--platform', 'linux/amd64',
    '--image', 'myorg/mtc',
  ]);
  assert.equal(o.version, '7.0.0-dev.42');
  assert.equal(o.platform, 'linux/amd64');
  assert.equal(o.image, 'myorg/mtc');
  assert.equal(o.dryRun, false);
});

test('parseOptions: --image defaults to trakhound/mtconnect-agent', () => {
  const o = parseOptions(['--version', '1.0.0', '--platform', 'linux/arm64']);
  assert.equal(o.image, 'trakhound/mtconnect-agent');
  assert.equal(o.platform, 'linux/arm64');
});

test('parseOptions: --dry-run is consumed pre-parseArgs and never leaks', () => {
  const o = parseOptions(['--dry-run', '--version', '1.0.0', '--platform', 'linux/amd64']);
  assert.equal(o.dryRun, true);
  assert.equal(o.version, '1.0.0');
});

test('parseOptions: missing --version throws', () => {
  assert.throws(
    () => parseOptions(['--platform', 'linux/amd64']),
    /--version is required/,
  );
});

test('parseOptions: unrecognized --platform throws with the offending value', () => {
  assert.throws(
    () => parseOptions(['--version', '1.0.0', '--platform', 'darwin/arm64']),
    /darwin\/arm64/,
  );
  // linux/amd64/arm64 typo:
  assert.throws(
    () => parseOptions(['--version', '1.0.0', '--platform', 'linux/x86_64']),
    /linux\/amd64 or linux\/arm64/,
  );
});

test('parseOptions: missing --platform names it in the error', () => {
  assert.throws(
    () => parseOptions(['--version', '1.0.0']),
    /--platform/,
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
