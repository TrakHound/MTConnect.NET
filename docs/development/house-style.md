# Docs-site house style

This page records the documentation-site visual choices the maintainer signed off on. Each row cites the GitHub discussion thread the answer came from so a future contributor can see who decided what, when, and why. Treat this page as the canonical answer when a new docs PR proposes a brand, palette, typography, or social-share change — match what shipped, or open a new discussion to revise it.

The source of record for the current set is [Discussion #184](https://github.com/TrakHound/MTConnect.NET/discussions/184), maintainer reply dated 2026-06-01.

## What shipped

| Surface | Answer | Source |
| --- | --- | --- |
| Logo | `docs/img/mtconnect-net-03-md.png`, copied into `docs/public/logo.png` and wired through `themeConfig.logo`. The asset already carries the `mtconnect .NET` wordmark, so `themeConfig.siteTitle` is hidden to avoid the wordmark rendering twice in the top nav. | Discussion #184 |
| Accent color | `#0073e6` — the TrakHound brand accent. Overrides `--vp-c-brand-1`, `--vp-c-brand-2`, `--vp-c-brand-3`, and `--vp-c-brand-soft` for both light (`:root`) and dark (`.dark`) modes. Implementation lives in `docs/.vitepress/theme/style.css`. | Discussion #184 |
| Typography | VitePress default. The maintainer expressed no strong preference, so no font-family override is applied. | Discussion #184 |
| Hero block | A third call-to-action — `Download latest release` — links to `https://github.com/TrakHound/MTConnect.NET/releases/latest`. Sits between `Get started` and `View on GitHub`. | Discussion #184 |
| Code-block syntax theme | VitePress default. Kept per the maintainer's preference. | Discussion #184 |
| Favicon | `<link rel="icon" type="image/png" href="/logo.png">`. Same asset as the hero logo. | Discussion #184 |
| Social-share preview | Open Graph (`og:type`, `og:title`, `og:description`, `og:image`) and Twitter Card (`twitter:card=summary_large_image`, `twitter:title`, `twitter:description`, `twitter:image`) meta tags in `head[]`. `og:image` and `twitter:image` both point at `/logo.png`. | Discussion #184 |
| `theme-color` | `#0073e6` — updated from the previous `#1f6feb` to match the new brand accent. Drives mobile browser chrome color. | Bonus alignment with the accent change, not separately requested in #184. |
| Google Analytics | Out of scope. The maintainer is wiring analytics on the deploy side; no GA `<script>` tag is added to the source. | Discussion #184 |

## File map

| Surface | File |
| --- | --- |
| Logo asset | `docs/public/logo.png` |
| Brand-palette CSS override | `docs/.vitepress/theme/style.css` |
| Theme entry that loads the override | `docs/.vitepress/theme/index.ts` |
| Favicon + OG / Twitter meta + `theme-color` | `docs/.vitepress/config.ts` (`head[]`) |
| `themeConfig.logo` + `themeConfig.siteTitle: false` | `docs/.vitepress/config.ts` (`themeConfig`) |
| Hero `Download latest release` CTA | `docs/index.md` (front-matter `hero.actions`) |

## Changing any of this

The list above is a maintainer decision, not a contributor preference. Before opening a PR that revises an entry — re-skinning the palette, replacing the logo, switching the syntax theme, adding analytics — open a discussion that proposes the change and tag the maintainer. Link the resulting answer here once it merges, replacing the row that changed.

## See also

- [Documentation site](/development/docs-site) — how the site is built and how `DOCS_BASE` works.
- [Discussion #184](https://github.com/TrakHound/MTConnect.NET/discussions/184) — the source-of-record thread for the current answers.
