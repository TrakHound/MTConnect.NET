#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `docker-push.ts`. The
 * spawning half (`main` shelling out to `docker push` / `docker buildx
 * imagetools`) is out of scope for the pure-unit suite. Run with:
 *
 *   tsx tools/release/docker-push.test.ts
 */

import { strict as assert } from 'node:assert';
import { archSuffixFor, parseOptions } from './docker-push.ts';

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

// ─── parseOptions: per-arch push mode ───────────────────────────
test('parseOptions: per-arch push — platform set, manifest false', () => {
  const o = parseOptions(['--version', '1.0.0', '--platform', 'linux/amd64']);
  assert.equal(o.version, '1.0.0');
  assert.equal(o.platform, 'linux/amd64');
  assert.equal(o.manifest, false);
  assert.equal(o.image, 'trakhound/mtconnect-agent');
  assert.equal(o.dryRun, false);
});

test('parseOptions: per-arch push honors --image override', () => {
  const o = parseOptions([
    '--version', '1.0.0',
    '--platform', 'linux/arm64',
    '--image', 'foo/bar',
  ]);
  assert.equal(o.image, 'foo/bar');
  assert.equal(o.platform, 'linux/arm64');
});

// ─── parseOptions: manifest mode ────────────────────────────────
test('parseOptions: manifest mode — platform undefined, manifest true', () => {
  const o = parseOptions(['--version', '1.0.0', '--manifest']);
  assert.equal(o.manifest, true);
  assert.equal(o.platform, undefined);
  assert.equal(o.image, 'trakhound/mtconnect-agent');
});

// ─── parseOptions: --dry-run flag ───────────────────────────────
test('parseOptions: --dry-run is consumed', () => {
  const o = parseOptions(['--dry-run', '--version', '1.0.0', '--manifest']);
  assert.equal(o.dryRun, true);
  assert.equal(o.manifest, true);
});

// ─── parseOptions: error paths ──────────────────────────────────
test('parseOptions: missing --version throws', () => {
  assert.throws(
    () => parseOptions(['--platform', 'linux/amd64']),
    /--version is required/,
  );
});

test('parseOptions: neither --platform nor --manifest throws', () => {
  assert.throws(
    () => parseOptions(['--version', '1.0.0']),
    /Either --platform or --manifest must be provided/,
  );
});

test('parseOptions: --platform and --manifest are mutually exclusive', () => {
  assert.throws(
    () => parseOptions([
      '--version', '1.0.0',
      '--platform', 'linux/amd64',
      '--manifest',
    ]),
    /mutually exclusive/,
  );
});

test('parseOptions: unrecognized --platform throws', () => {
  assert.throws(
    () => parseOptions(['--version', '1.0.0', '--platform', 'linux/riscv64']),
    /linux\/amd64 or linux\/arm64/,
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
