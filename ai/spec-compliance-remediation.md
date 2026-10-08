# Spec compliance remediation notes

## Work sequence

1. Treat the assignment and Appendix A as the contract; map each request, response, and callback to the implementation before changing code.
2. Implement the Acme payment and RTTQ client and a runnable local stub using the same documented wire format.
3. Persist the payment intent before the external call; preserve transaction/reference identifiers and reconcile ambiguous outcomes by RTTQ.
4. Verify payment-advice Basic auth and adjustment-webhook raw-body HMAC before binding/processing; persist notification identity, event time, and raw payload atomically with the state transition.
5. Align SQL summary groupings and event names with the settlement requirements.
6. Make local operation and deliverables accurately describe what is implemented and what has not been verified.

## Corrections made

- Replaced the prior flat Acme request/authentication assumption with the Appendix A bearer-authenticated `MessageHeader` / `PaymentRequest` contract and nested `PaymentResponse` parser.
- Added a local stub rather than requiring a developer to supply an external sandbox. It provides scenario switches for payment outcomes, callback duplication/order, reversal/refund, and timeout.
- Saved the local payment as `Processing` before calling Acme and used its stored identifiers for retries/RTTQ.
- Applied callback event deduplication and event-time ordering inside a SQL stored procedure transaction.
- Corrected documentation that implied AWS AppConfig was wired or Redis was the only mutable state.
- Preserved an Acme `Complete`/`COMPL` outcome rather than treating it as an acquirer decline.

## Verification and limits

The nine-test unit suite verifies client serialization/parsing, response forward-compatibility, RTTQ URL construction, rate-limit retry identity, non-JSON gateway error status, Basic/HMAC utilities, atomic stub-store idempotency/reference uniqueness, completed EFT state mapping, and daily-summary aggregation. Compose parsing, local launch shell syntax, the standalone stub build, and container HTTP smoke for payment/RTTQ/replay/conflict/auth were also checked.

The current unit suite does not start SQL Server or exercise the complete callback-to-report flow. The stub's named scenario paths have not all been run as end-to-end tests. No real Acme sandbox request or AWS deployment was performed. The externally hosted walkthrough remains outstanding.
