#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure helpers exported by `gh-release-create.ts`.
 * Exercises `parseOptions`, `collectAssets`, and `renderReleaseNotes`.
 * The `gh release create` shell-out is out of scope for a pure-unit
 * suite. Run with:
 *
 *   tsx tools/release/gh-release-create.test.ts
 */

import { strict as assert } from 'node:assert';
import { mkdtempSync, writeFileSync, mkdirSync, rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import {
  collectAssets,
  parseOptions,
  renderReleaseNotes,
} from './gh-release-create.ts';

const cases: Array<{ name: string; fn: () => void }> = [];
const test = (name: string, fn: () => void): void => {
  cases.push({ name, fn });
};

// ─── parseOptions ───────────────────────────────────────────────
test('parseOptions: happy path — every field populated', () => {
  const o = parseOptions([
    '--version', '7.0.0-dev.42',
    '--repo', 'me/proj',
    '--assets', '/tmp/nupkg',
    '--assets', '/tmp/sbom',
    '--docker-image', 'foo/bar:1',
    '--target', 'deadbeef',
  ]);
  assert.equal(o.version, '7.0.0-dev.42');
  assert.equal(o.repo, 'me/proj');
  assert.deepEqual(o.assetDirs, ['/tmp/nupkg', '/tmp/sbom']);
  assert.equal(o.dockerImage, 'foo/bar:1');
  assert.equal(o.target, 'deadbeef');
  assert.equal(o.dryRun, false);
});

test('parseOptions: defaults — repo, assetDirs, dockerImage, target undefined', () => {
  const o = parseOptions(['--version', '1.0.0']);
  assert.equal(o.repo, 'TrakHound/MTConnect.NET');
  assert.equal(o.dockerImage, undefined);
  assert.equal(o.target, undefined);
  // assetDirs default to build/output/nupkg + build/output/sbom under
  // repo root; assert shape rather than exact paths (repo-root-dependent).
  assert.equal(o.assetDirs.length, 2);
  assert.ok(o.assetDirs[0]!.endsWith('/build/output/nupkg'));
  assert.ok(o.assetDirs[1]!.endsWith('/build/output/sbom'));
});

test('parseOptions: --dry-run consumed', () => {
  const o = parseOptions(['--dry-run', '--version', '1.0.0']);
  assert.equal(o.dryRun, true);
});

test('parseOptions: missing --version throws', () => {
  assert.throws(() => parseOptions([]), /--version is required/);
});

// ─── collectAssets ──────────────────────────────────────────────
test('collectAssets: empty input list → empty output', () => {
  assert.deepEqual(collectAssets([]), []);
});

test('collectAssets: missing dir → skipped with stderr warning, not thrown', () => {
  // capture stderr — the function must emit a `skipping missing dir`
  // line and continue, not throw.
  const original = process.stderr.write.bind(process.stderr);
  let buf = '';
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (process.stderr as any).write = (chunk: string | Uint8Array): boolean => {
    buf += typeof chunk === 'string' ? chunk : Buffer.from(chunk).toString('utf8');
    return true;
  };
  try {
    const r = collectAssets(['/definitely/does/not/exist/gh-release-test']);
    assert.deepEqual(r, []);
    assert.match(buf, /skipping missing dir/);
  } finally {
    (process.stderr as unknown as { write: typeof original }).write = original;
  }
});

test('collectAssets: enumerates files, skips dotfiles', () => {
  const dir = mkdtempSync(join(tmpdir(), 'gh-release-collect-'));
  try {
    writeFileSync(join(dir, 'a.nupkg'), 'x');
    writeFileSync(join(dir, 'b.spdx.json'), 'x');
    writeFileSync(join(dir, '.hidden'), 'x');
    mkdirSync(join(dir, '.git'));
    const r = collectAssets([dir]);
    const names = r.map((p) => p.split('/').pop()).sort();
    assert.deepEqual(names, ['a.nupkg', 'b.spdx.json']);
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
});

test('collectAssets: aggregates across multiple dirs', () => {
  const dirA = mkdtempSync(join(tmpdir(), 'gh-release-collect-a-'));
  const dirB = mkdtempSync(join(tmpdir(), 'gh-release-collect-b-'));
  try {
    writeFileSync(join(dirA, 'one.nupkg'), 'x');
    writeFileSync(join(dirB, 'two.spdx.json'), 'x');
    const r = collectAssets([dirA, dirB]);
    assert.equal(r.length, 2);
    assert.ok(r.some((p) => p.endsWith('one.nupkg')));
    assert.ok(r.some((p) => p.endsWith('two.spdx.json')));
  } finally {
    rmSync(dirA, { recursive: true, force: true });
    rmSync(dirB, { recursive: true, force: true });
  }
});

test('collectAssets: recurses into nested subdirectories', () => {
  // `Microsoft.Sbom.DotNetTool` writes the SBOM under
  // `_manifest/spdx_2.2/manifest.spdx.json` — pin that shape so the
  // nested path is guaranteed to land on the release attachment list.
  const dir = mkdtempSync(join(tmpdir(), 'gh-release-collect-nested-'));
  try {
    mkdirSync(join(dir, '_manifest', 'spdx_2.2'), { recursive: true });
    writeFileSync(join(dir, '_manifest', 'spdx_2.2', 'manifest.spdx.json'), 'x');
    writeFileSync(join(dir, 'top.nupkg'), 'x');
    const r = collectAssets([dir]);
    const names = r.map((p) => p.split('/').pop()).sort();
    assert.deepEqual(names, ['manifest.spdx.json', 'top.nupkg']);
    // Full path of the nested manifest survives, not just the basename.
    assert.ok(
      r.some((p) => p.endsWith('/_manifest/spdx_2.2/manifest.spdx.json')),
      `no nested manifest path in ${r.join(', ')}`,
    );
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
});

test('collectAssets: skips nested dotfiles and dot-directories', () => {
  // A `.git` directory or a `.DS_Store` under any depth must NOT
  // leak into the attachment list — pin so the recursive walk
  // respects the dotfile skip at every level, not just the top.
  const dir = mkdtempSync(join(tmpdir(), 'gh-release-collect-nested-dot-'));
  try {
    mkdirSync(join(dir, 'sub', '.git'), { recursive: true });
    writeFileSync(join(dir, 'sub', '.git', 'HEAD'), 'x');
    writeFileSync(join(dir, 'sub', '.DS_Store'), 'x');
    writeFileSync(join(dir, 'sub', 'keep.nupkg'), 'x');
    const r = collectAssets([dir]);
    const basenames = r.map((p) => p.split('/').pop()).sort();
    assert.deepEqual(basenames, ['keep.nupkg']);
  } finally {
    rmSync(dir, { recursive: true, force: true });
  }
});

// ─── renderReleaseNotes ─────────────────────────────────────────
test('renderReleaseNotes: header includes MTConnect.NET + version', () => {
  const notes = renderReleaseNotes('7.0.0-dev.42', [], undefined);
  assert.match(notes, /^# MTConnect\.NET 7\.0\.0-dev\.42$/m);
});

test('renderReleaseNotes: empty assets renders explicit "no assets" line', () => {
  const notes = renderReleaseNotes('1.0.0', [], undefined);
  assert.match(notes, /_No assets attached\._/);
});

test('renderReleaseNotes: each asset rendered as backtick-quoted basename', () => {
  const notes = renderReleaseNotes('1.0.0', [
    '/build/output/nupkg/MTConnect.NET.Common.1.0.0.nupkg',
    '/build/output/sbom/manifest.spdx.json',
  ], undefined);
  assert.match(notes, /- `MTConnect\.NET\.Common\.1\.0\.0\.nupkg`/);
  assert.match(notes, /- `manifest\.spdx\.json`/);
});

test('renderReleaseNotes: docker section absent when dockerImage undefined', () => {
  const notes = renderReleaseNotes('1.0.0', [], undefined);
  assert.equal(/## Docker image/.test(notes), false);
});

test('renderReleaseNotes: docker section present with pull command when dockerImage set', () => {
  const notes = renderReleaseNotes('1.0.0', [], 'trakhound/mtconnect-agent:1.0.0');
  assert.match(notes, /## Docker image/);
  assert.match(notes, /docker pull trakhound\/mtconnect-agent:1\.0\.0/);
});

test('renderReleaseNotes: opening prose warns not-for-production', () => {
  // Docstring contract: "Not intended for production use". Pin so a
  // future refactor of the boilerplate does not silently drop the
  // pre-release warning.
  const notes = renderReleaseNotes('1.0.0', [], undefined);
  assert.match(notes, /Not intended for production use/i);
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
