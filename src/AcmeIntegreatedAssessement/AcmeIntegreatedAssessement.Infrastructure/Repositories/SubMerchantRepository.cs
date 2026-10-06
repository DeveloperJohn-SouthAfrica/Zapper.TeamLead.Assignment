using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Domain.Repositories;
using AcmeIntegreatedAssessement.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SubMerchantRepository : RepositoryBase<SubMerchant, SubMerchant, ApplicationDbContext>, ISubMerchantRepository
    {

        public SubMerchantRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (Guid Id, string SubMerchantAltId) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id.Id && x.SubMerchantAltId == id.SubMerchantAltId, cancellationToken);
        }

        public async Task<SubMerchant?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<SubMerchant?> FindByIdAsync(
            Guid id,
            Func<IQueryable<SubMerchant>, IQueryable<SubMerchant>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<SubMerchant>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}