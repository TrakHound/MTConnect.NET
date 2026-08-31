#!/usr/bin/env -S npx tsx
/**
 * `dotnet pack` every shipped library into `<repo>/build/output/nupkg/`.
 *
 * Ported from `build/MTConnect.NET.Builder/Parts/libraries/Nuget.cs`
 * (which is retained for the manual stable-release workflow). Runs
 * `dotnet pack MTConnect.NET.sln` once with `-c Release`, letting the
 * .sln filter select every project that opts in via `IsPackable=true`
 * in its `.csproj`. Emits a stable output directory the downstream
 * `nuget-push.ts` and `gh-release-create.ts` scripts consume.
 *
 * Usage:
 *   tsx tools/release/pack.ts --version <ver> [--output <dir>] [--dry-run]
 *
 * When `--dry-run` is passed, the underlying `dotnet` invocation is
 * echoed but not executed — used by the tools' verification pass.
 */

import { mkdirSync, existsSync, rmSync } from 'node:fs';
import { resolve } from 'node:path';
import { parseArgs } from 'node:util';
import { parseDryRun, run } from './shell.ts';

/** Repo root — computed once, used to resolve the .sln and default
 *  output directory. The scripts are always launched from the repo
 *  root by the workflows; the calculation stays honest either way. */
const repoRoot = resolve(new URL('../../', import.meta.url).pathname);

/** CLI options parsed via `node:util.parseArgs`. `output` defaults to
 *  `<repo>/build/output/nupkg` so per-version outputs are colocated
 *  with the existing Builder layout. */
export type Options = {
  version: string;
  output: string;
  dryRun: boolean;
};

/** Parse argv into strongly-typed `Options`. Fails fast on missing
 *  `--version`; no other flag is required. */
export const parseOptions = (argv: string[]): Options => {
  const { dryRun, rest } = parseDryRun(argv);
  const { values } = parseArgs({
    args: rest,
    options: {
      version: { type: 'string' },
      output: { type: 'string' },
    },
  });
  if (!values.version) {
    throw new Error('--version is required (e.g. --version 7.0.0-dev.42)');
  }
  return {
    version: values.version,
    output: values.output ?? resolve(repoRoot, 'build', 'output', 'nupkg'),
    dryRun,
  };
};

/** Entry point — packs every packable project in the solution into
 *  the target output directory. */
export const main = async (argv: string[]): Promise<void> => {
  const opts = parseOptions(argv);

  // Reset the output dir so a repeated invocation does not accumulate
  // stale .nupkg files from an earlier build.
  if (existsSync(opts.output) && !opts.dryRun) {
    rmSync(opts.output, { recursive: true, force: true });
  }
  if (!opts.dryRun) mkdirSync(opts.output, { recursive: true });

  const slnPath = resolve(repoRoot, 'MTConnect.NET.sln');

  const args = [
    'pack',
    slnPath,
    '-c',
    'Release',
    '--nologo',
    `-p:PackageVersion=${opts.version}`,
    '-p:IncludeSymbols=true',
    '-p:SymbolPackageFormat=snupkg',
    '-p:ContinuousIntegrationBuild=true',
    '-p:Deterministic=true',
    '-p:EmbedUntrackedSources=true',
    '--output',
    opts.output,
  ];
  await run('dotnet', args, { dryRun: opts.dryRun, cwd: repoRoot });
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
    process.stderr.write(`pack: ${(err as Error).message}\n`);
    process.exit(1);
  });
}
