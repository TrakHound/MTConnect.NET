// Builds the redirect pages published on GitHub Pages (https://trakhound.github.io/MTConnect.NET/).
//
// The MTConnect.NET documentation moved to the MTConnect.NET Docs on www.trakhound.com. Every route of the old
// documentation site gets a static page that redirects to its new location, so links from other sites, bookmarks and
// search results keep working. redirects.json lists the routes:
//
//   pages    old route -> new path (relative to `site`) or full URL. Routes ending with "/" are section index pages.
//   anchors  old route -> { anchor -> new path } for sections of pages that were merged into another page.
//   api      where every route under /api/ (the old generated API reference) goes.
//   fallback where any other unknown route goes.
//
// A redirect keeps the #anchor of the old URL (the new pages keep the old heading anchors), unless the anchor is
// listed in `anchors` or the new path has its own anchor. Unknown routes are handled by 404.html, which GitHub Pages
// serves for any path that has no file.
//
//   node docs/redirects/build.mjs [output directory]    (default: docs/redirects/_site)
//
// Requires Node.js 18 or later, with no packages.

import { mkdirSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const here = dirname(fileURLToPath(import.meta.url));
const outDir = process.argv[2] ?? join(here, '_site');
const config = JSON.parse(readFileSync(join(here, 'redirects.json'), 'utf8'));

const url = (to) => (/^[a-z]+:/i.test(to) ? to : config.site + to);
const escapeHtml = (s) => s.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;');
const json = (o) => JSON.stringify(o).replace(/</g, '\\u003c');

// The page a redirect sends the browser to, from the old route and anchor (without "#")
const resolveScript =
  'function resolve(r,h){var a=(A[r]||{})[h];if(a)return a;var t=P[r];if(!t)return null;' +
  'return t.indexOf("#")<0&&h?t+"#"+h:t}';

const page = (target, script) =>
  '<!doctype html><html lang="en"><head><meta charset="utf-8">' +
  '<title>MTConnect.NET Docs have moved</title><meta name="robots" content="noindex">' +
  `<link rel="canonical" href="${escapeHtml(target)}">${script}` +
  `<meta http-equiv="refresh" content="0; url=${escapeHtml(target)}">` +
  '</head><body><p>The MTConnect.NET documentation has moved to ' +
  `<a href="${escapeHtml(target)}">${escapeHtml(target)}</a>.</p></body></html>\n`;

const pages = Object.fromEntries(Object.entries(config.pages).map(([route, to]) => [route, url(to)]));
const anchors = Object.fromEntries(
  Object.entries(config.anchors).map(([route, map]) => [
    route,
    Object.fromEntries(Object.entries(map).map(([anchor, to]) => [anchor, url(to)])),
  ])
);
for (const route of Object.keys(anchors)) {
  if (!pages[route]) throw new Error(`anchors lists ${route}, which isn't in pages`);
}

rmSync(outDir, { recursive: true, force: true });
mkdirSync(outDir, { recursive: true });

const write = (file, html) => {
  const path = join(outDir, file);
  mkdirSync(dirname(path), { recursive: true });
  writeFileSync(path, html);
};

// One page per route. The meta refresh covers browsers without script; the script keeps the #anchor.
for (const [route, target] of Object.entries(pages)) {
  const r = json(route);
  const script =
    `<script>var P={},A={};P[${r}]=${json(target)};A[${r}]=${json(anchors[route] ?? {})};${resolveScript}` +
    `location.replace(resolve(${r},location.hash.slice(1)));</script>`;
  const file = route.endsWith('/') ? `${route.slice(1)}index.html` : `${route.slice(1)}.html`;
  write(file, page(target, script));
}

// Any other path: the same route with a different spelling (ex. "/cli/agent/", "/cli/agent.html", "/cli"),
// anything under /api/, or else the docs overview.
const fallback = url(config.fallback);
const notFound =
  `<script>var P=${json(pages)},A=${json(anchors)};${resolveScript}` +
  'var p=location.pathname,b="/MTConnect.NET";if(p.indexOf(b+"/")===0||p===b)p=p.slice(b.length)||"/";' +
  'p=p.replace(/\\/index(\\.html)?$/,"/").replace(/\\.html$/,"");var h=location.hash.slice(1);' +
  'var t=resolve(p,h)||resolve(p.replace(/\\/$/,""),h)||resolve(p+"/",h);' +
  `if(!t)t=/^\\/api(\\/|$)/.test(p)?${json(config.api)}:${json(fallback)};location.replace(t);</script>`;
write('404.html', page(fallback, notFound));

// Serve the files as they are (no Jekyll processing)
write('.nojekyll', '');

console.log(`Wrote ${Object.keys(pages).length} redirect pages and 404.html to ${outDir}`);
