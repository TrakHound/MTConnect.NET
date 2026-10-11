#!/usr/bin/env -S npx tsx
/**
 * Compute the next dev pre-release version for a push-to-master build.
 *
 * The algorithm follows the shape approved on Discussion #175
 * (2026-08-16, Patrick):
 *
 *  1. Find the most recent tag matching `vX.Y.Z` (stable) — this is
 *     the base version. If none exists, seed at `v0.0.0`.
 *  2. Walk the commits reachable from `HEAD` back to that tag.
 *  3. Parse each commit's subject as a Conventional Commit and bump
 *     the base version accordingly:
 *       - a `!` in the type/scope, or a `BREAKING CHANGE:` footer,
 *         bumps the major segment;
 *       - a `feat(...)` commit bumps the minor segment;
 *       - any other conventional type bumps the patch segment;
 *       - non-conventional commits are ignored (they cannot appear
 *         under a green `pre-merge` gate, but the algorithm stays
 *         permissive so a partially-migrated history still resolves).
 *     The largest bump wins — a `BREAKING CHANGE` anywhere in the
 *     range beats every `feat` and `fix`. An all-`none` range still
 *     lands a patch bump via `applyBump`'s `none → patch` fall-through
 *     so the emitted version is distinct from the last stable tag
 *     (avoids a `-dev.N` collision on a chore-only range).
 *  4. Count the commits reachable from `HEAD` back to the most
 *     recent commit that either
 *       (a) is the stable-cut marker
 *           `chore(release): publish new stable`, or
 *       (b) carries an existing `vX.Y.Z-dev.N` tag.
 *     That count becomes the pre-release counter `N`. A commit
 *     matching either condition resets `N` to 1 for the next dev
 *     build. The counter is monotone within one stable-target cycle
 *     and never reused across cycles.
 *  5. Emit `<bumped-version>-dev.<N>` on stdout.
 *
 * When `--github-output` is passed, the script additionally appends
 * `version=<value>` to the file referenced by `$GITHUB_OUTPUT` so a
 * workflow step can consume it via `${{ steps.<id>.outputs.version }}`.
 *
 * When `--range <A>..<B>` is passed, the commit range is taken
 * verbatim instead of being derived from the most recent stable tag.
 * The pre-release counter is then the number of commits in the range,
 * floored at one so an empty range still produces a distinct dev
 * version (matches the tag-derived case when the range starts at the
 * stable cut). Used by the unit-test hook in `test.ts`.
 */

import { spawnSync } from 'node:child_process';
import { appendFileSync } from 'node:fs';
import { parseArgs } from 'node:util';
import * as semver from 'semver';

/** One parsed commit — the subject plus the highest bump kind it demands. */
export type BumpKind = 'major' | 'minor' | 'patch' | 'none';

/** Extract the highest-priority bump kind from a single commit subject +
 *  optional body. Conventional-commit rules:
 *   - `!` in the type/scope prefix (`feat!:`, `fix(scope)!:`) → major;
 *   - a `BREAKING CHANGE:` or `BREAKING-CHANGE:` footer/line → major;
 *   - subject starts with `feat` (case-insensitive) → minor;
 *   - subject starts with any other type (`fix|chore|docs|style|refactor|
 *     perf|test|build|ci|revert`) → patch;
 *   - anything else → none. */
export const bumpKindFor = (subject: string, body: string): BumpKind => {
  const s = subject.trim();
  // A `!` before the colon marks a breaking change per the Conventional
  // Commits spec — accepts `feat!:`, `fix(scope)!:`, `feat(scope)!:`.
  const bangMatch = /^([a-zA-Z]+)(\([^)]*\))?!:/.exec(s);
  if (bangMatch) return 'major';
  if (/(^|\n)BREAKING[ -]CHANGE:/i.test(body)) return 'major';

  const typeMatch = /^([a-zA-Z]+)(\([^)]*\))?:/.exec(s);
  if (!typeMatch) return 'none';
  const type = typeMatch[1]!.toLowerCase();
  if (type === 'feat') return 'minor';
  const known = new Set([
    'fix',
    'chore',
    'docs',
    'style',
    'refactor',
    'perf',
    'test',
    'build',
    'ci',
    'revert',
  ]);
  return known.has(type) ? 'patch' : 'none';
};

/** Pick the highest bump kind over a list of commits — major > minor > patch > none. */
export const aggregateBump = (kinds: BumpKind[]): BumpKind => {
  if (kinds.includes('major')) return 'major';
  if (kinds.includes('minor')) return 'minor';
  if (kinds.includes('patch')) return 'patch';
  return 'none';
};

/** Apply a bump kind to a base version. `none` still bumps patch so a
 *  chore-only range still produces a distinct pre-release version — the
 *  dev counter would collide otherwise. */
export const applyBump = (base: string, kind: BumpKind): string => {
  const clean = base.startsWith('v') ? base.slice(1) : base;
  const parsed = semver.parse(clean);
  if (!parsed) {
    throw new Error(`semver-bump: base "${base}" is not a valid semver`);
  }
  switch (kind) {
    case 'major':
      return semver.inc(clean, 'major')!;
    case 'minor':
      return semver.inc(clean, 'minor')!;
    case 'patch':
    case 'none':
      return semver.inc(clean, 'patch')!;
  }
};

/** Test whether a commit subject is the stable-cut marker. Accepts the
 *  exact string in either the subject line or as a squash-merge PR title
 *  suffix ("...(#123)"). Case-sensitive on the marker itself so a random
 *  `chore(release): tweak wording` does not reset the counter. */
export const isStableCutMarker = (subject: string): boolean => {
  return /^chore\(release\): publish new stable( \(#\d+\))?$/.test(subject.trim());
};

/** Test whether a tag looks like an existing dev pre-release tag
 *  (`vX.Y.Z-dev.N`). Used to detect the last dev-cut boundary when the
 *  most recent commit is neither the stable-cut marker nor tagged as
 *  stable. */
export const isDevTag = (tag: string): boolean => {
  return /^v\d+\.\d+\.\d+-dev\.\d+$/.test(tag);
};

/** Run `git` with args and return stdout. Throws on non-zero exit so the
 *  workflow fails loudly rather than emitting a bogus version. */
const git = (args: string[]): string => {
  const r = spawnSync('git', args, { encoding: 'utf8' });
  if (r.status !== 0) {
    throw new Error(
      `git ${args.join(' ')} failed (exit ${r.status}): ${r.stderr.trim() || r.stdout.trim()}`,
    );
  }
  return r.stdout;
};

/** Return the most recent stable tag reachable from HEAD, or `v0.0.0`
 *  when the repo has never been tagged with a bare `vX.Y.Z`. Uses
 *  `git describe` to walk parent history. The match/exclude pair is
 *  deliberately conservative:
 *
 *   - `--match 'v[0-9]*.[0-9]*.[0-9]*'` — start narrow, at least
 *     three dot-separated numeric-lead segments prefixed with `v`;
 *   - `--exclude 'v*-*'` — reject anything with a hyphen suffix
 *     (`-dev.N`, `-rc.1`, `-beta-agents`, `-prerelease`, …). Only a
 *     pure numeric `vX.Y.Z` tag survives.
 *
 *  Falls back to `v0.0.0` on the sentinel "no names found" error
 *  rather than propagating it. */
export const lastStableTag = (): string => {
  const r = spawnSync(
    'git',
    [
      'describe',
      '--tags',
      '--abbrev=0',
      '--match',
      'v[0-9]*.[0-9]*.[0-9]*',
      '--exclude',
      'v*-*',
    ],
    { encoding: 'utf8' },
  );
  if (r.status !== 0) {
    if (/No names found/i.test(r.stderr)) return 'v0.0.0';
    throw new Error(`git describe failed: ${r.stderr.trim()}`);
  }
  return r.stdout.trim();
};

/** Return the list of commits in `<from>..HEAD` as an array of
 *  {sha, subject, body}. Uses `%x1e` (record separator) between commits
 *  and `%x1f` (unit separator) between fields to survive bodies with
 *  arbitrary whitespace and quoted content. */
export const commitsInRange = (
  from: string,
  to: string = 'HEAD',
): Array<{ sha: string; subject: string; body: string }> => {
  // Empty output when the range is empty (from == to) — handle gracefully.
  const raw = git(['log', `${from}..${to}`, '--pretty=format:%H%x1f%s%x1f%b%x1e']);
  if (!raw.trim()) return [];
  return raw
    .split('\x1e')
    .map((rec) => rec.trim())
    .filter((rec) => rec.length > 0)
    .map((rec) => {
      const [sha, subject, body] = rec.split('\x1f');
      return { sha: sha ?? '', subject: subject ?? '', body: body ?? '' };
    });
};

/** Count how many commits reachable from HEAD (walking parents) come
 *  BEFORE we hit either the stable-cut marker or a `vX.Y.Z-dev.N`
 *  tag. Returns the count of commits that are still on the "current"
 *  dev cycle, i.e. the `N` for the next `<version>-dev.<N>`. The
 *  boundary commit itself is excluded — it belongs to the prior
 *  cycle — and the counter is floored at one so a boundary sitting on
 *  HEAD still produces a distinct dev version. */
export const countCommitsSinceLastDevBoundary = (): number => {
  // Fastest path: if HEAD or an ancestor is tagged with a stable version,
  // walk the range and stop at the first stable-cut marker or dev tag.
  const stable = lastStableTag();
  const commits = commitsInRange(stable);

  // `commits` is ordered newest-first (git log default). Index `i` is
  // the number of commits between HEAD and `commits[i]` exclusive —
  // exactly the counter value once we exclude the boundary itself.
  for (let i = 0; i < commits.length; i++) {
    const c = commits[i]!;
    if (isStableCutMarker(c.subject)) {
      return Math.max(1, i);
    }
    // Any dev tag on this commit ends the current cycle at index `i`.
    const tagsRaw = spawnSync(
      'git',
      ['tag', '--points-at', c.sha, '--list', 'v*-dev.*'],
      { encoding: 'utf8' },
    );
    if (tagsRaw.status === 0) {
      const tags = tagsRaw.stdout.split('\n').map((t) => t.trim()).filter((t) => isDevTag(t));
      if (tags.length > 0) return Math.max(1, i);
    }
  }
  // No stable-cut marker and no dev-tag boundary found — the count
  // includes every commit since the last stable release.
  return Math.max(1, commits.length);
};

/**
 * Top-level entry: compute the next dev pre-release version and print it.
 * Accepts `--github-output` to also append to the GH Actions output file
 * and `--range <A>..<B>` to short-circuit the tag lookup (used by tests).
 */
export const main = (argv: string[]): void => {
  const { values } = parseArgs({
    args: argv,
    options: {
      'github-output': { type: 'boolean', default: false },
      range: { type: 'string' },
    },
  });

  let base: string;
  let commits: Array<{ sha: string; subject: string; body: string }>;
  let counter: number;

  if (values.range) {
    const [from, toRaw] = values.range.split('..');
    const to = toRaw && toRaw.length > 0 ? toRaw : 'HEAD';
    if (!from) {
      throw new Error(`--range must be of the form <from>..<to>, got "${values.range}"`);
    }
    // Extract the base version from the `from` ref when it looks like a
    // stable tag; otherwise fall back to the most recent stable tag.
    base = /^v\d+\.\d+\.\d+$/.test(from) ? from : lastStableTag();
    commits = commitsInRange(from, to);
    counter = Math.max(1, commits.length);
  } else {
    base = lastStableTag();
    commits = commitsInRange(base);
    counter = countCommitsSinceLastDevBoundary();
  }

  const kind = aggregateBump(commits.map((c) => bumpKindFor(c.subject, c.body)));
  const bumped = applyBump(base, kind);
  const version = `${bumped}-dev.${counter}`;

  // Print to stdout in a shape a workflow step can capture directly.
  process.stdout.write(`${version}\n`);

  if (values['github-output'] && process.env.GITHUB_OUTPUT) {
    appendFileSync(process.env.GITHUB_OUTPUT, `version=${version}\n`);
    appendFileSync(process.env.GITHUB_OUTPUT, `base=${base}\n`);
    appendFileSync(process.env.GITHUB_OUTPUT, `bump=${kind}\n`);
    appendFileSync(process.env.GITHUB_OUTPUT, `counter=${counter}\n`);
  }
};

// ESM-safe "run when invoked directly" — no `require.main` in ES modules.
// Compares the script URL to the process entrypoint; skips when imported.
const invokedDirectly = (() => {
  const entry = process.argv[1];
  if (!entry) return false;
  try {
    return new URL(`file://${entry}`).href === import.meta.url;
  } catch {
    return false;
  }
})();

if (invokedDirectly) {
  try {
    main(process.argv.slice(2));
  } catch (err) {
    process.stderr.write(`semver-bump: ${(err as Error).message}\n`);
    process.exit(1);
  }
}
