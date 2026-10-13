using AcmeIntegreatedAssessement.Domain.Entities;
using AcmeIntegreatedAssessement.Domain.Repositories;
using AcmeIntegreatedAssessement.Infrastructure.Persistence;
using AcmeIntegreatedAssessement.Infrastructure.Data;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StoredCardTokenRepository : RepositoryBase<StoredCardToken, StoredCardToken, ApplicationDbContext>, IStoredCardTokenRepository
    {
        private readonly ISqlStoredProcedureInvoker _sqlInvoker;

        public StoredCardTokenRepository(ApplicationDbContext dbContext, IMapper mapper, ISqlStoredProcedureInvoker sqlInvoker) : base(dbContext, mapper)
        {
            _sqlInvoker = sqlInvoker;
        }

        public async Task<StoredCardToken?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _sqlInvoker.QuerySingleAsync(
                "GetStoredCardTokenByCustomerId",
                reader => new StoredCardToken
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    StoredCardTokenId = reader.GetString(reader.GetOrdinal("StoredCardTokenId")),
                    AcmeToken = reader.GetString(reader.GetOrdinal("AcmeToken")),
                    CustomerId = reader.GetString(reader.GetOrdinal("CustomerId")),
                    CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail"))
                },
                parameters => parameters.Add("@CustomerId", System.Data.SqlDbType.NVarChar, 100).Value = customerId,
                cancellationToken);
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (Guid Id, string StoredCardTokenId) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id.Id && x.StoredCardTokenId == id.StoredCardTokenId, cancellationToken);
        }

        public async Task<StoredCardToken?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<StoredCardToken?> FindByIdAsync(
            Guid id,
            Func<IQueryable<StoredCardToken>, IQueryable<StoredCardToken>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<StoredCardToken>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}