#!/usr/bin/env -S npx tsx
/**
 * Build one native-arch Docker image for the agent (linux/amd64 or
 * linux/arm64). The workflow calls this script twice — once per arch,
 * on a matching runner — and then `docker-push.ts` on each arch, and
 * finally the workflow's `docker-manifest` step merges the two per-arch
 * tags into a single multi-arch tag.
 *
 * The rationale for native builds (vs a single QEMU-emulated buildx
 * matrix) is wall-clock: on the current `ubuntu-24.04-arm` and
 * `ubuntu-latest` runners, native `dotnet publish` for the arm64 leg
 * runs in ~3 min against ~18 min under QEMU emulation.
 *
 * Usage:
 *   tsx tools/release/docker-build.ts --version <ver> --platform <linux/amd64|linux/arm64>
 *                                     [--image <name>] [--dry-run]
 *
 * The image name defaults to `trakhound/mtconnect-agent` and the tag
 * becomes `<image>:<version>-<arch-suffix>` where the suffix is
 * `amd64` / `arm64`. The multi-arch merge in `docker-manifest` then
 * points `<image>:<version>` at both.
 */

import { resolve } from 'node:path';
import { parseArgs } from 'node:util';
import { parseDryRun, run } from './shell.ts';

/** Repo root — used to resolve the Dockerfile and build context. */
const repoRoot = resolve(new URL('../../', import.meta.url).pathname);

/** Platform strings this script accepts. */
type Platform = 'linux/amd64' | 'linux/arm64';

/** Map platform to the arch-suffix used in the per-arch tag. */
export const archSuffixFor = (p: Platform): 'amd64' | 'arm64' => {
  return p === 'linux/amd64' ? 'amd64' : 'arm64';
};

/** CLI options. */
export type Options = {
  version: string;
  platform: Platform;
  image: string;
  dryRun: boolean;
};

/** Parse argv into strongly-typed `Options`. */
export const parseOptions = (argv: string[]): Options => {
  const { dryRun, rest } = parseDryRun(argv);
  const { values } = parseArgs({
    args: rest,
    options: {
      version: { type: 'string' },
      platform: { type: 'string' },
      image: { type: 'string' },
    },
  });
  if (!values.version) {
    throw new Error('--version is required');
  }
  if (values.platform !== 'linux/amd64' && values.platform !== 'linux/arm64') {
    throw new Error(
      `--platform must be linux/amd64 or linux/arm64, got "${values.platform ?? '<none>'}"`,
    );
  }
  return {
    version: values.version,
    platform: values.platform,
    image: values.image ?? 'trakhound/mtconnect-agent',
    dryRun,
  };
};

/** Build the image with `docker buildx build --load`, tagging it with
 *  the per-arch suffix. The manifest merge is `docker-push.ts`'s job. */
export const main = async (argv: string[]): Promise<void> => {
  const opts = parseOptions(argv);

  const arch = archSuffixFor(opts.platform);
  const tag = `${opts.image}:${opts.version}-${arch}`;
  const dockerfile = resolve(
    repoRoot,
    'build',
    'MTConnect.NET.Builder',
    'Parts',
    'agent',
    'docker',
    'Dockerfile',
  );

  const args = [
    'buildx',
    'build',
    '--platform',
    opts.platform,
    '--file',
    dockerfile,
    '--tag',
    tag,
    '--load',
    // NOTE: `--provenance` and `--sbom` are deliberately NOT passed
    // here. `buildx build --load` on the default `docker` driver
    // silently strips both flags (the docker-in-docker image loader
    // only knows how to import a single-platform image manifest, not
    // an image index). The downstream `sbom` job in `release.yml`
    // generates a fresh SPDX SBOM from the pushed image via
    // `sbom.ts`, so the metadata surface is preserved without paying
    // the buildx-strip warning noise on every run.
    repoRoot,
  ];
  await run('docker', args, { dryRun: opts.dryRun, cwd: repoRoot });
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
    process.stderr.write(`docker-build: ${(err as Error).message}\n`);
    process.exit(1);
  });
}
