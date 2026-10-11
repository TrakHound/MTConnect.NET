#!/usr/bin/env -S npx tsx
/**
 * Push one per-arch Docker tag to Docker Hub. Called from the release
 * workflow's `docker-amd64` and `docker-arm64` jobs after
 * `docker-build.ts` has produced the tagged image on the runner.
 *
 * Optionally merges the per-arch tags into a single multi-arch tag
 * via `docker buildx imagetools create` when `--manifest` is passed
 * without `--platform`; that mode is what the `docker-manifest` job
 * runs after both per-arch pushes complete.
 *
 * Usage (per-arch push):
 *   tsx tools/release/docker-push.ts --version <ver>
 *                                    --platform <linux/amd64|linux/arm64>
 *                                    [--image <name>] [--dry-run]
 *
 * Usage (manifest merge):
 *   tsx tools/release/docker-push.ts --version <ver> --manifest
 *                                    [--image <name>] [--dry-run]
 *
 * Docker Hub credentials come from `DOCKERHUB_USERNAME` +
 * `DOCKERHUB_TOKEN` — a `docker login` in the workflow step precedes
 * this script (via `docker/login-action`); the script itself does not
 * touch the credentials.
 */

import { parseArgs } from 'node:util';
import { parseDryRun, run } from './shell.ts';

/** Platform strings this script accepts. */
type Platform = 'linux/amd64' | 'linux/arm64';

/** Map platform to the arch-suffix used in the per-arch tag. */
export const archSuffixFor = (p: Platform): 'amd64' | 'arm64' => {
  return p === 'linux/amd64' ? 'amd64' : 'arm64';
};

/** CLI options. */
export type Options = {
  version: string;
  platform: Platform | undefined;
  manifest: boolean;
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
      manifest: { type: 'boolean', default: false },
      image: { type: 'string' },
    },
  });
  if (!values.version) {
    throw new Error('--version is required');
  }
  const platform = values.platform;
  if (platform && platform !== 'linux/amd64' && platform !== 'linux/arm64') {
    throw new Error(
      `--platform must be linux/amd64 or linux/arm64, got "${platform}"`,
    );
  }
  if (!platform && !values.manifest) {
    throw new Error('Either --platform or --manifest must be provided');
  }
  if (platform && values.manifest) {
    throw new Error('--platform and --manifest are mutually exclusive');
  }
  return {
    version: values.version,
    platform: platform as Platform | undefined,
    manifest: values.manifest,
    image: values.image ?? 'trakhound/mtconnect-agent',
    dryRun,
  };
};

/** Push the per-arch tag, or merge both into a single multi-arch tag. */
export const main = async (argv: string[]): Promise<void> => {
  const opts = parseOptions(argv);

  if (opts.manifest) {
    // Multi-arch merge — reference both per-arch tags and publish a
    // single tag that clients resolve to the right arch automatically.
    const target = `${opts.image}:${opts.version}`;
    const amd64 = `${opts.image}:${opts.version}-amd64`;
    const arm64 = `${opts.image}:${opts.version}-arm64`;
    await run('docker', ['buildx', 'imagetools', 'create', '--tag', target, amd64, arm64], {
      dryRun: opts.dryRun,
    });
    return;
  }

  const arch = archSuffixFor(opts.platform!);
  const tag = `${opts.image}:${opts.version}-${arch}`;
  await run('docker', ['push', tag], { dryRun: opts.dryRun });
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
    process.stderr.write(`docker-push: ${(err as Error).message}\n`);
    process.exit(1);
  });
}
