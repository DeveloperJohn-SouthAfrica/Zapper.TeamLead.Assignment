# Graph Report - AcmeIntegreatedAssessement  (2026-10-05)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 2065 nodes · 3306 edges · 153 communities (103 shown, 50 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 114 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `cfcef3ca`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- RepositoryBase
- BoundedLoggingDestructuringPolicy
- RedisOmRepositoryBase
- Intent.RoslynWeaver.Attributes
- FinbuckleConsumingFilter
- MassTransitMessageBus
- SubMerchant
- AcmeIntegreatedAssessement.Application.Common.Interfaces
- .ConfigureSwagger
- AcmeTransactionNotification
- AcmeIntegreatedAssessement.Infrastructure/DependencyInjection.cs
- .Handle
- ScopedExecutor
- CustomerSearch
- add-entity-dialog-sample.razor
- HttpClientRequestException
- AcmeApprovedNotificationDto
- .AddInfrastructure
- search-entity-sample.razor
- edit-entity-sample.razor.cs
- AcmeIntegreatedAssessement.Domain.Repositories
- edit-entity-dialog-sample.razor
- AccountLayout.razor
- AcmeIntegreatedAssessement.Domain.Entities
- Payment
- Program.cs
- add-entity-sample.razor
- edit-entity-sample.razor
- SwashbuckleConfiguration.cs
- IntegrationTestWebAppFactory
- .ExecuteAsync
- AcmeIntegreatedAssessement.Api
- MainLayout
- NotificationDelivery
- AddCustomerDialog
- CustomerEdit
- CurrentUserService
- .Handle
- DomainEvent
- AcmeReversalNotificationDto
- CustomerAdd
- GetPaymentStateQuery
- IValidatorProvider
- .AddApplication
- ComparisonResult
- DistributedCacheWithUnitOfWork
- AcmeIntegreatedAssessement.Infrastructure.Services
- StoredCardToken
- AcmeIntegreatedAssessement.Infrastructure
- CustomerView
- UpdateCustomerModel
- view-entity-sample.razor
- .SendAsync
- UserInfo
- _Imports.razor
- NotificationDeliveryRepository
- PaymentChange
- .QueryAsync
- ApplicationDbContext
- UpdateCustomerModel
- CreateCustomerModel
- PaymentRepository
- EditCustomerDialog
- AcmeIntegreatedAssessement.Application
- IStoredCardTokenRepository
- ValueObject
- CreateCustomerModel1
- Task
- ProcessAcmeApprovedNotificationCommand
- ProcessAcmeReversalNotificationCommand
- ICurrentUserService
- BaseIntegrationTest
- IndexCreationService
- app-main-layout-sample.razor
- AbstractValidator
- MainLayout.razor
- OpenTelemetryConfiguration.cs
- .ProcessAcmeApprovedNotification
- IDistributedCacheWithUnitOfWork
- PagedResult
- SubmitPaymentRequestDto
- ScopedData
- app-user-menu-auth-sample.razor
- CreateCustomerModel2
- app-header-auth-sample.razor
- HealthChecksConfiguration.cs
- SubmitPaymentCommand
- DistributedCacheExtensions
- IntegrationEventConsumer
- CreateCustomerModel
- UpdateCustomerCommandAddressesModel
- UpdateCustomerCommandAddressesModel
- AcmeIntegreatedAssessement.Application.UnitTests
- AwsSecretsManagerOptions
- SignalRConfiguration
- .ApplyAppSettings
- CreateCustomerCommandAddressesModel
- MainLayoutHeader.razor
- Home.razor
- .ConfigureApplicationSecurity
- AwsSecretsManagerConfigurationSource.cs
- AwsSecretsManagerConfigurationProvider
- AuthorizationBehaviour.cs
- PerformanceBehaviour
- ValidationBehaviour
- DomainEventService.cs
- AcmeIntegreatedAssessement.Application.IntegrationTests
- .CreateOrUpdateCollection
- CreateCustomerCommandAddressesModel1
- Routes.razor
- AcmeIntegreatedAssessement.Api.Configuration
- BugsnagConfiguration.cs
- LoggingBehaviour
- UnhandledExceptionBehaviour
- Result
- AuthorizeAttribute
- CreateCustomerCommandAddressesModel
- UpdateCustomerCommandLoyaltyModel
- ThemeToggle.razor
- .ConfigureProblemDetails
- ICurrentUser
- PaymentStateResponseDto
- app-sider-auth-sample.razor
- UpdateCustomerCommandLoyaltyModel
- .OnInitializedAsync
- ReflectionHelper
- UpdateCustomerPreferenceModel
- UpdateCustomerPreferenceModel
- RedirectToLogin.razor
- App.razor
- MainLayoutSider.razor
- Home.razor.cs
- .GetNameForDocument
- app-sider-noauth-sample.razor
- CreateCustomerCommandLoyaltyModel
- CreateCustomerPreferenceModel
- CreateCustomerPreferenceModel1
- CreateCustomerPreferenceModel2
- CreateCustomerCommandLoyaltyModel
- intent-architect
- start-dotnet.sh

## God Nodes (most connected - your core abstractions)
1. `Intent.RoslynWeaver.Attributes` - 150 edges
2. `AcmeIntegreatedAssessement.Application.Common.Interfaces` - 38 edges
3. `AddCustomerDialog` - 37 edges
4. `Payment` - 33 edges
5. `CustomerEdit` - 32 edges
6. `AcmeApprovedNotificationDto` - 31 edges
7. `CustomerAdd` - 29 edges
8. `SubMerchant` - 26 edges
9. `EditCustomerDialog` - 25 edges
10. `CustomerSearch` - 24 edges

## Surprising Connections (you probably didn't know these)
- `RepositoryBase` --references--> `IUnitOfWork`  [EXTRACTED]
  AcmeIntegreatedAssessement.Infrastructure/Repositories/RepositoryBase.cs → AcmeIntegreatedAssessement.Domain/Common/Interfaces/IUnitOfWork.cs
- `IntegrationTestWebAppFactory` --references--> `Program`  [EXTRACTED]
  AcmeIntegreatedAssessement.Application.IntegrationTests/IntegrationTestWebAppFactory.cs → AcmeIntegreatedAssessement.Api/Program.cs
- `SubmitPaymentCommandHandler` --references--> `IStoredCardTokenRepository`  [EXTRACTED]
  AcmeIntegreatedAssessement.Application/Commands/SubmitPayment/SubmitPaymentCommandHandler.cs → AcmeIntegreatedAssessement.Domain/Repositories/IStoredCardTokenRepository.cs
- `SubmitPaymentCommandHandler` --references--> `ISubMerchantRepository`  [EXTRACTED]
  AcmeIntegreatedAssessement.Application/Commands/SubmitPayment/SubmitPaymentCommandHandler.cs → AcmeIntegreatedAssessement.Domain/Repositories/ISubMerchantRepository.cs
- `NotificationDeliveryRepository` --implements--> `INotificationDeliveryRepository`  [EXTRACTED]
  AcmeIntegreatedAssessement.Infrastructure/Repositories/NotificationDeliveryRepository.cs → AcmeIntegreatedAssessement.Application/Common/Interfaces/INotificationDeliveryRepository.cs

## Import Cycles
- None detected.

## Communities (153 total, 50 thin omitted)

### Community 0 - "RepositoryBase"
Cohesion: 0.09
Nodes (19): PagedList, PageCount, PageNo, PageSize, TotalCount, IEFRepository, UnitOfWork, IPagedList (+11 more)

### Community 1 - "BoundedLoggingDestructuringPolicy"
Cohesion: 0.06
Nodes (9): IdentityRedirectManager, IdentityRevalidatingAuthenticationStateProvider, RevalidationInterval, ExceptionFilter, ProblemDetailsExtensions, BoundedLoggingDestructuringPolicy, PropertyAnalysisInfo, PropertyAnalysisResult (+1 more)

### Community 2 - "RedisOmRepositoryBase"
Cohesion: 0.08
Nodes (7): IRedisOmRepository, UnitOfWork, IRepository, RedisOmUnitOfWork, RedisOmRepositoryBase, UnitOfWork, SubstitutionExpressionVisitor

### Community 3 - "Intent.RoslynWeaver.Attributes"
Cohesion: 0.09
Nodes (9): AcmeIntegreatedAssessement.Application.Common.Behaviours, AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeReversalNotification, AcmeIntegreatedAssessement.Application.Commands.ProcessAcmeApprovedNotification, AcmeIntegreatedAssessement.Application.Common.Validation, AcmeIntegreatedAssessement.Api.Controllers, AcmeIntegreatedAssessement.Application.Contracts, AcmeIntegreatedAssessement.Application.Queries.GetPaymentState, AcmeIntegreatedAssessement.Application.Commands.SubmitPayment (+1 more)

### Community 4 - "FinbuckleConsumingFilter"
Cohesion: 0.05
Nodes (7): MultiTenancyConfiguration, FinbuckleConsumingFilter, FinbuckleMessageHeaderStrategy, FinbucklePublishingFilter, FinbuckleSendingFilter, TenantExtendedInfo, ConnectionString

### Community 5 - "MassTransitMessageBus"
Cohesion: 0.07
Nodes (9): MessageBusPublishBehaviour, IMessageBus, MassTransitConfiguration, DispatchType, Publish, Send, MassTransitMessageBus, ConsumeContext (+1 more)

### Community 6 - "SubMerchant"
Cohesion: 0.08
Nodes (14): SubMerchant, City, Country, DomainEvents, Id, MerchantCategoryCode, Payments, Region (+6 more)

### Community 7 - "AcmeIntegreatedAssessement.Application.Common.Interfaces"
Cohesion: 0.14
Nodes (7): IBypassPipelineValidation, AuditActor, AcmeIntegreatedAssessement.Infrastructure.Data, AcmeIntegreatedAssessement.Application.Common.Interfaces, AcmeIntegreatedAssessement.Infrastructure.Repositories, AcmeIntegreatedAssessement.Infrastructure.Caching, AcmeIntegreatedAssessement.Infrastructure.Audit

### Community 8 - ".ConfigureSwagger"
Cohesion: 0.07
Nodes (6): RequireNonNullablePropertiesSchemaFilter, SwashbuckleConfiguration, AuthorizeCheckOperationFilter, HideRouteParametersFromBodyOperationFilter, TenantHeaderOperationFilter, TypeSchemaFilter

### Community 9 - "AcmeTransactionNotification"
Cohesion: 0.06
Nodes (29): AcmeAdjustmentNotification, CurrencyCode, IsApproved, Rrn, TransactionAmount, TransactionDateTime, TransactionType, TransactionUUID (+21 more)

### Community 10 - "AcmeIntegreatedAssessement.Infrastructure/DependencyInjection.cs"
Cohesion: 0.12
Nodes (5): IIntegrationEventHandler, AcmeIntegreatedAssessement.Infrastructure.Configuration, AcmeIntegreatedAssessement.Infrastructure.Eventing, AcmeIntegreatedAssessement.Application.Common.Eventing, AcmeIntegreatedAssessement.Infrastructure.MultiTenant

### Community 11 - ".Handle"
Cohesion: 0.10
Nodes (7): ProcessAcmeApprovedNotificationCommandHandler, ProcessAcmeReversalNotificationCommandHandler, SubmitPaymentCommandHandler, INotificationDeliveryRepository, IPaymentChangeRepository, IPaymentRepository, GetPaymentStateQueryHandler

### Community 12 - "ScopedExecutor"
Cohesion: 0.10
Nodes (4): IScopedExecutor, IScopedMediator, ScopedExecutor, ScopedMediator

### Community 13 - "CustomerSearch"
Cohesion: 0.09
Nodes (10): CustomerSearch, CustomersModels, DialogService, IsActive, Mediator, NavigationManager, SearchText, Snackbar (+2 more)

### Community 14 - "add-entity-dialog-sample.razor"
Cohesion: 0.07
Nodes (28): DialogActions, DialogContent, HeaderContent, MudButton, MudDialog, MudDivider, MudForm, MudGrid (+20 more)

### Community 15 - "HttpClientRequestException"
Cohesion: 0.08
Nodes (16): ForbiddenAccessException, HttpClientRequestException, ProblemDetails, ReasonPhrase, RequestUri, ResponseContent, ResponseHeaders, StatusCode (+8 more)

### Community 16 - "AcmeApprovedNotificationDto"
Cohesion: 0.07
Nodes (25): AcmeApprovedNotificationDto, AcmeNotificationTransactionUuid, Amount, AppPaymentId, AuthCode, CommProtocol, CurrencyCode, EventType (+17 more)

### Community 17 - ".AddInfrastructure"
Cohesion: 0.10
Nodes (7): IAcmePayClient, ConnectionStringResolver, IConnectionStringResolver, PaymentDatabaseSettings, ConnectionStringName, DependencyInjection, AcmePayClient

### Community 18 - "search-entity-sample.razor"
Cohesion: 0.08
Nodes (24): HeaderContent, MudButton, MudCard, MudCardContent, MudChip, MudGrid, MudIcon, MudIconButton (+16 more)

### Community 19 - "edit-entity-sample.razor.cs"
Cohesion: 0.20
Nodes (3): UI.AI.Samples.Api.Components.Pages.Templates.Pages, UI.AI.Samples.Api.Components.Pages.Dialogs.Customers, MudBlazor

### Community 20 - "AcmeIntegreatedAssessement.Domain.Repositories"
Cohesion: 0.13
Nodes (6): PagedResultMappingExtensions, IRedisOmDocument, AcmeIntegreatedAssessement.Infrastructure.Persistence.Documents, AcmeIntegreatedAssessement.Domain.Common.Interfaces, AcmeIntegreatedAssessement.Domain.Repositories, AcmeIntegreatedAssessement.Application.Common.Pagination

### Community 21 - "edit-entity-dialog-sample.razor"
Cohesion: 0.09
Nodes (22): DialogActions, DialogContent, HeaderContent, MudButton, MudDialog, MudForm, MudIconButton, MudNumericField (+14 more)

### Community 22 - "AccountLayout.razor"
Cohesion: 0.09
Nodes (16): LayoutComponentBase, MudDialogProvider, MudLayout, MudMainContent, MudPopoverProvider, MudSnackbarProvider, MudThemeProvider, ThemeToggle (+8 more)

### Community 23 - "AcmeIntegreatedAssessement.Domain.Entities"
Cohesion: 0.17
Nodes (4): AcmeIntegreatedAssessement.Domain.Entities, AcmeIntegreatedAssessement.Infrastructure.Persistence.Configurations, AcmeIntegreatedAssessement.Domain.Common, AcmeIntegreatedAssessement.Infrastructure.Persistence

### Community 24 - "Payment"
Cohesion: 0.11
Nodes (14): Payment, AcmeTransactionUuid, Amount, AppPaymentId, Currency, DomainEvents, Id, NotificationDeliveries (+6 more)

### Community 25 - "Program.cs"
Cohesion: 0.12
Nodes (6): AcmeIntegreatedAssessement.Api.Components, AcmeIntegreatedAssessement.Api.Components.Account, AcmeIntegreatedAssessement.Infrastructure, AcmeIntegreatedAssessement.Api.Logging, Microsoft.AspNetCore.Components.Authorization, MudBlazor.Services

### Community 26 - "add-entity-sample.razor"
Cohesion: 0.10
Nodes (20): MudButton, MudCard, MudCardContent, MudDivider, MudForm, MudGrid, MudIcon, MudItem (+12 more)

### Community 27 - "edit-entity-sample.razor"
Cohesion: 0.10
Nodes (20): MudButton, MudCard, MudCardContent, MudDivider, MudForm, MudGrid, MudIcon, MudItem (+12 more)

### Community 28 - "SwashbuckleConfiguration.cs"
Cohesion: 0.15
Nodes (3): AcmeIntegreatedAssessement.Api.Filters, AcmeIntegreatedAssessement.Application, AcmeIntegreatedAssessement.Domain.Common.Exceptions

### Community 29 - "IntegrationTestWebAppFactory"
Cohesion: 0.11
Nodes (5): BaseIntegrationTest, WebAppFactory, IntegrationTestWebAppFactory, SharedContainerFixture, AcmeIntegreatedAssessement.Application.IntegrationTests

### Community 31 - "AcmeIntegreatedAssessement.Api"
Cohesion: 0.11
Nodes (19): AcmeIntegreatedAssessement.Api, SonarAnalyzer.CSharp (10.28.0.143324), AspNetCore.HealthChecks.Redis (9.0.0), AspNetCore.HealthChecks.UI.Client (9.0.0), AWSSDK.SecretsManager (4.0.100.3), Bugsnag.AspNet.Core (4.1.1), Finbuckle.MultiTenant.AspNetCore (9.4.10), Microsoft.AspNetCore.Authentication.JwtBearer (10.0.9) (+11 more)

### Community 32 - "MainLayout"
Cohesion: 0.12
Nodes (9): MainLayout, HttpContext, IsDarkMode, JS, _themeService, ThemeService, IsDark, AcmeIntegreatedAssessement.Api.Components.Services (+1 more)

### Community 33 - "NotificationDelivery"
Cohesion: 0.11
Nodes (13): IHasDomainEvent, DomainEvents, NotificationDelivery, AppPaymentId, DomainEvents, Id, IsApplied, NotificationId (+5 more)

### Community 34 - "AddCustomerDialog"
Cohesion: 0.12
Nodes (7): AddCustomerDialog, CategoryLookup, Dialog, Mediator, Model, Snackbar, SubCategoryLookup

### Community 35 - "CustomerEdit"
Cohesion: 0.11
Nodes (9): CustomerEdit, CategoriesLookupModels, CustomerId, HasLoyalty, Mediator, Model, NavigationManager, Snackbar (+1 more)

### Community 36 - "CurrentUserService"
Cohesion: 0.22
Nodes (3): CurrentUser, CurrentUserService, ISetUserContext

### Community 37 - ".Handle"
Cohesion: 0.14
Nodes (3): UnitOfWorkBehaviour, IRedisOmUnitOfWork, IUnitOfWork

### Community 38 - "DomainEvent"
Cohesion: 0.15
Nodes (5): IDomainEventService, DomainEvent, DateOccurred, IsPublished, DomainEventService

### Community 39 - "AcmeReversalNotificationDto"
Cohesion: 0.12
Nodes (14): AcmeReversalNotificationDto, AcmeNotificationTransactionUuid, Amount, AppPaymentId, CurrencyCode, EventType, IsApproved, OccurredAt (+6 more)

### Community 40 - "CustomerAdd"
Cohesion: 0.12
Nodes (8): CustomerAdd, CategoriesLookupModels, HasLoyalty, Mediator, Model, NavigationManager, Snackbar, SubCategoriesLookupModels

### Community 41 - "GetPaymentStateQuery"
Cohesion: 0.12
Nodes (5): QueriesController, IQuery, GetPaymentStateQuery, AppPaymentId, GetPaymentStateQueryValidator

### Community 42 - "IValidatorProvider"
Cohesion: 0.16
Nodes (5): ProcessAcmeApprovedNotificationCommandValidator, ProcessAcmeReversalNotificationCommandValidator, SubmitPaymentCommandValidator, IValidatorProvider, ValidatorProvider

### Community 43 - ".AddApplication"
Cohesion: 0.14
Nodes (3): IValidationService, ValidationService, DependencyInjection

### Community 44 - "ComparisonResult"
Cohesion: 0.15
Nodes (8): CollectionExtensions, ComparisonResult, PossibleEdits, ToAdd, ToRemove, Match, Changed, Original

### Community 46 - "AcmeIntegreatedAssessement.Infrastructure.Services"
Cohesion: 0.15
Nodes (3): ISetCurrentUserContext, AcmeIntegreatedAssessement.Infrastructure.Services, AcmeIntegreatedAssessement.Api.Services

### Community 47 - "StoredCardToken"
Cohesion: 0.14
Nodes (10): StoredCardToken, AcmeToken, CustomerEmail, CustomerId, DomainEvents, Id, Payment, PaymentId (+2 more)

### Community 48 - "AcmeIntegreatedAssessement.Infrastructure"
Cohesion: 0.13
Nodes (15): AcmeIntegreatedAssessement.Infrastructure, SonarAnalyzer.CSharp (10.28.0.143324), Duende.AccessTokenManagement (3.2.0), Finbuckle.MultiTenant (9.4.10), Finbuckle.MultiTenant.EntityFrameworkCore (9.4.10), MassTransit (8.5.7), Microsoft.Data.SqlClient (5.13.0), Microsoft.EntityFrameworkCore (10.0.9) (+7 more)

### Community 49 - "CustomerView"
Cohesion: 0.16
Nodes (6): CustomerView, CustomerByIdModels, CustomerId, Mediator, NavigationManager, Snackbar

### Community 50 - "UpdateCustomerModel"
Cohesion: 0.13
Nodes (11): UpdateCustomerModel, Addresses, CategoryId, Email, Id, IsActive, Loyalty, Name (+3 more)

### Community 51 - "view-entity-sample.razor"
Cohesion: 0.13
Nodes (14): MudButton, MudCard, MudCardContent, MudChip, MudDivider, MudGrid, MudIcon, MudItem (+6 more)

### Community 52 - ".SendAsync"
Cohesion: 0.14
Nodes (3): IAuthService, ServerAuthorizationMessageHandler, AcmeIntegreatedAssessement.Api.Common

### Community 53 - "UserInfo"
Cohesion: 0.14
Nodes (9): AccountLayout, HttpContext, IsDarkTheme, UserInfo, AccessToken, AccessTokenExpiresAt, Email, UserId (+1 more)

### Community 54 - "_Imports.razor"
Cohesion: 0.14
Nodes (9): Microsoft.AspNetCore.Components.Web, AcmeIntegreatedAssessement.Api, Microsoft.AspNetCore.Components.Forms, Microsoft.AspNetCore.Components.Routing, Microsoft.AspNetCore.Components.Web.RenderMode, Microsoft.AspNetCore.Components.Web.Virtualization, Microsoft.AspNetCore.Http, System.Net.Http (+1 more)

### Community 56 - "PaymentChange"
Cohesion: 0.14
Nodes (11): PaymentChange, AcmeNotificationTransactionUuid, Amount, DomainEvents, EventType, Id, IsApproved, OccurredAt (+3 more)

### Community 58 - "ApplicationDbContext"
Cohesion: 0.20
Nodes (5): ApplicationDbContext, Payments, StoredCardTokens, SubMerchants, DesignTimeDbContextFactory

### Community 59 - "UpdateCustomerModel"
Cohesion: 0.14
Nodes (9): UpdateCustomerModel, Addresses, Email, Id, IsActive, Loyalty, Name, Preference (+1 more)

### Community 60 - "CreateCustomerModel"
Cohesion: 0.14
Nodes (10): CreateCustomerModel, Addresses, CategoryId, Email, IsActive, Loyalty, Name, Preference (+2 more)

### Community 62 - "EditCustomerDialog"
Cohesion: 0.15
Nodes (6): EditCustomerDialog, CustomerId, Dialog, Mediator, Model, Snackbar

### Community 63 - "AcmeIntegreatedAssessement.Application"
Cohesion: 0.18
Nodes (12): AcmeIntegreatedAssessement.Application, SonarAnalyzer.CSharp (10.28.0.143324), AcmeIntegreatedAssessement.Domain, SonarAnalyzer.CSharp (10.28.0.143324), Microsoft.NET.Sdk, AutoMapper (14.0.0), FluentValidation.DependencyInjectionExtensions (12.1.1), Intent.RoslynWeaver.Attributes (3.0.0) (+4 more)

### Community 66 - "CreateCustomerModel1"
Cohesion: 0.17
Nodes (8): CreateCustomerModel1, Addresses, Email, IsActive, Loyalty, Name, Preference, Surname

### Community 67 - "Task"
Cohesion: 0.21
Nodes (3): CreateCustomerCommandLoyaltyModel1, LoyaltyNo, Points

### Community 68 - "ProcessAcmeApprovedNotificationCommand"
Cohesion: 0.18
Nodes (9): ProcessAcmeApprovedNotificationCommand, AcmeNotificationTransactionUuid, Amount, AppPaymentId, Body, EventType, IsApproved, OccurredAt (+1 more)

### Community 69 - "ProcessAcmeReversalNotificationCommand"
Cohesion: 0.18
Nodes (9): ProcessAcmeReversalNotificationCommand, AcmeNotificationTransactionUuid, Amount, AppPaymentId, Body, EventType, IsApproved, OccurredAt (+1 more)

### Community 71 - "BaseIntegrationTest"
Cohesion: 0.22
Nodes (6): ProcessAcmeApprovedNotificationTests, ProcessAcmeReversalNotificationTests, SubmitPaymentTests, GetPaymentStateTests, AcmeIntegreatedAssessement.Application.IntegrationTests.Queries, AcmeIntegreatedAssessement.Application.IntegrationTests.Commands

### Community 73 - "app-main-layout-sample.razor"
Cohesion: 0.18
Nodes (10): MainLayoutHeader, MainLayoutProfile, MainLayoutSider, MudDialogProvider, MudLayout, MudMainContent, MudPopoverProvider, MudSnackbarProvider (+2 more)

### Community 74 - "AbstractValidator"
Cohesion: 0.20
Nodes (3): AcmeApprovedNotificationDtoValidator, AcmeReversalNotificationDtoValidator, SubmitPaymentRequestDtoValidator

### Community 75 - "MainLayout.razor"
Cohesion: 0.20
Nodes (9): LayoutComponentBase, MainLayoutHeader, MainLayoutSider, MudDialogProvider, MudLayout, MudMainContent, MudPopoverProvider, MudSnackbarProvider (+1 more)

### Community 79 - "PagedResult"
Cohesion: 0.22
Nodes (6): PagedResult, Data, PageCount, PageNumber, PageSize, TotalCount

### Community 80 - "SubmitPaymentRequestDto"
Cohesion: 0.20
Nodes (10): SubmitPaymentRequestDto, Amount, AppPaymentId, Currency, CurrencyCode, CustomerId, StoredCardToken, SubMerchantId (+2 more)

### Community 82 - "ScopedData"
Cohesion: 0.22
Nodes (3): ScopedData, ActionQueue, Cache

### Community 83 - "app-user-menu-auth-sample.razor"
Cohesion: 0.20
Nodes (9): ChildContent, Authorized, AuthorizeView, MudButton, MudIcon, MudStack, NotAuthorized, Trigger (+1 more)

### Community 84 - "CreateCustomerModel2"
Cohesion: 0.20
Nodes (8): CreateCustomerModel2, Addresses, Email, IsActive, Loyalty, Name, Preference, Surname

### Community 85 - "app-header-auth-sample.razor"
Cohesion: 0.20
Nodes (9): Authorized, AuthorizeView, MainLayoutProfile, MudAppBar, MudButton, MudIconButton, MudSpacer, MudText (+1 more)

### Community 87 - "SubmitPaymentCommand"
Cohesion: 0.22
Nodes (9): SubmitPaymentCommand, Amount, AppPaymentId, Currency, CustomerId, Request, SubMerchantRrn, ZapperMerchantId (+1 more)

### Community 90 - "CreateCustomerModel"
Cohesion: 0.22
Nodes (8): CreateCustomerModel, CategoryId, Email, IsActive, Name, Preference, SubCategoryId, Surname

### Community 91 - "UpdateCustomerCommandAddressesModel"
Cohesion: 0.22
Nodes (7): UpdateCustomerCommandAddressesModel, AddressType, City, Id, Line1, Line2, Postal

### Community 92 - "UpdateCustomerCommandAddressesModel"
Cohesion: 0.22
Nodes (7): UpdateCustomerCommandAddressesModel, AddressType, City, Id, Line1, Line2, Postal

### Community 93 - "AcmeIntegreatedAssessement.Application.UnitTests"
Cohesion: 0.25
Nodes (8): net10.0, AcmeIntegreatedAssessement.Application.UnitTests, Microsoft.NET.Test.Sdk (17.13.0), SonarAnalyzer.CSharp (10.28.0.143324), xunit.runner.visualstudio (3.1.5), xunit.v3 (3.2.2), coverlet.collector (10.0.1), Moq (4.20.72)

### Community 94 - "AwsSecretsManagerOptions"
Cohesion: 0.29
Nodes (6): AwsSecretsManagerOptions, Enabled, Secrets, AwsSecretsManagerSecret, Region, SecretName

### Community 97 - "CreateCustomerCommandAddressesModel"
Cohesion: 0.25
Nodes (6): CreateCustomerCommandAddressesModel, AddressType, City, Line1, Line2, Postal

### Community 98 - "MainLayoutHeader.razor"
Cohesion: 0.29
Nodes (6): MudAppBar, MudButton, MudIconButton, MudSpacer, MudText, ThemeToggle

### Community 99 - "Home.razor"
Cohesion: 0.29
Nodes (6): MudButton, MudIcon, MudContainer, MudLink, PageTitle, route:/

### Community 106 - "DomainEventService.cs"
Cohesion: 0.29
Nodes (3): DomainEventNotification, DomainEvent, AcmeIntegreatedAssessement.Application.Common.Models

### Community 107 - "AcmeIntegreatedAssessement.Application.IntegrationTests"
Cohesion: 0.29
Nodes (7): AcmeIntegreatedAssessement.Application.IntegrationTests, Microsoft.NET.Test.Sdk (17.13.0), SonarAnalyzer.CSharp (10.28.0.143324), xunit.runner.visualstudio (3.1.5), xunit.v3 (3.2.2), AutoFixture (4.18.1), Microsoft.AspNetCore.Mvc.Testing (10.0.9)

### Community 109 - "CreateCustomerCommandAddressesModel1"
Cohesion: 0.29
Nodes (6): CreateCustomerCommandAddressesModel1, AddressType, City, Line1, Line2, Postal

### Community 111 - "Routes.razor"
Cohesion: 0.33
Nodes (5): NotAuthorized, AuthorizeRouteView, FocusOnNavigate, Found, Router

### Community 116 - "Result"
Cohesion: 0.33
Nodes (3): Result, Errors, Succeeded

### Community 117 - "AuthorizeAttribute"
Cohesion: 0.33
Nodes (4): AuthorizeAttribute, Policy, Roles, AcmeIntegreatedAssessement.Application.Common.Security

### Community 118 - "CreateCustomerCommandAddressesModel"
Cohesion: 0.33
Nodes (6): CreateCustomerCommandAddressesModel, AddressType, City, Line1, Line2, Postal

### Community 121 - "UpdateCustomerCommandLoyaltyModel"
Cohesion: 0.33
Nodes (4): UpdateCustomerCommandLoyaltyModel, Id, LoyaltyNo, Points

### Community 123 - "ThemeToggle.razor"
Cohesion: 0.40
Nodes (4): Microsoft.AspNetCore.Components.Web, MudIconButton, MudTooltip, MouseEventArgs

### Community 125 - "ICurrentUser"
Cohesion: 0.40
Nodes (4): ICurrentUser, Id, Name, Principal

### Community 126 - "PaymentStateResponseDto"
Cohesion: 0.40
Nodes (5): PaymentStateResponseDto, Amount, AppPaymentId, Currency, Status

### Community 127 - "app-sider-auth-sample.razor"
Cohesion: 0.40
Nodes (4): Authorized, AuthorizeView, MudNavLink, MudNavMenu

### Community 128 - "UpdateCustomerCommandLoyaltyModel"
Cohesion: 0.40
Nodes (4): UpdateCustomerCommandLoyaltyModel, Id, LoyaltyNo, Points

### Community 129 - ".OnInitializedAsync"
Cohesion: 0.40
Nodes (3): CreateCustomerPreferenceModel, NewsLetter, Specials

### Community 132 - "UpdateCustomerPreferenceModel"
Cohesion: 0.50
Nodes (4): UpdateCustomerPreferenceModel, Id, NewsLetter, Specials

### Community 133 - "UpdateCustomerPreferenceModel"
Cohesion: 0.50
Nodes (4): UpdateCustomerPreferenceModel, Id, NewsLetter, Specials

### Community 140 - "CreateCustomerCommandLoyaltyModel"
Cohesion: 0.67
Nodes (3): CreateCustomerCommandLoyaltyModel, LoyaltyNo, Points

### Community 141 - "CreateCustomerPreferenceModel"
Cohesion: 0.67
Nodes (3): CreateCustomerPreferenceModel, NewsLetter, Specials

### Community 142 - "CreateCustomerPreferenceModel1"
Cohesion: 0.67
Nodes (3): CreateCustomerPreferenceModel1, NewsLetter, Specials

### Community 143 - "CreateCustomerPreferenceModel2"
Cohesion: 0.67
Nodes (3): CreateCustomerPreferenceModel2, NewsLetter, Specials

### Community 144 - "CreateCustomerCommandLoyaltyModel"
Cohesion: 0.67
Nodes (3): CreateCustomerCommandLoyaltyModel, LoyaltyNo, Points

## Knowledge Gaps
- **643 isolated node(s):** `Home`, `MainLayoutHeader`, `MainLayoutSider`, `IBypassPipelineValidation`, `AuditActor` (+638 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 1126 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **50 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Intent.RoslynWeaver.Attributes` connect `Intent.RoslynWeaver.Attributes` to `RepositoryBase`, `RedisOmRepositoryBase`, `MassTransitMessageBus`, `SubMerchant`, `AcmeIntegreatedAssessement.Application.Common.Interfaces`, `Home.razor.cs`, `AcmeIntegreatedAssessement.Infrastructure/DependencyInjection.cs`, `AcmeTransactionNotification`, `edit-entity-sample.razor.cs`, `AcmeIntegreatedAssessement.Domain.Repositories`, `AccountLayout.razor`, `AcmeIntegreatedAssessement.Domain.Entities`, `Program.cs`, `SwashbuckleConfiguration.cs`, `IntegrationTestWebAppFactory`, `MainLayout`, `NotificationDelivery`, `.Handle`, `DomainEvent`, `ComparisonResult`, `AcmeIntegreatedAssessement.Infrastructure.Services`, `.SendAsync`, `UserInfo`, `_Imports.razor`, `IStoredCardTokenRepository`, `ValueObject`, `BaseIntegrationTest`, `OpenTelemetryConfiguration.cs`, `HealthChecksConfiguration.cs`, `AwsSecretsManagerOptions`, `SignalRConfiguration`, `AwsSecretsManagerConfigurationSource.cs`, `AuthorizationBehaviour.cs`, `DomainEventService.cs`, `.CreateOrUpdateCollection`, `AcmeIntegreatedAssessement.Api.Configuration`, `BugsnagConfiguration.cs`, `AuthorizeAttribute`?**
  _High betweenness centrality (0.606) - this node is a cross-community bridge._
- **Why does `CustomerEdit` connect `CustomerEdit` to `UpdateCustomerPreferenceModel`, `Guid`, `UpdateCustomerModel`, `edit-entity-sample.razor.cs`, `UpdateCustomerCommandLoyaltyModel`, `UpdateCustomerCommandAddressesModel`?**
  _High betweenness centrality (0.067) - this node is a cross-community bridge._
- **Why does `UI.AI.Samples.Api.Components.Pages.Dialogs.Customers` connect `edit-entity-sample.razor.cs` to `edit-entity-dialog-sample.razor`, `add-entity-dialog-sample.razor`?**
  _High betweenness centrality (0.056) - this node is a cross-community bridge._
- **What connects `Home`, `MainLayoutHeader`, `MainLayoutSider` to the rest of the system?**
  _643 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `RepositoryBase` be split into smaller, more focused modules?**
  _Cohesion score 0.09000584453535944 - nodes in this community are weakly interconnected._
- **Should `BoundedLoggingDestructuringPolicy` be split into smaller, more focused modules?**
  _Cohesion score 0.055904961565338925 - nodes in this community are weakly interconnected._
- **Should `RedisOmRepositoryBase` be split into smaller, more focused modules?**
  _Cohesion score 0.07755102040816327 - nodes in this community are weakly interconnected._