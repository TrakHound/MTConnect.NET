# Documentation

The user documentation for MTConnect.NET is published as the [MTConnect.NET Docs](https://www.trakhound.com/docs/mtconnect-net) on www.trakhound.com. The pages are Razor pages in the TrakHound.Site repository, alongside the other TrakHound documentation, so they are edited and reviewed there.

Three things about the documentation stay in this repository: the generated reference pages, the redirects from the old documentation site, and this contributor documentation.

## Generated reference pages

`docs/reference/` holds four pages generated from the source by [`build/MTConnect.NET-DocsGen`](../build/MTConnect.NET-DocsGen/):

| Page | Built from |
| --- | --- |
| `http-api.md` | The HTTP routes the agent's HTTP server registers |
| `environment-variables.md` | `Environment.GetEnvironmentVariable` reads, and `${MTCONNECT_…}` references in the contributor scripts |
| `configuration.md` | The configuration option classes |
| `cli.md` | The command-line tools in the repository |

Regenerate them after changing any of those:

```bash
bash docs/scripts/generate-reference.sh
```

`bash docs/scripts/generate-reference.sh --check` reports any page that is out of date with the source, without writing files.

The MTConnect.NET Docs show these pages from a copy in TrakHound.Site. After they change here, run `tools/mtconnect-net-docs/refresh-reference.ps1` in TrakHound.Site to update the copy.

## Redirects from the old documentation site

The documentation used to be a VitePress site on GitHub Pages at `https://trakhound.github.io/MTConnect.NET/`. GitHub Pages now serves only redirect pages, so links from other sites, bookmarks and search results land on the new pages.

- [`docs/redirects/redirects.json`](../docs/redirects/redirects.json) lists every route of the old site and its new path on www.trakhound.com, or a GitHub URL for the contributor pages that moved to `contributing/`. Sections of pages that were merged into another page have their own entries under `anchors`.
- [`docs/redirects/build.mjs`](../docs/redirects/build.mjs) writes one page per route, plus a `404.html` for anything else. A redirect keeps the `#anchor` of the old URL, because the new pages keep the old heading anchors. Every route under `/api/` (the old generated API reference) goes to the TrakHound profile on nuget.org, and any other unknown route goes to the docs overview.
- `.github/workflows/docs.yml` builds the pages for pull requests that change them, and deploys them to GitHub Pages on merges to `master` (or when run by hand).

Build and look at the pages locally (Node.js 18 or later, no packages):

```bash
node docs/redirects/build.mjs
```

The pages are written to `docs/redirects/_site/`.

## Images

`docs/img/` holds images that READMEs (ex. the agent README) link to with `raw.githubusercontent.com` URLs. Keep the paths stable, since copies of those READMEs on other sites link to them.

## See also

- [Release builder](builder.md): the in-tree tool that assembles the release artifacts (NuGet packages, installers, Docker images).
