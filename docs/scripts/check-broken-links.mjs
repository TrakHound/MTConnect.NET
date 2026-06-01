#!/usr/bin/env node

// Walk every Markdown file under <docs-root> (default: '.') and report
// internal links that do not resolve. Validates relative paths, docs-root
// absolute paths ('/api/index' -> '<docs-root>/api/index.md'), and anchor
// fragments against rehype-slug-derived IDs on the target file.
//
// External (http/https) URLs are not validated by design: third-party state
// is not the CI gate. The script exits 0 when no broken link is found,
// 1 when one or more broken links are reported, and 2 on a top-level error.
//
// Usage: check-broken-links.mjs [<docs-root>]

import { readdir, readFile, stat } from 'node:fs/promises';
import { cpus } from 'node:os';
import { dirname, relative, resolve, sep } from 'node:path';

import slug from 'rehype-slug';
import stringify from 'rehype-stringify';
import gfm from 'remark-gfm';
import parse from 'remark-parse';
import rehype from 'remark-rehype';
import { unified } from 'unified';
import { visit } from 'unist-util-visit';

// Directories skipped during the Markdown walk. 'cache' and 'dist' are
// only skipped when their parent is '.vitepress' (they are VitePress's
// gitignored build artefacts); 'node_modules' is dependency state and
// never a link target, so it is skipped wherever it appears. Dot-prefixed
// directories (.git, .docfx, .vitepress, …) are skipped wholesale to keep
// fresh local clones from surfacing scratch markdown under them.

// Each entry is { sourceFile, line, column, url }. Sorted by source then
// position before reporting so the output reads file-by-file.
const brokenLinks = [];

const ZERO_POSITION = { line: 0, column: 0 };

const recordBrokenLink = ({ sourceFile, position, url }) => {
    const { line, column } = position ?? ZERO_POSITION;
    brokenLinks.push({ sourceFile, line, column, url });
};

const isSkippedDir = (parentPath, name) => {
    if (name === 'node_modules') return true;
    if (name.startsWith('.')) return true;
    if (name === 'cache' || name === 'dist') {
        return parentPath.endsWith(`${sep}.vitepress`);
    }
    return false;
};

// Directories the walker could not read (permission tweak mid-run, race
// condition on a transient checkout) are recorded here and surfaced at
// report time so the operator sees what was skipped.
const skippedDirectories = [];

const getMarkdownFiles = async (directory) => {
    let entries;
    try {
        entries = await readdir(directory, { withFileTypes: true });
    } catch (error) {
        const message = error instanceof Error ? error.message : String(error);
        skippedDirectories.push({ directory, message });
        return [];
    }
    const markdownFiles = [];

    for (const entry of entries) {
        const fullPath = resolve(directory, entry.name);

        // Symlinks are not followed; a stray docs/ symlink to /etc would
        // otherwise leak presence + line numbers of files outside the tree.
        if (entry.isSymbolicLink()) continue;

        if (entry.isDirectory()) {
            if (isSkippedDir(directory, entry.name)) continue;
            const nested = await getMarkdownFiles(fullPath);
            markdownFiles.push(...nested);
        } else if (entry.isFile() && entry.name.endsWith('.md')) {
            markdownFiles.push(fullPath);
        }
    }

    return markdownFiles;
};

// Cache the in-flight Promise<Set<string>> rather than the resolved Set:
// two concurrent links into the same un-cached target must share one parse
// rather than each kicking off their own. Same-file anchor lookups reuse
// the in-progress entry via the same cache.
const anchorCache = new Map();

// Tightened raw-HTML id sweep:
//  - (?<![\w-:]) prevents data-id, aria-labelledby, and any namespaced
//    *:id (xml:id, etc.) from matching the bare "id=" segment.
//  - No whitespace in the captured value matches what a real anchor id
//    would be.
//  - Fenced code blocks are stripped before the sweep so an id="…" inside
//    a code sample does not silently expand the anchor set.
const RAW_ID_PATTERN = /(?<![\w-:])id\s*=\s*["']([^"'\s]+)["']/g;
const FENCED_CODE_PATTERN = /^([ \t]*)(`{3,}|~{3,})[^\n]*\n[\s\S]*?\n\1\2[ \t]*$/gm;

const stripFencedCode = (content) => content.replace(FENCED_CODE_PATTERN, '');

// CRLF -> LF normalisation is shared by every markdown read so the fenced-
// code regex (anchored at \n) matches uniformly on Windows clones and so
// every downstream pass sees identical line counts.
const readMarkdownNormalized = async (file) => (await readFile(file, 'utf8')).replace(/\r\n/g, '\n');

const computeAnchorSet = async (targetFile) => {
    const content = await readMarkdownNormalized(targetFile);
    const processor = unified().use(parse).use(gfm).use(rehype, { allowDangerousHtml: true }).use(slug).use(stringify);
    const tree = await processor.run(processor.parse(content));
    const ids = new Set();
    collectIds(tree, ids);
    for (const match of stripFencedCode(content).matchAll(RAW_ID_PATTERN)) {
        ids.add(match[1]);
    }
    return ids;
};

const collectIds = (node, ids) => {
    if (!node) return;
    if (node.properties && typeof node.properties.id === 'string') ids.add(node.properties.id);
    if (Array.isArray(node.children)) {
        for (const child of node.children) collectIds(child, ids);
    }
};

const targetHasAnchor = async (targetFile, anchor) => {
    let pending = anchorCache.get(targetFile);
    if (!pending) {
        pending = computeAnchorSet(targetFile);
        anchorCache.set(targetFile, pending);
    }
    return (await pending).has(anchor);
};

// Resolve a non-external link's filesystem candidates. VitePress allows
// omitting the .md extension, so '<path>' and '<path>.md' are both tried.
// Index pages: '<path>/index.md' covers directory links like '/api/'.
// Percent-encoded segments (%20 for spaces, etc.) are decoded so the
// on-disk lookup matches.
const safeDecode = (segment) => {
    try {
        return decodeURIComponent(segment);
    } catch {
        return segment;
    }
};

const candidatePaths = (basePath, relativePath, docsRoot) => {
    const cleaned = relativePath.replace(/\/+$/, '');
    if (cleaned === '') {
        // URL was bare '/' or '' (already de-fragmented / de-queried by
        // the caller); resolve to the docs-root index page.
        return [resolve(docsRoot, 'index.md')];
    }
    const decoded = safeDecode(cleaned);
    const root = decoded.startsWith('/') ? resolve(docsRoot, `.${decoded}`) : resolve(basePath, decoded);
    return [root, `${root}.md`, resolve(root, 'index.md')];
};

// Defence-in-depth: a markdown link of the form [x](/../../../etc/passwd)
// would otherwise stat() arbitrary paths above docsRoot. Containment is
// enforced by string-prefix match against the resolved docs root.
const isInsideDocsRoot = (candidate, docsRoot) => candidate === docsRoot || candidate.startsWith(docsRoot + sep);

const firstExistingFile = async (paths, docsRoot) => {
    for (const candidate of paths) {
        if (!isInsideDocsRoot(candidate, docsRoot)) continue;
        try {
            const info = await stat(candidate);
            if (info.isFile()) return candidate;
        } catch {
            // Candidate does not exist; try the next one.
        }
    }
    return undefined;
};

// Split on '#' before stripping '?': both '?…#…' and '#…?…' shapes appear
// in the wild. Splitting on '#' first preserves the fragment for the
// 'path?query#anchor' case; the query is then stripped from each side so
// the on-disk lookup never sees it.
const stripQuery = (segment) => {
    const [head] = segment.split('?');
    return head;
};

const splitUrl = (url) => {
    const [head, ...frag] = url.split('#');
    const path = stripQuery(head);
    const anchor = frag.length > 0 ? stripQuery(frag.join('#')) : '';
    return { path, anchor };
};

const checkLink = async ({ sourceFile, url, position, basePath, docsRoot }) => {
    // '#' on its own is a deliberate placeholder some editors emit; treat
    // as a no-op rather than reporting it broken.
    if (url === '#') return;

    // Anchor-only links resolve against the current file's headings.
    if (url.startsWith('#')) {
        const anchor = url.slice(1);
        const ok = await targetHasAnchor(sourceFile, anchor);
        if (!ok) recordBrokenLink({ sourceFile, position, url });
        return;
    }

    const { path: relativePath, anchor } = splitUrl(url);
    const existing = await firstExistingFile(candidatePaths(basePath, relativePath, docsRoot), docsRoot);

    if (!existing) {
        recordBrokenLink({ sourceFile, position, url });
        return;
    }

    if (anchor) {
        const ok = await targetHasAnchor(existing, anchor);
        if (!ok) recordBrokenLink({ sourceFile, position, url });
    }
};

const processFile = async (filePath, docsRoot) => {
    const content = await readMarkdownNormalized(filePath);
    const basePath = dirname(filePath);

    const processor = unified().use(parse).use(gfm).use(rehype, { allowDangerousHtml: true }).use(slug).use(stringify);
    const tree = await processor.run(processor.parse(content));

    const pending = [];

    visit(tree, (node) => {
        if (!node || (node.type !== 'element')) return;
        if (node.tagName !== 'a' && node.tagName !== 'img') return;
        const url = node.properties?.[node.tagName === 'a' ? 'href' : 'src'];
        if (typeof url !== 'string' || url === '') return;
        if (/^[a-z][a-z0-9+\-.]*:/i.test(url)) return;
        if (url.startsWith('//')) return;

        const position = node.position?.start ?? ZERO_POSITION;

        pending.push(
            checkLink({
                sourceFile: filePath,
                url,
                position,
                basePath,
                docsRoot,
            }),
        );
    });

    await Promise.all(pending);
};

// Bounded concurrency for the file walk. The original serial loop was the
// single biggest contributor to runtime; running ~ cpu-count parses at
// once cuts the 2k-file pass materially without overwhelming the FS cache.
// Every launched promise carries its own .catch so a transient rejection
// (FS hiccup mid-parse, etc.) does not leak as an unhandled rejection nor
// abort the rest of the pass.
const runWithConcurrency = async (items, limit, worker) => {
    const queue = items.slice();
    const inFlight = new Set();

    const launch = () => {
        if (queue.length === 0) return null;
        const item = queue.shift();
        const promise = (async () => worker(item))()
            .catch((error) => {
                const message = error instanceof Error ? error.message : String(error);
                recordBrokenLink({ sourceFile: item, position: ZERO_POSITION, url: `(worker failure: ${message})` });
            })
            .finally(() => inFlight.delete(promise));
        inFlight.add(promise);
        return promise;
    };

    const cap = Math.max(1, limit);
    while (queue.length > 0 || inFlight.size > 0) {
        while (inFlight.size < cap && queue.length > 0) {
            launch();
        }
        if (inFlight.size > 0) {
            await Promise.race(inFlight);
        }
    }
};

const isDirectory = async (inputPath) => {
    try {
        const info = await stat(inputPath);
        return info.isDirectory();
    } catch {
        return false;
    }
};

const parseArgs = (argv) => {
    const positional = [];
    for (const arg of argv) {
        if (arg === '--help' || arg === '-h') {
            console.info('Usage: check-broken-links.mjs [<docs-root>]');
            process.exit(0);
        } else {
            positional.push(arg);
        }
    }
    return { root: positional[0] ?? '.' };
};

const checkLinks = async ({ root }) => {
    const docsRoot = resolve(root);
    const isDir = await isDirectory(docsRoot);
    const files = isDir ? await getMarkdownFiles(docsRoot) : [docsRoot];

    const concurrency = Math.max(2, Math.min(16, cpus().length));
    await runWithConcurrency(files, concurrency, (file) => processFile(file, docsRoot));

    return docsRoot;
};

const reportAndExit = (docsRoot) => {
    if (skippedDirectories.length > 0) {
        console.error(`Skipped ${skippedDirectories.length} directories that could not be read:`);
        for (const skipped of skippedDirectories) {
            const rel = relative(docsRoot, skipped.directory) || skipped.directory;
            console.error(`  ${rel}: ${skipped.message}`);
        }
    }

    if (brokenLinks.length === 0) {
        console.info('No broken links found.');
        process.exit(0);
    }

    brokenLinks.sort((a, b) => {
        if (a.sourceFile !== b.sourceFile) return a.sourceFile.localeCompare(b.sourceFile);
        if (a.line !== b.line) return a.line - b.line;
        return a.column - b.column;
    });

    // Single stream for the entire failure block, in a per-row format
    // editor problem-matchers (VS Code, vim :cfile, GHA annotations) pick
    // up — '<file>:<line>:<col>: broken link <url>'. Routes to stderr so
    // CI grep / tee pairings stay coherent.
    console.error(`Found ${brokenLinks.length} broken links:`);
    for (const entry of brokenLinks) {
        const rel = relative(docsRoot, entry.sourceFile);
        console.error(`${rel}:${entry.line}:${entry.column}: broken link ${entry.url}`);
    }
    process.exit(1);
};

checkLinks(parseArgs(process.argv.slice(2)))
    .then(reportAndExit)
    .catch((error) => {
        const message = error instanceof Error ? error.message : String(error);
        console.error('check-broken-links failed:', message);
        if (error instanceof Error && error.stack) console.error(error.stack);
        console.error("Hint: run from the 'docs/' directory; the script expects a markdown root as its sole positional argument.");
        process.exit(2);
    });
