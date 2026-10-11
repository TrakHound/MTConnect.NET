#!/usr/bin/env -S npx tsx
/**
 * Unit tests for the pure functions in `semver-bump.ts`. Run with:
 *
 *   tsx tools/ci/semver-bump.test.ts
 *
 * Exits 0 when every assertion passes, non-zero otherwise. Not wired
 * into a formal test runner (Jest/Vitest) because the module has zero
 * runtime deps in these tests and adding a runner just for one file
 * paid nothing back.
 */

import { strict as assert } from 'node:assert';
import { execSync } from 'node:child_process';
import { mkdtempSync, rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import {
  aggregateBump,
  applyBump,
  bumpKindFor,
  commitsInRange,
  countCommitsSinceLastDevBoundary,
  isDevTag,
  isStableCutMarker,
} from './semver-bump.ts';

/** Simple test-runner shim — each `test(name, fn)` runs immediately
 *  and prints pass/fail. A failure throws and terminates the script,
 *  so the exit code is 1 on the first miss. */
let passed = 0;
const test = (name: string, fn: () => void): void => {
  fn();
  passed += 1;
  process.stdout.write(`  ok  ${name}\n`);
};

// ─── bumpKindFor ────────────────────────────────────────────────
test('bumpKindFor: feat → minor', () => {
  assert.equal(bumpKindFor('feat: add xyz', ''), 'minor');
  assert.equal(bumpKindFor('feat(agent): add xyz', ''), 'minor');
});

test('bumpKindFor: fix → patch', () => {
  assert.equal(bumpKindFor('fix: correct off-by-one', ''), 'patch');
  assert.equal(bumpKindFor('fix(common): correct', ''), 'patch');
});

test('bumpKindFor: bang → major', () => {
  assert.equal(bumpKindFor('feat!: drop v6 API', ''), 'major');
  assert.equal(bumpKindFor('fix(agent)!: rename field', ''), 'major');
});

test('bumpKindFor: BREAKING CHANGE footer → major', () => {
  assert.equal(
    bumpKindFor('feat: add xyz', 'BREAKING CHANGE: renamed field'),
    'major',
  );
  assert.equal(
    bumpKindFor('feat: add xyz', 'BREAKING-CHANGE: renamed field'),
    'major',
  );
});

test('bumpKindFor: chore/docs/build → patch', () => {
  assert.equal(bumpKindFor('chore: bump deps', ''), 'patch');
  assert.equal(bumpKindFor('docs(agent): describe xyz', ''), 'patch');
  assert.equal(bumpKindFor('build(ci): tweak workflow', ''), 'patch');
});

test('bumpKindFor: every documented patch type resolves to patch', () => {
  // Doc lists `fix|chore|docs|style|refactor|perf|test|build|ci|revert`;
  // pin every arm so a docstring drift or regex tweak surfaces as a
  // failure rather than a silently-lost bump kind.
  for (const type of ['refactor', 'style', 'perf', 'test', 'ci', 'revert']) {
    assert.equal(bumpKindFor(`${type}: do the thing`, ''), 'patch', type);
    assert.equal(bumpKindFor(`${type}(scope): do the thing`, ''), 'patch', `${type}(scope)`);
  }
});

test('bumpKindFor: unknown conventional-shaped type → none', () => {
  // `foo:` matches the type regex but is not in the known-types set,
  // so it falls through to `none`. Distinguishes the "no match" arm
  // from the "matched but unknown" arm — both return `none` but via
  // different code paths.
  assert.equal(bumpKindFor('foo: bar', ''), 'none');
  assert.equal(bumpKindFor('wip(scope): thing', ''), 'none');
});

test('bumpKindFor: case-insensitive type recognition', () => {
  // Docstring: "subject starts with `feat` (case-insensitive) → minor".
  // The `type.toLowerCase()` branch is only exercised end-to-end when
  // the input arrives mixed-case; pin the contract.
  assert.equal(bumpKindFor('FEAT: shout', ''), 'minor');
  assert.equal(bumpKindFor('Feat(agent): pascal', ''), 'minor');
  assert.equal(bumpKindFor('FIX: shout', ''), 'patch');
  assert.equal(bumpKindFor('Fix(agent): pascal', ''), 'patch');
  // Bang also — the `[a-zA-Z]+` character class covers both cases.
  assert.equal(bumpKindFor('FEAT!: shout', ''), 'major');
  assert.equal(bumpKindFor('Fix(agent)!: pascal', ''), 'major');
});

test('bumpKindFor: BREAKING CHANGE detection covers each anchor', () => {
  // The regex `(^|\n)BREAKING[ -]CHANGE:` has two anchors — start-of-
  // body and post-newline. The existing test only hits the start-of-
  // body arm; pin the newline arm too.
  assert.equal(
    bumpKindFor('feat: add', 'Some prose paragraph.\n\nBREAKING CHANGE: renamed field'),
    'major',
  );
  assert.equal(
    bumpKindFor('feat: add', 'Prose\nBREAKING-CHANGE: renamed'),
    'major',
  );
  // Case-insensitivity on the footer marker (per `/i` flag).
  assert.equal(
    bumpKindFor('feat: add', 'breaking change: lowercased'),
    'major',
  );
  // A `BREAKING CHANGE` mid-word (no leading anchor) must NOT trigger.
  assert.equal(
    bumpKindFor('feat: add', 'This is not-a-BREAKING CHANGE: really'),
    'minor',
  );
});

test('bumpKindFor: non-conventional → none', () => {
  assert.equal(bumpKindFor('WIP', ''), 'none');
  assert.equal(bumpKindFor('add stuff', ''), 'none');
  assert.equal(bumpKindFor('Merge branch xyz', ''), 'none');
  // Empty subject — the trim collapses it, no type match, `none`.
  assert.equal(bumpKindFor('', ''), 'none');
  assert.equal(bumpKindFor('   ', ''), 'none');
});

// ─── aggregateBump ──────────────────────────────────────────────
test('aggregateBump: major beats everything', () => {
  assert.equal(aggregateBump(['patch', 'major', 'minor']), 'major');
});

test('aggregateBump: minor beats patch and none', () => {
  assert.equal(aggregateBump(['patch', 'minor', 'none']), 'minor');
});

test('aggregateBump: patch beats none only', () => {
  assert.equal(aggregateBump(['none', 'patch', 'none']), 'patch');
});

test('aggregateBump: empty → none', () => {
  assert.equal(aggregateBump([]), 'none');
});

test('aggregateBump: all-none → none (fall-through arm)', () => {
  // Distinct from the empty case: exercises the trailing `return 'none'`
  // after every `includes(...)` check misses. A single `none` and a
  // list of `none`s must both fall through.
  assert.equal(aggregateBump(['none']), 'none');
  assert.equal(aggregateBump(['none', 'none', 'none']), 'none');
});

// ─── applyBump ──────────────────────────────────────────────────
test('applyBump: major → X+1.0.0', () => {
  assert.equal(applyBump('v6.6.0', 'major'), '7.0.0');
  assert.equal(applyBump('6.6.0', 'major'), '7.0.0');
});

test('applyBump: minor → X.Y+1.0', () => {
  assert.equal(applyBump('v6.6.0', 'minor'), '6.7.0');
});

test('applyBump: patch → X.Y.Z+1', () => {
  assert.equal(applyBump('v6.6.0', 'patch'), '6.6.1');
});

test('applyBump: none → patch bump (avoids version collision)', () => {
  assert.equal(applyBump('v6.6.0', 'none'), '6.6.1');
});

test('applyBump: rejects invalid base', () => {
  assert.throws(() => applyBump('nope', 'minor'), /not a valid semver/);
  // Also rejects an empty string and a v-prefix-only string.
  assert.throws(() => applyBump('', 'minor'), /not a valid semver/);
  assert.throws(() => applyBump('v', 'minor'), /not a valid semver/);
});

test('applyBump: strips v-prefix for every kind', () => {
  // The `v` strip is a single line but each kind arm consumes the
  // stripped value differently. Pin every kind × prefix combination.
  assert.equal(applyBump('v6.6.0', 'minor'), '6.7.0');
  assert.equal(applyBump('6.6.0', 'minor'), '6.7.0');
  assert.equal(applyBump('v6.6.0', 'patch'), '6.6.1');
  assert.equal(applyBump('6.6.0', 'patch'), '6.6.1');
  assert.equal(applyBump('v6.6.0', 'none'), '6.6.1');
  assert.equal(applyBump('6.6.0', 'none'), '6.6.1');
});

// ─── isStableCutMarker ──────────────────────────────────────────
test('isStableCutMarker: bare marker', () => {
  assert.equal(isStableCutMarker('chore(release): publish new stable'), true);
});

test('isStableCutMarker: with squash-merge PR suffix', () => {
  assert.equal(
    isStableCutMarker('chore(release): publish new stable (#123)'),
    true,
  );
});

test('isStableCutMarker: other chore(release) messages rejected', () => {
  assert.equal(isStableCutMarker('chore(release): tweak wording'), false);
  assert.equal(isStableCutMarker('chore: publish new stable'), false);
});

test('isStableCutMarker: whitespace-trimmed', () => {
  // Docstring: "Case-sensitive on the marker itself". Trim behavior
  // is implicit via `subject.trim()` — pin so a future refactor that
  // drops the trim does not silently break real squash-merge subjects.
  assert.equal(isStableCutMarker('  chore(release): publish new stable\n'), true);
  assert.equal(isStableCutMarker('\tchore(release): publish new stable (#42)\t'), true);
});

test('isStableCutMarker: case-sensitivity on the marker literal', () => {
  // Docstring: "Case-sensitive on the marker itself so a random
  // `chore(release): tweak wording` does not reset the counter."
  assert.equal(isStableCutMarker('Chore(release): publish new stable'), false);
  assert.equal(isStableCutMarker('chore(Release): publish new stable'), false);
  assert.equal(isStableCutMarker('chore(release): Publish new stable'), false);
});

test('isStableCutMarker: PR suffix must be well-formed', () => {
  // A `(#` opener without a digit sequence must not match, and a `#`
  // without digits after the space likewise. Guards the `(#\d+)?`
  // arm from over-liberal matching.
  assert.equal(isStableCutMarker('chore(release): publish new stable (#)'), false);
  assert.equal(isStableCutMarker('chore(release): publish new stable (#abc)'), false);
  assert.equal(isStableCutMarker('chore(release): publish new stable (123)'), false);
  assert.equal(isStableCutMarker('chore(release): publish new stable#123'), false);
});

// ─── isDevTag ───────────────────────────────────────────────────
test('isDevTag: accepts vX.Y.Z-dev.N', () => {
  assert.equal(isDevTag('v6.6.0-dev.42'), true);
  assert.equal(isDevTag('v7.0.0-dev.1'), true);
});

test('isDevTag: rejects stable + rc tags', () => {
  assert.equal(isDevTag('v6.6.0'), false);
  assert.equal(isDevTag('v6.6.0-rc.1'), false);
  assert.equal(isDevTag('v6.6.0-dev.1.2'), false);
});

test('isDevTag: rejects malformed dev-adjacent shapes', () => {
  // Boundary cases that look almost right but must not match:
  //  - missing counter (`-dev`);
  //  - trailing separator (`-dev.`);
  //  - non-numeric counter (`-dev.rc`);
  //  - missing `v` prefix (`6.6.0-dev.1`);
  //  - extra prefix (`vv6.6.0-dev.1`);
  //  - leading whitespace (regex is anchored, no `.trim()`).
  assert.equal(isDevTag('v6.6.0-dev'), false);
  assert.equal(isDevTag('v6.6.0-dev.'), false);
  assert.equal(isDevTag('v6.6.0-dev.rc'), false);
  assert.equal(isDevTag('6.6.0-dev.1'), false);
  assert.equal(isDevTag('vv6.6.0-dev.1'), false);
  assert.equal(isDevTag(' v6.6.0-dev.1'), false);
  assert.equal(isDevTag(''), false);
});

test('isDevTag: accepts zero counter', () => {
  // The regex allows `\d+` including a leading zero — pin the boundary
  // so a future author does not narrow it accidentally.
  assert.equal(isDevTag('v6.6.0-dev.0'), true);
  assert.equal(isDevTag('v0.0.0-dev.1'), true);
  assert.equal(isDevTag('v10.20.30-dev.400'), true);
});

// ─── countCommitsSinceLastDevBoundary (git-fixture) ─────────────
// Builds a throw-away git repo, then chdir's into it so the module's
// bare `git` calls resolve to that repo. Verifies the two counter
// paths (`countCommitsSinceLastDevBoundary` + the `--range`-derived
// `Math.max(1, commits.length)`) agree on a boundary that lives at
// HEAD~1 — the fixture the docstring pins as the reference case.
test('countCommitsSinceLastDevBoundary: boundary at HEAD~1 → N=1 (agrees with --range path)', () => {
  const dir = mkdtempSync(join(tmpdir(), 'semver-boundary-'));
  const g = (cmd: string): string =>
    execSync(`git ${cmd}`, { cwd: dir, stdio: ['ignore', 'pipe', 'pipe'] })
      .toString()
      .trim();
  const cwd = process.cwd();
  try {
    g('init -q -b master');
    g('config user.email test@example');
    g('config user.name test');
    // Suppress developer-machine gpg/signing so `git tag` and
    // `git commit` do not prompt for a key inside the sandbox.
    g('config commit.gpgsign false');
    g('config tag.gpgsign false');
    g('commit -q --allow-empty -m "initial"');
    g('tag v0.0.0');
    g('commit -q --allow-empty -m "chore(release): publish new stable"');
    const markerSha = g('rev-parse HEAD');
    g('commit -q --allow-empty -m "feat(agent): post-marker one"');

    process.chdir(dir);
    // Boundary path: walk from stable tag until the marker at index 1
    // (HEAD~1). Excludes the boundary itself → N = 1.
    assert.equal(countCommitsSinceLastDevBoundary(), 1);
    // --range path: pass a range that STARTS at the marker (exclusive
    // per git's `A..B` semantics); one commit remains → N = 1.
    const rangeCommits = commitsInRange(markerSha, 'HEAD');
    assert.equal(Math.max(1, rangeCommits.length), 1);
  } finally {
    process.chdir(cwd);
    rmSync(dir, { recursive: true, force: true });
  }
});

test('countCommitsSinceLastDevBoundary: boundary at HEAD itself → N=1 (floor guard)', () => {
  // A stable-cut marker sitting on HEAD would return i = 0 without the
  // Math.max(1, i) floor; pin the guard so the next `-dev.N` for that
  // commit never collapses to `-dev.0`.
  const dir = mkdtempSync(join(tmpdir(), 'semver-boundary-head-'));
  const g = (cmd: string): string =>
    execSync(`git ${cmd}`, { cwd: dir, stdio: ['ignore', 'pipe', 'pipe'] })
      .toString()
      .trim();
  const cwd = process.cwd();
  try {
    g('init -q -b master');
    g('config user.email test@example');
    g('config user.name test');
    // Suppress developer-machine gpg/signing so `git tag` and
    // `git commit` do not prompt for a key inside the sandbox.
    g('config commit.gpgsign false');
    g('config tag.gpgsign false');
    g('commit -q --allow-empty -m "initial"');
    g('tag v0.0.0');
    g('commit -q --allow-empty -m "chore(release): publish new stable"');
    process.chdir(dir);
    assert.equal(countCommitsSinceLastDevBoundary(), 1);
  } finally {
    process.chdir(cwd);
    rmSync(dir, { recursive: true, force: true });
  }
});

test('countCommitsSinceLastDevBoundary: no boundary, three commits since stable → N=3', () => {
  // Fall-through arm: no marker, no dev tag; N equals the plain commit
  // count. Pins the trailing `Math.max(1, commits.length)`.
  const dir = mkdtempSync(join(tmpdir(), 'semver-noboundary-'));
  const g = (cmd: string): string =>
    execSync(`git ${cmd}`, { cwd: dir, stdio: ['ignore', 'pipe', 'pipe'] })
      .toString()
      .trim();
  const cwd = process.cwd();
  try {
    g('init -q -b master');
    g('config user.email test@example');
    g('config user.name test');
    // Suppress developer-machine gpg/signing so `git tag` and
    // `git commit` do not prompt for a key inside the sandbox.
    g('config commit.gpgsign false');
    g('config tag.gpgsign false');
    g('commit -q --allow-empty -m "initial"');
    g('tag v0.0.0');
    g('commit -q --allow-empty -m "feat(agent): one"');
    g('commit -q --allow-empty -m "fix(agent): two"');
    g('commit -q --allow-empty -m "docs(agent): three"');
    process.chdir(dir);
    assert.equal(countCommitsSinceLastDevBoundary(), 3);
  } finally {
    process.chdir(cwd);
    rmSync(dir, { recursive: true, force: true });
  }
});

process.stdout.write(`\n${passed} assertions passed.\n`);
