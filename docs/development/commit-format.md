# Commit-message format

Every commit in this repo follows the [Conventional
Commits](https://www.conventionalcommits.org/) grammar. The
`pre-merge` CI gate rejects any PR whose commit range contains a
subject that does not parse under `commitlint.config.mjs`; the
`lefthook.yml` client-side `commit-msg` hook replays the same check
locally so a broken commit never leaves the workstation in the first
place.

## Grammar

```
<type>(<scope>): <subject>

[optional body]

[optional footer(s)]
```

- `<type>` — one of `feat`, `fix`, `chore`, `docs`, `style`,
  `refactor`, `perf`, `test`, `build`, `ci`, `revert`.
- `<scope>` — optional, but if present must be one of the pinned
  scopes below.
- `<subject>` — imperative, sentence-cased, no trailing full stop.
- The header (type + scope + subject, including the punctuation) is
  capped at 70 characters by `commitlint`'s `header-max-length` rule.
  The rule measures the whole first line, not just the subject — a
  long scope eats into the subject budget.
- The body and each footer must be separated from the header (and
  from each other) by one blank line — `commitlint` enforces
  `body-leading-blank` and `footer-leading-blank`. Body and footer
  lines have no formal length cap in the config today; keep them at
  the Conventional-Commits recommended 100-character wrap for
  readability on `git log --oneline`-adjacent tooling.
- A `!` before the colon (`feat!:`, `fix(scope)!:`) marks a breaking
  change and triggers a major-version bump in the release pipeline.
- A `BREAKING CHANGE:` footer has the same effect.

## Pinned scopes

The scope enum lives in `commitlint.config.mjs`:

| Scope | Covers |
| --- | --- |
| `agent` | Anything under `agent/` excluding modules. |
| `agent-module` | Anything under `agent/Modules/**`. |
| `adapter` | Anything under `adapter/` excluding modules. |
| `adapter-module` | Anything under `adapter/Modules/**`. |
| `common` | Anything under `libraries/**`. |
| `sysml-import` | The `build/MTConnect.NET-SysML-Import` project. |
| `build` | `build/**` outside the SysML-import project. |
| `ci` | `.github/**` + `tools/ci/**`. |
| `docs` | `docs/**`. |
| `deps` | Dependency bumps (used by the weekly deps workflow). |
| `release` | The release pipeline itself (`tools/release/**`). |
| `test` | Anything under `tests/**`. |

A missing scope is allowed — some cross-cutting changes have no
single home. A scope not on the list is rejected.

## Local install

```
npm install --global lefthook
lefthook install
```

That wires the `commit-msg` + `pre-commit` hooks into the local
clone. The `.github/workflows/pre-merge.yml` gate replays the
commit-msg check on every PR head so an un-installed clone cannot
bypass the invariant.

## Testing a message before commit

```
echo "feat(agent): add xyz" | npx commitlint
```

Exit code 0 means the message parses; a non-zero exit prints the
rule that failed.
