 # Copilot instructions

 The Payments Integration Service is a .NET 8 containerized API that manages sub-merchant and stored-card tokens, submits payments to a stubbed Acme Pay API (simulating timeouts, out-of-order notifications and idempotency), persists payment state and notifications in a SQL database, and exposes a daily per-merchant payment summary.

 ## Key conventions

 **JSON envelope:** app requests/responses and Acme payment/RTTQ messages wrap payloads in a `MessageHeader` with `MessageDateTime`, `MessageID` (UUID) and `Channel` (`App`/`Web`/`POS`). Preserve the exact flat JSON shape for Acme payment-advice and adjustment callbacks.

 **Field naming:** PascalCase for JSON fields; enums like `TransactionStatus` use exact values (Success, Failure, Reversed, Complete, Refunded); `CardOnFileScenario` values are CIT/MIT.

 **Amounts & currency:** amounts are integers in the lowest monetary unit; `CurrencyCode` optional, defaults to ZAR.

 **Sub-merchant facilitator fields:** `SubMerchantID`, `MerchantCategoryCode`, `CardAcceptorName` (fixed 40-char, space-padded with sub-fields at fixed positions).

 **Forward-compatibility:** ignore unrecognized fields in Acme API responses.

 **Webhooks & notifications:** Basic auth for payment advice (`ACME_ADVICE_USERNAME`/`PASSWORD`), HMAC-SHA256 `x-signature` header for reversal/refund webhook (`ACME_WEBHOOK_SECRET`, Base64 of raw body), retry on non-200 responses.

 ## Deliverables & workflow

 Refer to `SOLUTION.md` for design trade-offs, AWS readiness and AI workflow. Commit `README.md` (local run instructions), `SOLUTION.md`, AI config files (e.g., `CLAUDE.md`, `AGENTS.md`, `.cursor/rules/`) and `ai/` artifacts. Local credentials in Compose/launch profiles are development placeholders only.
