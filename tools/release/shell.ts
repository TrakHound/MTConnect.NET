/**
 * Shared shell-out helper for the `tools/release/` scripts.
 *
 * Every release script eventually shells out to `dotnet`, `docker`, or
 * `gh`, streaming the child's stdout/stderr to the CI log so a
 * workflow-log tail is enough to diagnose a failed run. The helper
 * exists to (a) surface non-zero exits as thrown errors so `main()`
 * bodies stay linear and (b) support a repo-wide `--dry-run` flag that
 * echoes the command it would have run without executing anything.
 *
 * No script pipes command output through a variable — every subprocess
 * inherits stdio, so a long `dotnet pack` remains observable at
 * runtime instead of surfacing as a wall of text at the end.
 */

import { spawn } from 'node:child_process';

/** Whether to actually spawn subprocesses. When true, commands are
 *  logged but not executed — used by the "no push" verification runs
 *  the release scripts do on every branch. */
export type DryRun = boolean;

/**
 * Run one command with inherited stdio. Rejects with a descriptive
 * error when the child exits non-zero. Under `dryRun`, prints the
 * command it would have run and resolves immediately.
 *
 * @param cmd — executable name (resolved via PATH).
 * @param args — argv passed verbatim; no shell expansion.
 * @param opts — `dryRun` swaps the spawn for a stdout log line; `cwd`
 *   sets the working directory; `env` adds to `process.env`.
 */
export const run = async (
  cmd: string,
  args: string[],
  opts: { dryRun?: DryRun; cwd?: string; env?: NodeJS.ProcessEnv } = {},
): Promise<void> => {
  const rendered = renderCmd(cmd, args);
  if (opts.dryRun) {
    process.stdout.write(`[dry-run] ${rendered}\n`);
    return;
  }
  process.stdout.write(`+ ${rendered}\n`);
  await new Promise<void>((resolve, reject) => {
    const child = spawn(cmd, args, {
      stdio: 'inherit',
      cwd: opts.cwd,
      env: { ...process.env, ...opts.env },
    });
    child.on('error', reject);
    child.on('exit', (code, signal) => {
      if (code === 0) return resolve();
      const reason = signal ? `signal ${signal}` : `exit ${code}`;
      reject(new Error(`${rendered} failed (${reason})`));
    });
  });
};

/** Argument names whose IMMEDIATELY-FOLLOWING value is a credential.
 *  When any of these appears in the argv passed to `renderCmd`, the
 *  next arg is redacted in the printed log line only — the value that
 *  reaches the child process's argv is untouched. Applies to the
 *  `--name value` form; the `--name=value` form is redacted separately
 *  (the equals-form arg is a single token). */
export const SECRET_ARG_NAMES: ReadonlySet<string> = new Set([
  '--api-key',
  '--password',
  '-p',
  '--token',
]);

/** Render a command for logging — quoting any arg that contains
 *  whitespace or shell-special characters, and redacting the value
 *  after any `SECRET_ARG_NAMES` arg so credentials never surface in
 *  CI logs. Human-readable, not round-trip parseable. */
export const renderCmd = (cmd: string, args: string[]): string => {
  const rendered: string[] = [];
  let redactNext = false;
  for (const arg of args) {
    if (redactNext) {
      rendered.push('<redacted>');
      redactNext = false;
      continue;
    }
    // `--name=value` form — split at the first `=` and redact the RHS
    // whenever the LHS is a known secret arg name.
    const eqIdx = arg.indexOf('=');
    if (eqIdx > 0) {
      const lhs = arg.slice(0, eqIdx);
      if (SECRET_ARG_NAMES.has(lhs)) {
        rendered.push(`${lhs}=<redacted>`);
        continue;
      }
    }
    rendered.push(quoteForLog(arg));
    if (SECRET_ARG_NAMES.has(arg)) redactNext = true;
  }
  return [cmd, ...rendered].join(' ');
};

/** Wrap in double quotes if the arg contains anything shell would
 *  otherwise re-tokenise. Interior double quotes are backslash-escaped. */
const quoteForLog = (arg: string): string => {
  if (/^[A-Za-z0-9._\-/=:]+$/.test(arg)) return arg;
  return `"${arg.replace(/"/g, '\\"')}"`;
};

/** Parse a `--dry-run` flag out of an argv list, returning both the
 *  boolean and the remaining args. Kept trivial so the scripts do not
 *  need a dependency for one flag. */
export const parseDryRun = (argv: string[]): { dryRun: DryRun; rest: string[] } => {
  const rest: string[] = [];
  let dryRun = false;
  for (const arg of argv) {
    if (arg === '--dry-run') {
      dryRun = true;
    } else {
      rest.push(arg);
    }
  }
  return { dryRun, rest };
};

/** Look up a required environment variable. Throws when missing so a
 *  workflow step fails loudly instead of publishing an empty-versioned
 *  artifact. */
export const requireEnv = (name: string): string => {
  const v = process.env[name];
  if (!v || v.trim().length === 0) {
    throw new Error(`Environment variable ${name} is required but not set.`);
  }
  return v;
};

/** Look up an optional environment variable, returning `undefined`
 *  when unset. Present so call sites read symmetrically. */
export const optionalEnv = (name: string): string | undefined => {
  const v = process.env[name];
  return v && v.trim().length > 0 ? v : undefined;
};
