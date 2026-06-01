#!/usr/bin/env node

// Walk every Markdown file under <docs-root> (default: '.'), derive the
// VitePress route for each, then navigate to it in a headless Chromium
// browser and fail on any client-side 404.
//
// Complements check-broken-links.mjs (which validates internal link targets
// exist on disk). This script validates that VitePress actually registers and
// renders each route at runtime—a gap the filesystem check cannot catch.
//
// The script spawns `vitepress preview` against docs/.vitepress/dist/ (the
// already-built site artifact), waits for the server to be ready, then
// crawls every route with Playwright's chromium browser. VitePress's
// client-side 404 page is detected via the `.NotFound` CSS class, a
// document.title of '404', or body text containing 'PAGE NOT FOUND'.
//
// External (http/https) URLs are not validated by design: third-party state
// is not the CI gate.
//
// Usage (run from the docs/ directory):
//   node scripts/check-routes.mjs
//
// Environment:
//   DOCS_BASE_PATH  URL path prefix (default: ''). Set to '/MTConnect.NET'
//                   when checking the GitHub Pages deploy base.

import { readdir } from 'node:fs/promises';
import { createServer as createTcpServer, Socket } from 'node:net';
import { resolve, sep } from 'node:path';
import { spawn } from 'node:child_process';
import { chromium } from 'playwright';
import pLimit from 'p-limit';
import treeKill from 'tree-kill';

// ─── Configuration ───────────────────────────────────────────────────────────

const DOCS_ROOT = resolve('.');
const DIST_DIR = resolve(DOCS_ROOT, '.vitepress/dist');
const CONCURRENCY = 8;
const PROGRESS_EVERY = 25;
const SERVER_READY_POLL_MS = 200;
const SERVER_READY_TIMEOUT_MS = 30_000;

const BASE_PATH = (process.env.DOCS_BASE_PATH ?? '').replace(/\/$/, '');

// ─── Route derivation ────────────────────────────────────────────────────────

// Directories skipped during the Markdown walk. Skip dot-prefixed dirs
// wholesale (.vitepress includes the build cache and the dist tree).
const isSkippedDir = (name) => name === 'node_modules' || name.startsWith('.');

const collectMarkdownFiles = async (dir) => {
    const entries = await readdir(dir, { withFileTypes: true });
    const results = [];
    for (const entry of entries) {
        const fullPath = resolve(dir, entry.name);
        if (entry.isSymbolicLink()) continue;
        if (entry.isDirectory()) {
            if (isSkippedDir(entry.name)) continue;
            results.push(...(await collectMarkdownFiles(fullPath)));
        } else if (entry.isFile() && entry.name.endsWith('.md')) {
            results.push(fullPath);
        }
    }
    return results;
};

// Convert an absolute .md file path to the VitePress route it maps to.
//
//   docs/index.md              -> /
//   docs/getting-started.md    -> /getting-started
//   docs/reference/index.md    -> /reference/
//   docs/reference/cli.md      -> /reference/cli
//
// VitePress cleanUrls strips the .html extension, so both /foo and /foo/
// work for regular pages; directory-level index pages reliably use the
// trailing-slash form which avoids the slug-vs-cleanUrls ambiguity that
// motivated this checker in the first place.
const mdFileToRoute = (absPath) => {
    const rel = absPath.slice(DOCS_ROOT.length).replaceAll(sep, '/');
    if (rel === '/index.md') return BASE_PATH + '/';
    if (rel.endsWith('/index.md')) return BASE_PATH + rel.slice(0, -'index.md'.length);
    return BASE_PATH + rel.slice(0, -'.md'.length);
};

// ─── Free-port finder ────────────────────────────────────────────────────────

const findFreePort = () =>
    new Promise((resolve, reject) => {
        const srv = createTcpServer();
        srv.listen(0, '127.0.0.1', () => {
            const { port } = srv.address();
            srv.close(() => resolve(port));
        });
        srv.on('error', reject);
    });

// ─── Preview server lifecycle ────────────────────────────────────────────────

// Poll TCP until the port accepts connections or the deadline passes.
const waitForServer = (port) =>
    new Promise((resolve, reject) => {
        const deadline = Date.now() + SERVER_READY_TIMEOUT_MS;

        const probe = () => {
            const sock = new Socket();
            sock
                .connect(port, '127.0.0.1', () => {
                    sock.destroy();
                    resolve();
                })
                .on('error', () => {
                    sock.destroy();
                    if (Date.now() >= deadline) {
                        reject(new Error(`Preview server did not start within ${SERVER_READY_TIMEOUT_MS / 1000}s`));
                    } else {
                        setTimeout(probe, SERVER_READY_POLL_MS);
                    }
                });
        };

        probe();
    });

const startPreviewServer = async (port) => {
    const proc = spawn(
        'npx',
        ['vitepress', 'preview', '--port', String(port), '--outDir', DIST_DIR],
        {
            cwd: DOCS_ROOT,
            stdio: ['ignore', 'pipe', 'pipe'],
            detached: false,
        },
    );
    // Drain stdout/stderr so the child process doesn't block on a full pipe.
    proc.stdout.resume();
    proc.stderr.resume();
    proc.on('error', (err) => {
        throw new Error(`Failed to spawn vitepress preview: ${err.message}`);
    });
    await waitForServer(port);
    return proc;
};

const stopPreviewServer = (proc) =>
    new Promise((done) => {
        if (!proc || proc.exitCode !== null) {
            done();
            return;
        }
        treeKill(proc.pid, 'SIGTERM', () => done());
    });

// ─── Route crawl ─────────────────────────────────────────────────────────────

// Detect the VitePress 404 page. VitePress renders a <div class="NotFound">
// wrapper, sets document.title to '404', and includes an h1 whose text
// contains 'PAGE NOT FOUND'. Any one signal is enough to declare a miss.
const is404Page = (page) =>
    page.evaluate(() => {
        const hasClass = !!document.querySelector('.NotFound');
        const title404 = document.title === '404';
        const body = (document.body?.innerText ?? '').toUpperCase();
        return hasClass || title404 || body.includes('PAGE NOT FOUND');
    });

const checkRoute = async (browser, baseUrl, route) => {
    const url = `${baseUrl}${route}`;
    const page = await browser.newPage();
    try {
        const response = await page.goto(url, { waitUntil: 'networkidle', timeout: 15_000 });
        const httpStatus = response?.status() ?? 0;
        if (await is404Page(page)) {
            const h1 = await page.evaluate(() => document.querySelector('h1')?.textContent ?? '');
            return { route, url, httpStatus, h1: h1.trim() };
        }
        return null;
    } finally {
        await page.close();
    }
};

// ─── Main ────────────────────────────────────────────────────────────────────

const main = async () => {
    const files = await collectMarkdownFiles(DOCS_ROOT);
    const routes = [...new Set(files.map(mdFileToRoute))].sort();

    console.info(`Found ${routes.length} routes to check.`);

    const port = await findFreePort();
    const baseUrl = `http://127.0.0.1:${port}`;

    console.info(`Starting vitepress preview on port ${port}…`);
    const server = await startPreviewServer(port);

    let browser;
    const failures = [];

    try {
        browser = await chromium.launch({ headless: true });
        const limit = pLimit(CONCURRENCY);
        let checked = 0;

        const tasks = routes.map((route) =>
            limit(async () => {
                const result = await checkRoute(browser, baseUrl, route);
                if (result) failures.push(result);
                checked++;
                if (checked % PROGRESS_EVERY === 0 || checked === routes.length) {
                    console.info(`  ${checked}/${routes.length} routes checked…`);
                }
            }),
        );

        await Promise.all(tasks);
    } finally {
        if (browser) await browser.close();
        await stopPreviewServer(server);
    }

    if (failures.length === 0) {
        console.info('All routes resolved successfully.');
        process.exit(0);
    }

    console.error(`\n${failures.length} route(s) returned a 404:\n`);
    for (const f of failures) {
        const statusNote = f.httpStatus > 0 ? ` (HTTP ${f.httpStatus})` : '';
        const h1Note = f.h1 ? ` — "${f.h1}"` : '';
        console.error(`  ${f.route}${statusNote}${h1Note}`);
    }
    process.exit(1);
};

main().catch((err) => {
    console.error('check-routes failed:', err instanceof Error ? err.message : String(err));
    if (err instanceof Error && err.stack) console.error(err.stack);
    process.exit(2);
});
