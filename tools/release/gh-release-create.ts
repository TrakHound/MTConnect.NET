#!/usr/bin/env -S npx tsx
/**
 * Create a GitHub pre-release for a `<version>-dev.<N>` cut and attach
 * the SBOMs + .nupkg files that the earlier pipeline steps produced.
 *
 * Uses the `gh` CLI rather than `@octokit/rest` for the release
 * creation itself — `gh release create` handles the multipart asset
 * upload semantics without any of the retry / MIME plumbing an
 * Octokit-side implementation would need. Octokit remains available
 * to `tools/` scripts that need the fine-grained REST surface (see
 * `tools/package.json`), just not this one.
 *
 * Usage:
 *   tsx tools/release/gh-release-create.ts --version <ver>
 *                                          [--repo <owner/name>]
 *                                          [--assets <dir> ...]
 *                                          [--docker-image <ref>]
 *                                          [--target <sha>]
 *                                          [--dry-run]
 *
 * `--target <sha>` pins the tag to the commit that produced the
 * artefacts. Omitting it lets `gh release create` pin the tag to the
 * tip of the default branch, which drifts under concurrent pushes.
 *
 * The release is always created as `--prerelease` (Phase 1 automates
 * only the dev pre-release cadence; stable releases stay under the
 * existing MTConnect.NET.Builder flow until a follow-up wires them
 * in). Release notes list every attached asset plus the docker image
 * reference so consumers have a single machine-readable manifest of
 * the cut.
 */

import { readdirSync, existsSync, writeFileSync, mkdirSync } from 'node:fs';
import { resolve } from 'node:path';
import { parseArgs } from 'node:util';
import { parseDryRun, run } from './shell.ts';

/** Repo root — used for default asset directories. */
const repoRoot = resolve(new URL('../../', import.meta.url).pathname);

/** CLI options. */
export type Options = {
  version: string;
  repo: string;
  assetDirs: string[];
  dockerImage: string | undefined;
  target: string | undefined;
  dryRun: boolean;
};

/** Parse argv into strongly-typed `Options`. Multiple `--assets` flags
 *  accumulate into a list; missing dirs are skipped with a warning
 *  (a workflow may pass both `nupkg/` and `sbom/` even when only one
 *  step ran). */
export const parseOptions = (argv: string[]): Options => {
  const { dryRun, rest } = parseDryRun(argv);
  const { values } = parseArgs({
    args: rest,
    options: {
      version: { type: 'string' },
      repo: { type: 'string' },
      assets: { type: 'string', multiple: true },
      'docker-image': { type: 'string' },
      target: { type: 'string' },
    },
  });
  if (!values.version) {
    throw new Error('--version is required');
  }
  const assetDirs = values.assets ?? [
    resolve(repoRoot, 'build', 'output', 'nupkg'),
    resolve(repoRoot, 'build', 'output', 'sbom'),
  ];
  return {
    version: values.version,
    repo: values.repo ?? 'TrakHound/MTConnect.NET',
    assetDirs,
    dockerImage: values['docker-image'],
    target: values.target,
    dryRun,
  };
};

/** Enumerate assets across the requested directories, returning
 *  absolute paths. Recurses so nested SBOM layouts such as
 *  `sbom/_manifest/spdx_2.2/manifest.spdx.json` (the shape
 *  `Microsoft.Sbom.DotNetTool` emits) are picked up alongside
 *  flat `nupkg/*.nupkg` files. Dotfiles are skipped at every level.
 *  Directories themselves are not attached — only files. */
export const collectAssets = (dirs: string[]): string[] => {
  const files: string[] = [];
  for (const dir of dirs) {
    if (!existsSync(dir)) {
      process.stderr.write(`[gh-release-create] skipping missing dir: ${dir}\n`);
      continue;
    }
    // `readdirSync(dir, { recursive: true, withFileTypes: true })`
    // returns `Dirent`s whose `parentPath` is the absolute directory
    // containing the entry. Filter to regular files (skip directories
    // and symlinks) and drop anything under a dotfile / dot-directory
    // path segment.
    for (const entry of readdirSync(dir, { recursive: true, withFileTypes: true })) {
      if (!entry.isFile()) continue;
      const parent = (entry as unknown as { parentPath?: string; path?: string }).parentPath
        ?? (entry as unknown as { path?: string }).path
        ?? dir;
      const rel = resolve(parent, entry.name).slice(dir.length + 1);
      if (rel.split('/').some((seg) => seg.startsWith('.'))) continue;
      files.push(resolve(parent, entry.name));
    }
  }
  return files;
};

/** Build the release-notes body — a short header plus a
 *  human-readable manifest of every attached asset and the docker
 *  image reference. Kept plain-markdown so the GitHub release page
 *  renders it without extension conversions. */
export const renderReleaseNotes = (
  version: string,
  assets: string[],
  dockerImage: string | undefined,
): string => {
  const lines: string[] = [];
  lines.push(`# MTConnect.NET ${version}`);
  lines.push('');
  lines.push(
    'Automated dev pre-release cut by the CI release pipeline. Not intended for production use — the stable-release cadence still runs through the manual `MTConnect.NET.Builder` flow.',
  );
  lines.push('');
  lines.push('## Assets');
  lines.push('');
  if (assets.length === 0) {
    lines.push('_No assets attached._');
  } else {
    for (const a of assets) {
      lines.push(`- \`${a.split('/').pop()}\``);
    }
  }
  lines.push('');
  if (dockerImage) {
    lines.push('## Docker image');
    lines.push('');
    lines.push('```');
    lines.push(`docker pull ${dockerImage}`);
    lines.push('```');
    lines.push('');
  }
  return lines.join('\n');
};

/** Entry point — write the notes file, then invoke `gh release
 *  create`. Idempotent: if a release + tag for `v<version>` already
 *  exist (a re-run of the workflow on the same SHA), the prior
 *  release + tag are deleted and re-cut so the assets attached match
 *  the current run. */
export const main = async (argv: string[]): Promise<void> => {
  const opts = parseOptions(argv);

  const assets = collectAssets(opts.assetDirs);
  const notes = renderReleaseNotes(opts.version, assets, opts.dockerImage);

  const notesDir = resolve(repoRoot, 'build', 'output', 'release-notes');
  const notesFile = resolve(notesDir, `${opts.version}.md`);
  if (!opts.dryRun) {
    mkdirSync(notesDir, { recursive: true });
    writeFileSync(notesFile, notes, 'utf8');
  }

  const tag = `v${opts.version}`;

  // Idempotency guard — `gh release create` errors when the tag or
  // release already exists. Silently swallow the "no such release"
  // return by checking existence first, then delete both the release
  // and the underlying tag so the fresh `create` below starts clean.
  await run('sh', [
    '-c',
    `gh release view ${tag} --repo ${opts.repo} >/dev/null 2>&1 && ` +
      `gh release delete ${tag} --repo ${opts.repo} --yes --cleanup-tag || true`,
  ], { dryRun: opts.dryRun, cwd: repoRoot });

  const args = [
    'release',
    'create',
    tag,
    '--repo',
    opts.repo,
    '--title',
    `MTConnect.NET ${opts.version}`,
    '--notes-file',
    notesFile,
    '--prerelease',
  ];
  if (opts.target) {
    args.push('--target', opts.target);
  }
  args.push(...assets);
  await run('gh', args, { dryRun: opts.dryRun, cwd: repoRoot });
};

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
  main(process.argv.slice(2)).catch((err) => {
    process.stderr.write(`gh-release-create: ${(err as Error).message}\n`);
    process.exit(1);
  });
}
