#!/usr/bin/env -S npx tsx
/**
 * Discover and run every `*.test.ts` file under `tools/`. Each test
 * file is spawned as its own `tsx` process so its top-level state does
 * not leak into siblings (`process.env` mutations, `process.stdout`
 * patches, etc.). Exit code is 0 iff every file exits 0.
 *
 * Run with:
 *   npm test              (via the `test` script in package.json)
 *   npx tsx tools/run-tests.ts
 *
 * Kept small and dependency-free — no runner (Jest/Vitest/node:test)
 * because the in-house `test(name, fn)` shim in each `*.test.ts` file
 * paid the same cost with zero extra config to maintain.
 */

import { spawn } from 'node:child_process';
import { readdirSync, statSync } from 'node:fs';
import { resolve } from 'node:path';

const here = resolve(new URL('.', import.meta.url).pathname);

/** Recursive walk that yields absolute paths of every file ending in
 *  `.test.ts` under `dir`. `node_modules` is skipped. */
const walk = (dir: string): string[] => {
  const out: string[] = [];
  for (const name of readdirSync(dir)) {
    if (name === 'node_modules' || name.startsWith('.')) continue;
    const path = resolve(dir, name);
    const st = statSync(path);
    if (st.isDirectory()) {
      out.push(...walk(path));
    } else if (name.endsWith('.test.ts')) {
      out.push(path);
    }
  }
  return out;
};

const files = walk(here).sort();
if (files.length === 0) {
  process.stdout.write('no *.test.ts files found under tools/\n');
  process.exit(0);
}

const runOne = (file: string): Promise<number> =>
  new Promise((resolvePromise) => {
    const rel = file.slice(here.length + 1);
    process.stdout.write(`\n── ${rel} ──────────────────────────────────\n`);
    const child = spawn('npx', ['tsx', file], { stdio: 'inherit' });
    child.on('exit', (code) => resolvePromise(code ?? 1));
    child.on('error', (err) => {
      process.stderr.write(`spawn error for ${rel}: ${err.message}\n`);
      resolvePromise(1);
    });
  });

const main = async (): Promise<void> => {
  let failed = 0;
  for (const f of files) {
    const code = await runOne(f);
    if (code !== 0) failed += 1;
  }
  process.stdout.write(
    `\n── summary ────────────────────────────────────\n` +
      `${files.length} test file(s), ${failed} failure(s)\n`,
  );
  process.exit(failed === 0 ? 0 : 1);
};

main();
