/**
 * Conventional-commits config for the MTConnect.NET repository.
 *
 * Rules:
 *   - extends `@commitlint/config-conventional` for the base type
 *     grammar (`feat|fix|chore|docs|style|refactor|perf|test|build|
 *     ci|revert`) and the standard `<type>(<scope>): <subject>`
 *     shape;
 *   - pins the allowed scopes so a stray typo (`agnet`, `adaptor`)
 *     or a made-up scope (`stuff`) is rejected at commit-time
 *     instead of leaking into the release pipeline's semver-bump.
 *
 * The scope list matches the module layout of the repo:
 *   - `agent` — anything under `agent/` excluding modules;
 *   - `agent-module` — anything under `agent/Modules/**`;
 *   - `adapter` — anything under `adapter/` excluding modules;
 *   - `adapter-module` — anything under `adapter/Modules/**`;
 *   - `common` — anything under `libraries/**`;
 *   - `sysml-import` — anything under `build/MTConnect.NET-SysML-Import/**`;
 *   - `build` — `build/**` outside the SysML-import project;
 *   - `ci` — `.github/**` + `tools/ci/**`;
 *   - `docs` — `docs/**`;
 *   - `deps` — dependency bumps (used by the weekly deps workflow);
 *   - `release` — the release pipeline itself (`tools/release/**`);
 *   - `test` — anything under `tests/**`.
 *
 * A missing scope is allowed (some cross-cutting changes have no
 * single home); a scope that is not on the pinned list is rejected.
 */

/** @type {import('@commitlint/types').UserConfig} */
export default {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'scope-enum': [
      2,
      'always',
      [
        'agent',
        'agent-module',
        'adapter',
        'adapter-module',
        'common',
        'sysml-import',
        'build',
        'ci',
        'docs',
        'deps',
        'release',
        'test',
      ],
    ],
    // A subject-length ceiling that matches the ≤70-char PR-title
    // convention (which is derived from the last commit's subject on a
    // squash merge). Body/footer are unconstrained.
    'header-max-length': [2, 'always', 70],
    'body-leading-blank': [2, 'always'],
    'footer-leading-blank': [2, 'always'],
  },
};
