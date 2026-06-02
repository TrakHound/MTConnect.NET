import { readdirSync, readFileSync, statSync, existsSync } from 'node:fs';
import { resolve, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

/**
 * Auto-derived sidebars for the generated docs sections.
 *
 * Two trees feed this module:
 *
 * - `docs/api/` — docfx-generated reference for the public surface of
 *   every shipped MTConnect.NET library. ~1800 type pages keyed by
 *   `MTConnect.<Namespace>.<Type>.md`, plus a docfx-produced `toc.yml`
 *   that lists each namespace and the types under it. We parse `toc.yml`
 *   and build a hierarchical sidebar where namespace dots fold into
 *   nested collapsible groups — so `MTConnect.Devices.DataItems.SampleDataItem`
 *   lives at `MTConnect > Devices > DataItems > SampleDataItem`.
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

// Internal tree node used while building the namespace hierarchy.
// Each node represents one dot-segment in a namespace path. `types`
// holds the leaf entries (sidebar items pointing at type pages);
// `overview` is the namespace landing page link if one exists.
type ApiNode = {
  children: Map<string, ApiNode>;
  types: SidebarItem[];
  overview?: string;
};

/** Allocate an empty namespace-tree node — `children` empty, `types` empty,
 *  no overview link yet. */
const makeNode = (): ApiNode => ({ children: new Map(), types: [] });

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

/** Build the nested namespace tree by walking each namespace's
 *  dot-separated path. Each segment becomes a child node; the final
 *  segment receives the namespace's overview href and the type list.
 *  Returns the synthetic root whose children are the top-level segments
 *  (`MTConnect`, …). */
const buildApiTree = (
  namespaces: ReturnType<typeof parseToc>,
): ApiNode => {
  const root = makeNode();
  for (const ns of namespaces) {
    const segments = ns.name.split('.');
    let node = root;
    for (const segment of segments) {
      let child = node.children.get(segment);
      if (!child) {
        child = makeNode();
        node.children.set(segment, child);
      }
      node = child;
    }
    if (ns.href) node.overview = hrefToRoute(ns.href);
    for (const t of ns.types) {
      node.types.push({ text: t.name, link: hrefToRoute(t.href) });
    }
  }
  return root;
};

/** Case-insensitive locale comparator so groups and types sort predictably
 *  regardless of underlying string ordering quirks (e.g. uppercase ASCII
 *  grouping ahead of lowercase). Stable on equal-keyed inputs. */
const byTextCI = (a: SidebarItem, b: SidebarItem) =>
  a.text.localeCompare(b.text, 'en', { sensitivity: 'base' });

/** Recursively project the tree into VitePress sidebar items. A node with
 *  children becomes a collapsible group; types are sorted into the group
 *  alongside any nested child groups. The namespace overview (if present)
 *  leads the group as an "Overview" entry. Input: the path segment whose
 *  node we are projecting + the node itself; output: one SidebarItem. */
const projectNode = (segment: string, node: ApiNode): SidebarItem => {
  const items: SidebarItem[] = [];
  if (node.overview) {
    items.push({ text: 'Overview', link: node.overview });
  }
  const typeItems = [...node.types].sort(byTextCI);
  const childItems = [...node.children.entries()]
    .map(([s, n]) => projectNode(s, n))
    .sort(byTextCI);
  // Children (sub-namespaces) listed before types so the hierarchy
  // reads top-down: nested namespaces first, then the types declared
  // directly in this namespace.
  items.push(...childItems, ...typeItems);
  return {
    text: segment,
    collapsed: true,
    items,
  };
};

// Module-level cache keyed on toc.yml mtime. VitePress hot-reloads the
// config on any edit to config.ts; without the cache, each hot-reload
// re-reads + re-parses the ~1800-namespace toc.yml. The mtime check
// keeps the cache correct when `npm run regen` rewrites toc.yml in the
// same Node process.
let apiSidebarCache: { mtimeMs: number; sidebar: SidebarItem[] } | undefined;

/**
 * Build the `/api/` sidebar from docfx's `toc.yml`. Returns a single
 * top-level "API reference" group whose items are the nested namespace
 * tree. Falls back to a one-entry "Overview" sidebar when the docfx
 * output is missing (e.g. a tree without `npm run regen` first).
 */
export const apiSidebar = (): SidebarItem[] => {
  const tocPath = resolve(docsRoot, 'api', 'toc.yml');
  const overview: SidebarItem = { text: 'Overview', link: '/api/' };
  if (!existsSync(tocPath)) {
    return [{ text: 'API reference', items: [overview] }];
  }
  const mtimeMs = statSync(tocPath).mtimeMs;
  if (apiSidebarCache && apiSidebarCache.mtimeMs === mtimeMs) {
    return apiSidebarCache.sidebar;
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

  const tree = buildApiTree(namespaces);
  const topLevel = [...tree.children.entries()]
    .map(([s, n]) => projectNode(s, n))
    .sort(byTextCI);
  const sidebar: SidebarItem[] = [
    {
      text: 'API reference',
      items: [overview, ...topLevel],
    },
  ];
  apiSidebarCache = { mtimeMs, sidebar };
  return sidebar;
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
