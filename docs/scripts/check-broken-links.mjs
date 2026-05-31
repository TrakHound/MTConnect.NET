#!/usr/bin/env node

// Walk every Markdown file under <docs-root> (default: '.') and report
// internal links that do not resolve. Validates relative paths, docs-root
// absolute paths ('/api/index' -> '<docs-root>/api/index.md'), and anchor
// fragments against rehype-slug-derived IDs on the target file.
//
// External (http/https) URLs are not validated by design: third-party state
// is not the CI gate. The script exits 0 when no broken link is found,
// 1 when one or more broken links are reported, and 2 on a top-level error.

import { readdir, readFile, stat } from 'node:fs/promises';
import { dirname, relative, resolve } from 'node:path';

import inspectUrls from '@jsdevtools/rehype-url-inspector';
import slug from 'rehype-slug';
import stringify from 'rehype-stringify';
import gfm from 'remark-gfm';
import parse from 'remark-parse';
import rehype from 'remark-rehype';
import { unified } from 'unified';

// Directories skipped during the Markdown walk. 'cache' and 'dist' are
// only skipped when their parent is '.vitepress' (they are VitePress's
// gitignored build artefacts); 'node_modules' is dependency state and
// never a link target, so it is skipped wherever it appears.

// Each entry is { sourceFile, line, column, url }. Sorted by source then
// position before reporting so the output reads file-by-file.
const brokenLinks = [];

const recordBrokenLink = ({ sourceFile, position, url }) => {
    const { line, column } = position;
    brokenLinks.push({ sourceFile, line, column, url });
};

const isSkippedDir = (parentPath, name) => {
    if (name === 'node_modules') return true;
    if (name === 'cache' || name === 'dist') {
        return parentPath.endsWith('/.vitepress');
    }
    return false;
};

const getMarkdownFiles = async (directory) => {
    const entries = await readdir(directory, { withFileTypes: true });
    const markdownFiles = [];

    for (const entry of entries) {
        const fullPath = resolve(directory, entry.name);

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

// Cache of slugged-heading ID sets keyed by target file path. A docs tree
// of 2 000+ files cross-links extensively; re-parsing the same target on
// every link would dominate runtime.
const anchorCache = new Map();

// Picks up id="..." attributes from inline-HTML anchors that VitePress
// (and docfx-generated reference pages) embed in Markdown headings. The
// rehype tree only sees slugged-heading ids; raw HTML anchors live in
// `html` nodes and need a separate sweep.
const RAW_ID_PATTERN = /\bid\s*=\s*["']([^"']+)["']/g;

const loadAnchorSet = async (targetFile) => {
    if (anchorCache.has(targetFile)) return anchorCache.get(targetFile);
    const content = await readFile(targetFile, 'utf8');
    const processor = unified().use(parse).use(gfm).use(rehype).use(slug).use(stringify);
    const tree = await processor.run(processor.parse(content));
    const ids = new Set();
    collectIds(tree, ids);
    for (const match of content.matchAll(RAW_ID_PATTERN)) {
        ids.add(match[1]);
    }
    anchorCache.set(targetFile, ids);
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
    const ids = await loadAnchorSet(targetFile);
    return ids.has(anchor);
};

// Resolve a non-external link's filesystem candidates. VitePress allows
// omitting the .md extension, so '<path>' and '<path>.md' are both tried.
// Index pages: '<path>/index.md' covers directory links like '/api/'.
const candidatePaths = (basePath, relativePath, docsRoot) => {
    const cleaned = relativePath.replace(/\/+$/, '');
    const target = cleaned === '' ? basePath : cleaned;
    const root = target.startsWith('/') ? resolve(docsRoot, `.${target}`) : resolve(basePath, target);
    return [root, `${root}.md`, resolve(root, 'index.md')];
};

const firstExistingFile = async (paths) => {
    for (const candidate of paths) {
        try {
            const info = await stat(candidate);
            if (info.isFile()) return candidate;
        } catch {
            // Candidate does not exist; try the next one.
        }
    }
    return undefined;
};

const checkLink = async ({ sourceFile, url, position, basePath, docsRoot }) => {
    // Anchor-only links resolve against the current file's headings.
    if (url.startsWith('#')) {
        const anchor = url.slice(1);
        const ok = await targetHasAnchor(sourceFile, anchor);
        if (!ok) recordBrokenLink({ sourceFile, position, url });
        return;
    }

    const [relativePath, anchor] = url.split('#');
    const existing = await firstExistingFile(candidatePaths(basePath, relativePath, docsRoot));

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
    const content = await readFile(filePath, 'utf8');
    const basePath = dirname(filePath);

    // Collect every link-check promise; the original script's bug was
    // forgetting to await the .then() chain inside inspectEach.
    const pending = [];

    const inspector = unified()
        .use(parse)
        .use(gfm)
        .use(rehype)
        .use(slug)
        .use(inspectUrls, {
            inspectEach({ node, url }) {
                // Skip external URLs and protocol-relative / mailto / etc.
                if (/^[a-z][a-z0-9+\-.]*:/i.test(url)) return;
                if (url.startsWith('//')) return;

                pending.push(
                    checkLink({
                        sourceFile: filePath,
                        url,
                        position: node.position.start,
                        basePath,
                        docsRoot,
                    }),
                );
            },
        })
        .use(stringify);

    await inspector.process(content);
    await Promise.all(pending);
};

const isDirectory = async (inputPath) => {
    try {
        const info = await stat(inputPath);
        return info.isDirectory();
    } catch {
        return false;
    }
};

const checkLinks = async (root) => {
    const docsRoot = resolve(root);
    const isDir = await isDirectory(docsRoot);
    const files = isDir ? await getMarkdownFiles(docsRoot) : [docsRoot];

    for (const file of files) {
        await processFile(file, docsRoot);
    }

    return docsRoot;
};

const reportAndExit = (docsRoot) => {
    if (brokenLinks.length === 0) {
        console.info('No broken links found.');
        process.exit(0);
    }

    brokenLinks.sort((a, b) => {
        if (a.sourceFile !== b.sourceFile) return a.sourceFile.localeCompare(b.sourceFile);
        if (a.line !== b.line) return a.line - b.line;
        return a.column - b.column;
    });

    console.error(`Found ${brokenLinks.length} broken links:`);
    let currentFile = null;
    for (const entry of brokenLinks) {
        if (entry.sourceFile !== currentFile) {
            currentFile = entry.sourceFile;
            console.info(`${relative(docsRoot, currentFile)}:`);
        }
        console.info(`  ${entry.line}:${entry.column} -> ${entry.url}`);
    }
    process.exit(1);
};

checkLinks(process.argv[2] || '.')
    .then(reportAndExit)
    .catch((error) => {
        console.error(error);
        process.exit(2);
    });
