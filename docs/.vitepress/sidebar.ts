import { readdirSync, readFileSync, statSync, existsSync } from 'node:fs';
import { resolve, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

/**
 * Auto-derived sidebars for the generated docs sections.
 *
 * Two trees feed this module:
 *
 * - `docs/api/` — docfx-generated reference for the public surface of
 *   every shipped MTConnect.NET library. ~2400 type pages keyed by
 *   `MTConnect.<Namespace>.<Type>.md`, plus a docfx-produced `toc.yml`
 *   that lists each namespace and the types under it. We parse `toc.yml`
 *   and build one small sidebar per namespace (its parent namespaces,
 *   its child namespaces and its own types) rather than one sidebar
 *   holding every type: VitePress renders the whole sidebar into every
 *   page that uses it, so a single ~2500-entry API sidebar made each of
 *   the ~2400 API pages ~1.4 MB of HTML (3.2 GB in total) and dominated
 *   the site build time.
 *
 * - `docs/reference/` — Roslyn-generated narrative reference (CLI flags,
 *   environment variables, configuration schema, HTTP API). A flat
 *   alphabetical list keyed off whatever `.md` files the generator
 *   emits, so a future page added by `MTConnect.NET-DocsGen` surfaces
 *   in the sidebar without a config-side edit.
 *
 * Both functions are pure-fs reads; they run at VitePress config-load
 * time (which is before the dev server boots and before the build
 * traverses pages). The npm `predev` / `prebuild` hooks regenerate
 * both trees before VitePress reads them, so a fresh clone produces
 * a fully-populated sidebar on the first `npm run dev`.
 */

// VitePress sidebar shapes (loosely typed — VitePress accepts plain
// objects, and pulling the official types would require an import that
// VitePress' own config doesn't enforce).
type SidebarItem = {
  text: string;
  link?: string;
  collapsed?: boolean;
  items?: SidebarItem[];
};

// Resolve paths relative to this file so the module works whether
// VitePress is invoked from `docs/` or from the repo root.
const here = dirname(fileURLToPath(import.meta.url));
const docsRoot = resolve(here, '..');

// ─── /api/ — docfx hierarchy ────────────────────────────────────────────────

/** Strip the `.md` extension and any leading `/` so the result is a clean
 *  VitePress route, e.g. `MTConnect.Adapters.AgentClient.md` →
 *  `/api/MTConnect.Adapters.AgentClient`. Input: docfx href; output:
 *  VitePress route string. */
const hrefToRoute = (href: string): string =>
  `/api/${href.replace(/\.md$/, '')}`;

// Minimal `toc.yml` parser. The docfx-emitted file is regular enough
// that a line-oriented parse is robust and avoids pulling in a YAML
// dependency. Schema:
//
//   - name: <namespace>
//     href: <namespace>.md
//     items:
//     - name: Classes              # section divider (no href)
//     - name: <type>
//       href: <namespace>.<type>.md
//     - name: Structs              # next divider
//     ...
//   - name: <next namespace>
//     ...
/** Parse the docfx-emitted `toc.yml` body into a flat list of namespace
 *  entries, each with its overview href and an ordered list of type
 *  entries. Tolerates section dividers (`- name: Classes` without href)
 *  by treating them as no-op pending entries. */
const parseToc = (content: string) => {
  const namespaces: Array<{
    name: string;
    href: string;
    types: Array<{ name: string; href: string }>;
  }> = [];

  let current: (typeof namespaces)[number] | null = null;
  let pending: { name: string; href?: string } | null = null;
  let inItems = false;

  /** Commit the buffered type entry to `current.types` if it has an href;
   *  drop it silently if it was a section-divider (`- name: Classes`). */
  const flushPending = () => {
    if (!pending || !current || !pending.href) return;
    current.types.push({ name: pending.name, href: pending.href });
    pending = null;
  };

  for (const raw of content.split('\n')) {
    const line = raw.replace(/\r$/, '');
    // Top-level namespace entry.
    let m = /^- name: (.+)$/.exec(line);
    if (m) {
      flushPending();
      if (current) namespaces.push(current);
      current = { name: m[1], href: '', types: [] };
      pending = null;
      inItems = false;
      continue;
    }
    // Top-level href for the namespace block currently being parsed.
    m = /^ {2}href: (.+)$/.exec(line);
    if (m && current && !inItems) {
      current.href = m[1];
      continue;
    }
    if (/^ {2}items:$/.test(line)) {
      inItems = true;
      continue;
    }
    // Item-level entry inside the current namespace block.
    m = /^ {2}- name: (.+)$/.exec(line);
    if (m && inItems) {
      flushPending();
      pending = { name: m[1] };
      continue;
    }
    m = /^ {4}href: (.+)$/.exec(line);
    if (m && pending) {
      pending.href = m[1];
      continue;
    }
  }
  flushPending();
  if (current) namespaces.push(current);
  return namespaces;
};

/** Case-insensitive locale comparator so groups and types sort predictably
 *  regardless of underlying string ordering quirks (e.g. uppercase ASCII
 *  grouping ahead of lowercase). Stable on equal-keyed inputs. */
const byTextCI = (a: SidebarItem, b: SidebarItem) =>
  a.text.localeCompare(b.text, 'en', { sensitivity: 'base' });

type SidebarMap = Record<string, SidebarItem[]>;

// Module-level cache keyed on toc.yml mtime. VitePress hot-reloads the
// config on any edit to config.ts; without the cache, each hot-reload
// re-reads + re-parses toc.yml. The mtime check keeps the cache correct
// when `npm run regen` rewrites toc.yml in the same Node process.
let apiSidebarCache: { mtimeMs: number; sidebars: SidebarMap } | undefined;

/**
 * Build the `/api/` sidebars from docfx's `toc.yml`, as a map to spread
 * into the VitePress `sidebar` config.
 *
 * Every namespace `N` gets its own key, `/api/N.`. VitePress matches a
 * key against the page's source path (`api/N.md`, `api/N.Type.md`), so
 * the key covers the namespace page and the type pages under it. All
 * keys have the same number of `/` segments, which is what VitePress
 * sorts by, so ties fall back to insertion order: keys are inserted
 * longest first, so `MTConnect.Devices.DataItems.` wins over
 * `MTConnect.Devices.` for a DataItems type. `/api/` comes last and
 * catches `/api/` itself plus anything outside a namespace.
 *
 * A namespace sidebar lists the API overview and the parent namespaces,
 * then the namespace itself: its overview, its child namespaces and its
 * own types. The `/api/` sidebar lists every namespace. Falls back to a
 * one-entry "Overview" sidebar when the docfx output is missing (e.g. a
 * tree without `npm run regen` first).
 */
export const apiSidebars = (): SidebarMap => {
  const tocPath = resolve(docsRoot, 'api', 'toc.yml');
  const overview: SidebarItem = { text: 'Overview', link: '/api/' };
  if (!existsSync(tocPath)) {
    return { '/api/': [{ text: 'API reference', items: [overview] }] };
  }
  const mtimeMs = statSync(tocPath).mtimeMs;
  if (apiSidebarCache && apiSidebarCache.mtimeMs === mtimeMs) {
    return apiSidebarCache.sidebars;
  }
  const tocBody = readFileSync(tocPath, 'utf8');
  const namespaces = parseToc(tocBody);

  // Self-check: every top-level `- name:` line should produce exactly one
  // parsed namespace. A mismatch means the hand-rolled YAML parser silently
  // dropped entries — most likely a docfx output reformat (extra indent,
  // BOM, quoted strings). Fail loudly rather than letting the sidebar
  // surface a partial tree.
  const nameLineCount = tocBody.split('\n').filter((l) => /^- name: /.test(l)).length;
  if (nameLineCount !== namespaces.length) {
    throw new Error(
      `sidebar.ts: parsed ${namespaces.length} namespace(s) from api/toc.yml but counted ${nameLineCount} top-level entries. ` +
        `The hand-rolled parser likely needs updating — check for indentation or quoting changes from docfx.`,
    );
  }

  type Namespace = (typeof namespaces)[number];
  const byName = new Map<string, Namespace>(namespaces.map((ns) => [ns.name, ns]));
  const link = (ns: Namespace): SidebarItem =>
    ns.href ? { text: ns.name, link: hrefToRoute(ns.href) } : { text: ns.name };

  // Nearest namespace that encloses `name` (`MTConnect.Devices` for
  // `MTConnect.Devices.DataItems`), skipping segments that are not
  // namespaces in their own right.
  const parentOf = (name: string): Namespace | undefined => {
    for (let i = name.lastIndexOf('.'); i > 0; i = name.lastIndexOf('.', i - 1)) {
      const parent = byName.get(name.slice(0, i));
      if (parent) return parent;
    }
    return undefined;
  };
  const children = new Map<string, Namespace[]>();
  for (const ns of namespaces) {
    const parent = parentOf(ns.name);
    if (parent) children.set(parent.name, [...(children.get(parent.name) ?? []), ns]);
  }

  const sidebars: SidebarMap = {};
  const longestFirst = [...namespaces].sort((a, b) => b.name.length - a.name.length);
  for (const ns of longestFirst) {
    const ancestors: Namespace[] = [];
    for (let p = parentOf(ns.name); p; p = parentOf(p.name)) ancestors.unshift(p);

    const items: SidebarItem[] = [];
    if (ns.href) items.push({ text: 'Overview', link: hrefToRoute(ns.href) });
    const childLinks = (children.get(ns.name) ?? []).map(link).sort(byTextCI);
    if (childLinks.length > 0) {
      items.push({ text: 'Namespaces', collapsed: false, items: childLinks });
    }
    items.push(
      ...ns.types.map((t) => ({ text: t.name, link: hrefToRoute(t.href) })).sort(byTextCI),
    );

    const key = `/api/${(ns.href || ns.name).replace(/\.md$/, '')}.`;
    sidebars[key] = [
      { text: 'API reference', items: [overview, ...ancestors.map(link)] },
      { text: ns.name, items },
    ];
  }
  sidebars['/api/'] = [
    { text: 'API reference', items: [overview] },
    { text: 'Namespaces', items: namespaces.map(link).sort(byTextCI) },
  ];

  apiSidebarCache = { mtimeMs, sidebars };
  return sidebars;
};

// ─── /reference/ — Roslyn-generated narrative ─────────────────────────────

/** Convert a file name like `environment-variables.md` to a sidebar label
 *  like `Environment variables` (lower-case-with-hyphens to sentence case,
 *  keeping mid-word capitals as-is for HTTP/CLI/etc.). Overrides cover
 *  acronyms only; no trailing-word suffix is added — the parent group
 *  ("Auto-generated reference") already supplies the "reference" context,
 *  so adding it per-leaf is redundant and the half-applied policy (only on
 *  `cli` and `configuration`) was the worst of both worlds. */
const labelFor = (slug: string): string => {
  const overrides: Record<string, string> = {
    cli: 'CLI',
    'http-api': 'HTTP API',
    'environment-variables': 'Environment variables',
    configuration: 'Configuration',
  };
  if (overrides[slug]) return overrides[slug];
  const spaced = slug.replace(/-/g, ' ');
  return spaced.charAt(0).toUpperCase() + spaced.slice(1);
};

/**
 * Build the `/reference/` sidebar by listing every `.md` file under
 * `docs/reference/` except `index.md` (which is wired in as the
 * "Overview" entry). Pages sort alphabetically by displayed label,
 * so a new generator output appears without a config-side edit.
 */
export const referenceSidebar = (): SidebarItem[] => {
  const refRoot = resolve(docsRoot, 'reference');
  const items: SidebarItem[] = [
    { text: 'Overview', link: '/reference/' },
  ];
  if (existsSync(refRoot)) {
    const pages = readdirSync(refRoot)
      .filter((f) => f.endsWith('.md') && f !== 'index.md')
      .map((f) => {
        const slug = f.replace(/\.md$/, '');
        return { text: labelFor(slug), link: `/reference/${slug}` };
      })
      .sort(byTextCI);
    items.push(...pages);
  }
  return [
    {
      text: 'Auto-generated reference',
      items,
    },
  ];
};
