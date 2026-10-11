#!/usr/bin/env -S npx tsx
/**
 * Generate SPDX SBOMs for the release artefacts.
 *
 * Two flavors in one script:
 *   1. `--nuget` — invokes `dotnet sbom-tool generate` against the
 *      built .nupkg output so each package ships an in-tree SBOM
 *      alongside its manifest. The tool is expected to be installed
 *      as a global dotnet tool (`dotnet tool install --global
 *      Microsoft.Sbom.DotNetTool`) before this script runs; the
 *      release workflow's `sbom` job does that in a preceding step.
 *   2. `--docker <image:tag>` — invokes `syft <image:tag> -o
 *      spdx-json=<output>/…`. Syft is the SBOM engine that Anchore
 *      ship inside the `anchore/sbom-action` GitHub Action the
 *      workflow uses for its CI path — running syft directly from
 *      this script keeps the local `--dry-run` output shape aligned
 *      with what CI produces. `docker scout sbom` was the previous
 *      backend and is no longer used: it required a `docker scout`
 *      install on the runner and produced a materially different
 *      SPDX shape from the anchore/syft baseline.
 *
 * Usage:
 *   tsx tools/release/sbom.ts --nuget [--input <dir>] [--output <dir>] [--dry-run]
 *   tsx tools/release/sbom.ts --docker <image:tag> [--output <dir>] [--dry-run]
 */

import { existsSync, mkdirSync } from 'node:fs';
import { resolve } from 'node:path';
import { parseArgs } from 'node:util';
import { parseDryRun, run } from './shell.ts';

/** Repo root — used for default input/output paths. */
const repoRoot = resolve(new URL('../../', import.meta.url).pathname);

/** CLI options — `nuget` and `docker` are mutually exclusive top-level
 *  modes. Exactly one must be provided. */
export type Options = {
  mode: 'nuget' | 'docker';
  input: string;
  output: string;
  dockerImage: string | undefined;
  dryRun: boolean;
};

/** Parse argv into strongly-typed `Options`. */
export const parseOptions = (argv: string[]): Options => {
  const { dryRun, rest } = parseDryRun(argv);
  const { values } = parseArgs({
    args: rest,
    options: {
      nuget: { type: 'boolean', default: false },
      docker: { type: 'string' },
      input: { type: 'string' },
      output: { type: 'string' },
    },
  });
  if (!values.nuget && !values.docker) {
    throw new Error('Either --nuget or --docker <image:tag> is required');
  }
  if (values.nuget && values.docker) {
    throw new Error('--nuget and --docker are mutually exclusive');
  }
  return {
    mode: values.nuget ? 'nuget' : 'docker',
    input: values.input ?? resolve(repoRoot, 'build', 'output', 'nupkg'),
    output: values.output ?? resolve(repoRoot, 'build', 'output', 'sbom'),
    dockerImage: values.docker,
    dryRun,
  };
};

/** SBOM generation dispatch. Delegates to the tool best suited to each
 *  artefact class; both branches write into `<output>/`. */
export const main = async (argv: string[]): Promise<void> => {
  const opts = parseOptions(argv);
  if (!opts.dryRun && !existsSync(opts.output)) {
    mkdirSync(opts.output, { recursive: true });
  }

  if (opts.mode === 'nuget') {
    // `sbom-tool generate` scans the whole build output and emits one
    // manifest describing every .nupkg + its transitive dependency
    // graph (as reported by NuGet's project.assets.json).
    const args = [
      'sbom-tool',
      'generate',
      '-b',
      opts.input,
      '-bc',
      repoRoot,
      '-pn',
      'MTConnect.NET',
      '-ps',
      'TrakHound Inc.',
      '-nsb',
      'https://github.com/TrakHound/MTConnect.NET',
      '-m',
      opts.output,
    ];
    await run('dotnet', args, { dryRun: opts.dryRun });
    return;
  }

  // Docker mode — the image should already be present locally (built
  // by `docker-build.ts` and pulled by the workflow). Syft scans the
  // image layers and writes SPDX-JSON straight to disk. Same engine
  // as `anchore/sbom-action` so local + CI outputs agree.
  if (!opts.dockerImage) throw new Error('--docker image tag is required in docker mode');
  const slug = opts.dockerImage.replace(/[^A-Za-z0-9._-]/g, '_');
  const outFile = resolve(opts.output, `${slug}.spdx.json`);
  const args = [opts.dockerImage, '-o', `spdx-json=${outFile}`];
  await run('syft', args, { dryRun: opts.dryRun });
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
    process.stderr.write(`sbom: ${(err as Error).message}\n`);
    process.exit(1);
  });
}
