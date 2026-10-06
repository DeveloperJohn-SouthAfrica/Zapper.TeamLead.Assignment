Feature: Merchant operations dashboard

  Scenario: Surface the operations flow
    Given the dashboard is available
    When the visitor lands on the hero and stats
    Then the hero title contains "Acme payments reliability command center"
    And the stats deck mentions approved, declined, reversals, refunds
    And the merchant summary table is visible
    And the infrastructure board lists Redis cache and SQL Server (DACPAC)
    Then I capture a screenshot of the dashboard for review
