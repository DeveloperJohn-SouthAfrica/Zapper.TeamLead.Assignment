using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IStoredCardTokenRepository : IEFRepository<StoredCardToken, StoredCardToken>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((Guid Id, string StoredCardTokenId) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<StoredCardToken?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<StoredCardToken?> FindByIdAsync(Guid id, Func<IQueryable<StoredCardToken>, IQueryable<StoredCardToken>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<StoredCardToken>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
        Task<StoredCardToken?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    }
}