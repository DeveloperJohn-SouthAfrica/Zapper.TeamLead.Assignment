# Walkthrough recording outline (5–10 minutes)

**Status: outline only.** Record and share a hosted video separately; this repository does not contain a video or an external link.

1. **0:00–1:00 — Problem and contract.** Show the assignment scope and Appendix A; explain minor-unit amounts, account merchant ID, message envelopes, and flat callback payloads.
2. **1:00–2:30 — Architecture.** Trace API, application handler, SQL repositories/procedures, Acme client, and the local stub. Distinguish SQL payment durability from optional ancillary Redis/MassTransit infrastructure.
3. **2:30–4:00 — Submit and reconcile.** Show the Acme request/response mapping, persisted `Processing` state, same-ID retry semantics, and timeout/RTTQ scenario configuration.
4. **4:00–5:30 — Callbacks and reporting.** Show Basic advice authentication, raw-body HMAC validation, SQL idempotency/event-time ordering, the daily summary grouping, and exact HTTP 200 acknowledgement.
5. **5:30–7:00 — Run and verify.** Show `scripts/local-up.sh`, scenario selection, test command, and what the tests do and do not cover.
6. **7:00–8:30 — Trade-offs and operations.** Explain SQL choice, external-system ambiguity, AWS deployment proposal, secrets, graceful shutdown, and key diagnostics from `SOLUTION.md`.
7. **8:30–9:00 — Honest limits.** State that no live Acme sandbox/AWS deployment or SQL-backed end-to-end lifecycle run was performed, and identify follow-up verification.
