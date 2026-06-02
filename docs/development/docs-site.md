# Documentation site

The documentation site you are reading is built with [VitePress](https://vitepress.dev/) from the `docs/` directory in the repository. This page covers how to build it locally and how the production deploy resolves its public base path.

## Local development

```
cd docs
npm ci
npm run dev
```

The dev server hot-reloads markdown edits, theme tweaks, and config changes. It serves from `http://localhost:5173/` with no path prefix.

## Production build

```
cd docs
npm run build
```

The build emits a static site under `docs/.vitepress/dist/`. Preview it with `npm run preview`, which serves the built artifact at the same `localhost:5173` origin.

## Base path—`DOCS_BASE`

VitePress emits asset URLs (CSS, JavaScript, fonts, images) at build time using its `base` configuration. The shipped config reads `base` from the `DOCS_BASE` environment variable and defaults to `/` when unset:

```ts
// docs/.vitepress/config.ts
base: process.env.DOCS_BASE ?? '/',
```

Why it matters: when the built site is served from a sub-path—for example GitHub Pages at `https://trakhound.github.io/MTConnect.NET/`—the asset URLs must include the `/MTConnect.NET/` prefix or the browser fetches `https://trakhound.github.io/assets/…` and 404s every asset, leaving an unstyled raw-HTML render. The base must match the public deploy URL at the time `vitepress build` runs; rewriting it post-build is not supported by VitePress.

| Environment | `DOCS_BASE` value | Resolved asset URL |
| --- | --- | --- |
| Local dev (`npm run dev` / `npm run preview`) | unset (defaults to `/`) | `http://localhost:5173/assets/…` |
| GitHub Pages production build | `/MTConnect.NET/` | `https://trakhound.github.io/MTConnect.NET/assets/…` |
| Custom deploy target at `https://docs.example.com/mtconnect/` | `/mtconnect/` | `https://docs.example.com/mtconnect/assets/…` |
| Custom deploy target at a root domain like `https://docs.example.com/` | unset or `/` | `https://docs.example.com/assets/…` |

The variable is read by Node at the moment `vitepress build` starts, so any deployment pipeline can override it by exporting the variable in the build step.

## How CI sets the base

`.github/workflows/docs.yml` builds the site on every push and pull request, and deploys the artifact to GitHub Pages on merges to `master`. The build step passes `DOCS_BASE: /MTConnect.NET/` explicitly:

```yaml
- name: Build site
  working-directory: docs
  run: npm run build
  env:
    DOCS_BASE: /MTConnect.NET/
```

A fork or third-party deploy that hosts the same documentation under a different URL—for example a vendor preview hosted at `/preview/`—sets `DOCS_BASE: /preview/` on its own build step. No code changes are needed.

## Diagnosing a styling break on a live deploy

The classic symptom of a base mismatch is a deployed page that renders as raw HTML: title and tagline run together without spacing, the `Search` button reads `SearchK` because the keyboard-shortcut hint sits adjacent to the label without CSS spacing, and the nav links concatenate without separators. Check the page's HTML source for `<link rel="stylesheet">` and `<script type="module">` tags; if their `href` / `src` attributes do not include the deploy URL's path prefix, the build was produced with the wrong `DOCS_BASE`.

## End-to-end route check

`tests/MTConnect.NET-Docs-Tests/RouteCheckTests.cs` is a Playwright e2e fixture that builds the docs site, spawns `vitepress preview` against the built `dist/` tree, walks every route the markdown source tree implies in a headless Chromium browser, and asserts no client-side 404s. CI runs it on the `ubuntu-latest` matrix leg of `.github/workflows/dotnet.yml` (the `windows-latest` leg filters `Category=E2E` out — hosted Windows runners do not carry Linux-image Docker, and the test fixture's `npm ci && npm run build` bootstrap is the easier target to keep Linux-only).

Run locally from the repo root:

```
dotnet test tests/MTConnect.NET-Docs-Tests --filter Category=E2E
```

On the first run the fixture installs the chromium binary the Playwright .NET binding drives (~150 MB; cached on subsequent runs) and — if `docs/.vitepress/dist/` is missing — invokes `npm ci && npm run build` from `docs/` to produce a preview-able site. Subsequent runs reuse both, so a warm working tree completes in a couple of minutes; a cold checkout takes longer because the build artefact is rebuilt from scratch.

Failure output names every route that surfaced as a 404 along with which of the two signals fired—the `.NotFound` element rendered by the VitePress default theme's NotFound component, or `document.title` starting with `404` (the static `404.html` emits `<title>404 | MTConnect.NET</title>`, so a prefix match catches it regardless of the trailing site-title suffix). Typical fixes:

| Symptom | Probable cause | Fix |
| --- | --- | --- |
| "vitepress preview did not bind to 127.0.0.1:NNNN within 60s" | Stale `dist/` tree from a prior aborted build, or `node_modules/` mismatch after a vitepress bump. | `rm -rf docs/.vitepress/dist docs/node_modules && cd docs && npm ci && npm run build`. |
| "/some-route — .NotFound element present" | Sidebar entry points at a markdown file that no longer exists, or a vitepress route was added without the matching `.md`. | Remove the sidebar entry or add the page; re-run the test. |
| "playwright install chromium exited NNN" | The chromium download was blocked or interrupted. | Re-run; if the install fails again, run `dotnet tool restore && npx playwright install chromium` manually and re-try. |
| "port NNNN already in use" hang | Another process grabbed the free port between selection and bind (rare; mitigated by the banner-parse fallback). | Re-run the test — the next free port avoids the contended one. |

The negative companion test (`A_Synthetic_Unmapped_Route_Surfaces_As_A_404`) pins the detector itself: a future Playwright or VitePress upgrade that breaks one of the two 404 signals would still see every real route render fine, but the negative test would go red and surface the regression before a real 404 slipped past.

## See also

- [Release builder](/development/builder)—the in-tree tool that assembles binary release artifacts (NuGet packages, installers, Docker images). Distinct from the documentation site build covered above.
