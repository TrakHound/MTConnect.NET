#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `sbom.ts`. The
 * `dotnet sbom-tool` and `syft` shell-outs are out of scope for a
 * pure-unit suite. Run with:
 *
 *   tsx tools/release/sbom.test.ts
 */

import { strict as assert } from 'node:assert';
import { parseOptions } from './sbom.ts';

const cases: Array<{ name: string; fn: () => void }> = [];
const test = (name: string, fn: () => void): void => {
  cases.push({ name, fn });
};

// ─── parseOptions: mode selection ───────────────────────────────
test('parseOptions: --nuget selects nuget mode, dockerImage undefined', () => {
  const o = parseOptions(['--nuget']);
  assert.equal(o.mode, 'nuget');
  assert.equal(o.dockerImage, undefined);
});

test('parseOptions: --docker <image:tag> selects docker mode', () => {
  const o = parseOptions(['--docker', 'trakhound/mtconnect-agent:1.0.0']);
  assert.equal(o.mode, 'docker');
  assert.equal(o.dockerImage, 'trakhound/mtconnect-agent:1.0.0');
});

test('parseOptions: neither --nuget nor --docker throws', () => {
  assert.throws(
    () => parseOptions([]),
    /Either --nuget or --docker <image:tag> is required/,
  );
});

test('parseOptions: --nuget and --docker are mutually exclusive', () => {
  assert.throws(
    () => parseOptions(['--nuget', '--docker', 'img:1']),
    /mutually exclusive/,
  );
});

// ─── parseOptions: --input / --output defaults ──────────────────
test('parseOptions: --input defaults to <repo>/build/output/nupkg', () => {
  const o = parseOptions(['--nuget']);
  assert.ok(o.input.endsWith('/build/output/nupkg'), o.input);
});

test('parseOptions: --output defaults to <repo>/build/output/sbom', () => {
  const o = parseOptions(['--nuget']);
  assert.ok(o.output.endsWith('/build/output/sbom'), o.output);
});

test('parseOptions: --input override honored', () => {
  const o = parseOptions(['--nuget', '--input', '/tmp/nupkg']);
  assert.equal(o.input, '/tmp/nupkg');
});

test('parseOptions: --output override honored', () => {
  const o = parseOptions(['--nuget', '--output', '/tmp/sbom']);
  assert.equal(o.output, '/tmp/sbom');
});

test('parseOptions: --dry-run consumed', () => {
  const o = parseOptions(['--dry-run', '--nuget']);
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
