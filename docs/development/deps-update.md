# Weekly deps update

The `deps-update` workflow (`.github/workflows/deps-update.yml`) fires
every Saturday at 02:00 UTC. It bumps every dependency in four
ecosystems, split across two PR shapes.

## Ecosystems covered

- GitHub Actions plugin versions in `.github/workflows/*.yml` (via
  Renovate's `github-actions` manager) — one PR per package on a
  `renovate/*` branch.
- Docker base images in every `Dockerfile` (via Renovate's
  `dockerfile` manager) — one PR per base image on a `renovate/*`
  branch.
- npm packages under `docs/` (via `npm-check-updates`) — folded into
  the single `chore/deps-weekly-npm-nuget` bulk PR.
- NuGet packages across every `.csproj` (via `dotnet-outdated-tool`)
  — folded into the same bulk PR as npm.

## Supply-chain quarantine

Three of the four ecosystems apply a minimum-age filter — no version
younger than `env.MIN_AGE_DAYS` (default seven) days is accepted:

- **GitHub Actions + Dockerfile bases** — Renovate's
  `minimumReleaseAge` config passed through
  `.github/renovate-actions-only.json`.
- **npm packages** — `npm-check-updates --cooldown <days>` (v18+),
  which rejects any candidate release whose npm-registry publish
  timestamp is younger than the cooldown window.
- **NuGet packages** — **no quarantine**. `dotnet-outdated` has no
  built-in age filter, so a hot-published bad NuGet release will land
  in the weekly PR unfiltered; downstream CI + reviewer eyes are the
  only line of defense. A proper NuGet quarantine is tracked in
  issue [#237](https://github.com/TrakHound/MTConnect.NET/issues/237).

The invariant catches the standard OSS "poisoned publish yanked
within a week" response window for the three ecosystems that support
it. Increase the window by editing `env.MIN_AGE_DAYS` at the top of
the workflow.

## Auto-merge

The workflow enables auto-merge on the resulting PR (`gh pr merge
--auto --squash`). CI must go green for the merge to happen; a red
CI keeps the PR open for triage.

## PR shapes

Two shapes ship in parallel every week:

- **Per-package Renovate PRs** for GH Actions + Dockerfile bases —
  Renovate opens one PR per package on its own `renovate/*` branch.
  Each PR follows Renovate's own open/close/rebase rules; this
  workflow does not manage them.
- **One bulk PR** for npm + NuGet on `chore/deps-weekly-npm-nuget`.
  If a prior bulk PR is still open when the workflow re-fires, it is
  closed as superseded before the new branch is pushed. Only one bulk
  PR from this workflow is ever open at once — the newest bumps
  supersede the older ones by construction.

## Manual re-run

`gh workflow run deps-update.yml` triggers a run on-demand. The
`workflow_dispatch` handler is present for exactly this case (a hotfix
that needs a fresh dep bump outside the Saturday cadence).
