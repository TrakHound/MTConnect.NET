import { mkdirSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';

/**
 * Pages that were removed, merged or moved out of the published docs.
 *
 * Each old URL keeps working: `writeRedirects` (run from `buildEnd` in
 * config.ts) writes a static `<old-route>.html` into the build output that
 * redirects to the new location. GitHub Pages serves `<route>.html` for the
 * clean URL `<route>`, so links from other sites, bookmarks and search results
 * land on the new page. The redirect pages are not part of the docs source, so
 * they are not in the sidebar, the search index or the route check.
 *
 * Keys are site routes without the base path or `.html`; values are a site
 * route (optionally with `#anchor`) or an absolute URL.
 */
const contributing = 'https://github.com/TrakHound/MTConnect.NET/blob/master/contributing/';

export const redirects: Record<string, string> = {
  // Contributor documentation, moved to contributing/ in the repository
  '/testing': `${contributing}testing.md`,
  '/testing/workflows': `${contributing}testing/workflows.md`,
  '/testing/v2-6': `${contributing}testing/v2-6.md`,
  '/testing/v2-7': `${contributing}testing/v2-7.md`,
  '/development/docs-site': `${contributing}docs-site.md`,
  '/development/house-style': `${contributing}house-style.md`,
  '/development/builder': `${contributing}builder.md`,
  '/cli/dotnet-sh': `${contributing}tools/dotnet-sh.md`,
  '/cli/test-sh': `${contributing}tools/test-sh.md`,
  '/cli/sysml-import': `${contributing}tools/sysml-import.md`,
  '/compliance/test-harness': `${contributing}compliance/test-harness.md`,
  '/compliance/spec-cross-references': `${contributing}compliance/spec-cross-references.md`,
  '/README': '/',

  // Integration pages merged into one page; the MQTT topic protocol moved to Wire formats
  '/configure/integrations/influxdb': '/configure/integrations/#influxdb',
  '/configure/integrations/mqtt-aws-iot': '/configure/integrations/#mqtt-to-aws-iot',
  '/configure/integrations/mqtt-aws-greengrass-moquette': '/configure/integrations/#aws-greengrass-moquette-broker',
  '/configure/integrations/mqtt-aws-greengrass-mqtt-bridge': '/configure/integrations/#aws-greengrass-mqtt-bridge',
  '/configure/integrations/mqtt-hivemq': '/configure/integrations/#mqtt-to-hivemq',
  '/configure/integrations/openssl': '/configure/integrations/#create-a-self-signed-certificate',
  '/configure/integrations/mqtt-protocol': '/wire-formats/mqtt',
  '/wire-formats/json-v1': '/wire-formats/#the-formats',

  // Examples that duplicated cookbook pages
  '/examples/agent-embedded': '/cookbook/write-an-agent#embedded-agent',
  '/examples/client-mqtt': '/cookbook/write-a-json-mqtt-consumer#use-the-built-in-mqtt-client',

  // Concept and compliance pages merged into their companion page
  '/concepts/components': '/concepts/devices#components',
  '/concepts/observations': '/concepts/data-items#observations',
  '/compliance/wire-format': '/compliance/per-version-matrix#wire-formats',
};

/**
 * Where a section of a merged page went, for links to `<old-route>#<anchor>`.
 * Anchors not listed here go to the page-level target above.
 */
export const anchorRedirects: Record<string, Record<string, string>> = {
  '/concepts/components': {
    'organizer-components': '/concepts/devices#organizer-components',
    compositions: '/concepts/devices#compositions',
    'querying-the-tree': '/concepts/devices#device-tree-traversal',
    'version-gating': '/concepts/devices#version-gating',
    'where-to-next': '/concepts/devices#where-to-next',
  },
  '/concepts/observations': {
    'the-unavailable-sentinel': '/concepts/data-items#the-unavailable-sentinel',
    'value-class-aware-empty-result-coerce': '/concepts/data-items#value-class-aware-empty-result-coerce',
    samples: '/concepts/data-items#samples',
    events: '/concepts/data-items#events',
    conditions: '/concepts/data-items#conditions',
    'reset-triggers': '/concepts/data-items#reset-triggers',
    'sample-vs-current-vs-probe': '/concepts/data-items#sample-vs-current-vs-probe',
    'where-to-next': '/concepts/data-items#where-to-next',
  },
  '/compliance/wire-format': {
    xml: '/compliance/per-version-matrix#xml',
    'json-v1': '/compliance/per-version-matrix#json-v1',
    'json-cppagent-v2': '/compliance/per-version-matrix#json-cppagent-v2',
    'json-cppagent-mqtt': '/compliance/per-version-matrix#json-cppagent-mqtt',
    shdr: '/compliance/per-version-matrix#shdr',
    'variants-of-compliant': '/compliance/per-version-matrix#variants-of-compliant',
    'cross-implementation-matrix': '/compliance/per-version-matrix#cross-implementation-matrix',
    'where-to-next': '/compliance/per-version-matrix#where-to-next',
  },
};

const escape = (s: string) => s.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;');

/** Write one redirect page per entry in `redirects` into `outDir`. */
export const writeRedirects = (outDir: string, base: string) => {
  const url = (to: string) => (/^[a-z]+:/i.test(to) ? to : base.replace(/\/$/, '') + to);
  for (const [from, to] of Object.entries(redirects)) {
    const target = url(to);
    const anchors = Object.fromEntries(Object.entries(anchorRedirects[from] ?? {}).map(([a, t]) => [a, url(t)]));
    // The meta refresh covers browsers without script; the script also maps a #section link to its new place
    const script = Object.keys(anchors).length
      ? `<script>var a=${JSON.stringify(anchors).replace(/</g, '\\u003c')}[location.hash.slice(1)];` +
        `location.replace(a||${JSON.stringify(target)});</script>`
      : '';
    const html =
      '<!doctype html><html lang="en"><head><meta charset="utf-8">' +
      `<title>Moved</title><meta name="robots" content="noindex">` +
      `<link rel="canonical" href="${escape(target)}">${script}` +
      `<meta http-equiv="refresh" content="0; url=${escape(target)}">` +
      `</head><body><p>This page has moved to <a href="${escape(target)}">${escape(target)}</a>.</p></body></html>\n`;
    const file = join(outDir, `${from.replace(/^\//, '')}.html`);
    mkdirSync(dirname(file), { recursive: true });
    writeFileSync(file, html);
  }
};
