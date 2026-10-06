using AcmeIntegreatedAssessement.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISubMerchantRepository : IEFRepository<SubMerchant, SubMerchant>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((Guid Id, string SubMerchantAltId) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<SubMerchant?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<SubMerchant?> FindByIdAsync(Guid id, Func<IQueryable<SubMerchant>, IQueryable<SubMerchant>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<SubMerchant>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}