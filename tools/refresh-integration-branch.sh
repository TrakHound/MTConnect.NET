#!/usr/bin/env bash
# Refresh the integration/all-fixes branch with the current tips of every
# in-flight feature branch. The integration branch is consumed by downstream apps for
# end-to-end smoke-checking before any per-plan PR merges upstream.
#
# Usage: tools/refresh-integration-branch.sh [--push]
#
# Flags:
#   --push   force-push the rebuilt integration branch to origin (otherwise
#            stops at "ready to push", lets the user review the merge result).
#
# Configuration: edit IN_FLIGHT_BRANCHES below to add / remove plan branches
# as plans start / merge upstream.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WORKTREE_PATH="${REPO_ROOT}/.claude/worktrees/integration-all-fixes"

# Edit this list as plans start / merge.
# Each entry must be a branch on `origin` (the user's fork).
IN_FLIGHT_BRANCHES=(
    feat/issue-133
    fix/issue-127
    fix/issue-129
    fix/issue-135
    fix/issue-138
    # fix/issue-128       # blocked on bootstrap precondition + plan-file scope fixes (2026-04-25)
    # fix/issue-132       # blocked on bootstrap precondition + plan-file scope fixes (2026-04-25)
    # fix/issue-134       # awaiting subagent completion
    # fix/issue-130-131
    # fix/issue-136-137
    # feat/sysml-importer-improvements
    # feat/xsd-validation
    # test/coverage-and-conventions
    # chore/deps-update-XXXX-MM-DD
)

PUSH=0
if [[ "${1:-}" == "--push" ]]; then PUSH=1; fi

if [[ ! -d "${WORKTREE_PATH}" ]]; then
    echo "error: integration worktree not found at ${WORKTREE_PATH}" >&2
    echo "       create it via:" >&2
    echo "       git worktree add -b integration/all-fixes \\" >&2
    echo "           ${WORKTREE_PATH#${REPO_ROOT}/} upstream/master" >&2
    exit 1
fi

cd "${WORKTREE_PATH}"

echo "==> Fetching upstream + origin..."
git fetch upstream
git fetch origin --multiple

echo "==> Resetting integration/all-fixes to upstream/master..."
git checkout integration/all-fixes
git reset --hard upstream/master

echo "==> Re-merging in-flight plan branches..."
for branch in "${IN_FLIGHT_BRANCHES[@]}"; do
    echo "    - ${branch}"
    if ! git rev-parse --verify "origin/${branch}" >/dev/null 2>&1; then
        echo "      (skipped — origin/${branch} doesn't exist; remove from script if intentional)"
        continue
    fi
    if ! git merge --no-ff "origin/${branch}" -m "merge ${branch} into integration"; then
        echo "      MERGE CONFLICT on ${branch}; resolve manually then re-run." >&2
        exit 2
    fi
done

if [[ "${PUSH}" == "1" ]]; then
    echo "==> Force-push-with-lease to origin..."
    git push --force-with-lease origin integration/all-fixes
    echo "==> Done. integration/all-fixes is now at $(git rev-parse HEAD)"
else
    echo "==> Local merge complete at $(git rev-parse HEAD)."
    echo "    To publish: ${0} --push"
fi
