## Platform prerequisites

1. **Install nvm and Node 24**
   ```bash
   curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.8/install.sh | bash
   source "$HOME/.nvm/nvm.sh"
   nvm install 24
   node -v    # should read v24.21.0
   npm -v     # should read 11.19.0
   ```

2. **Install Copilot CLI (BYOM/BYOK aware)**
   ```bash
   curl -fsSL https://gh.io/copilot-install | bash
   ```
   After installation, run `copilot` in this repo, use `/init`, and `exit` once initialization completes. Configure BYOM/BYOK credentials via `copilot config set cloud.byoKey <FILE>` or the dashboard-provided secret so that the CLI always uses our chosen model and avoids unbounded token usage.

3. **Optional tooling that keeps us agentic**
   - **Astral UV package manager (Linux)**  
     `curl -LsSf https://astral.sh/uv/install.sh | sh`
   - **Graphify (for structure-aware diagrams)**  
     `uv tool install graphifyy` (then run `graphifyy` from the repo as needed).
   - **Context7 (context-first guidance for autocoding)**  
     `npx ctx7 setup`
   - **Impeccable (quality guardrails and visual QA)**  
     `npm install -g impeccable` or add to devDependencies; run `impeccable scan` during reviews.

## Tooling workflow notes

- **Copilot CLI** will be our primary agent. Keep it context aware by starting each session from this repo, pointing it at our BYOM/BYOK endpoint, and supplying only the files that describe the current focus.
- **Graphify** lets us extract diagrams from structured prompts; use it to capture the DFD/ERD/Sequence drafts before they are committed into docs.
- **Context7** steers the autopilot toward existing documentation—always seed its context with `setup.md`, `Zapper-Team-Lead-Assignment-Polished.md`, and `docs/analysis/*.md` so it grounds answers.
- **Impeccable** runs infrequently but as a sanity check when touching UI or UX-related outputs (storyboards, instructions, or diagrams).

## Repository setup

1. `npm install` (if package files appear) or run the repo-specific bootstrap.
2. Keep environment variables in a `.env.local` (excluded) and reference them via `process.env`.
3. After tooling is ready, run `npx ctx7 validate` (if available) to check context captures before letting Copilot act.
