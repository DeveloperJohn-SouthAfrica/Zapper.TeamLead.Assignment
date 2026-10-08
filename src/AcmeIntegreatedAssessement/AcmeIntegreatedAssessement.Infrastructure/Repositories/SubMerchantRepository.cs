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
    public class SubMerchantRepository : RepositoryBase<SubMerchant, SubMerchant, ApplicationDbContext>, ISubMerchantRepository
    {
        private readonly ISqlStoredProcedureInvoker _sqlInvoker;

        public SubMerchantRepository(ApplicationDbContext dbContext, IMapper mapper, ISqlStoredProcedureInvoker sqlInvoker) : base(dbContext, mapper)
        {
            _sqlInvoker = sqlInvoker;
        }

        public async Task<SubMerchant?> GetByZapperMerchantIdAsync(string zapperMerchantId, CancellationToken cancellationToken = default)
        {
            return await _sqlInvoker.QuerySingleAsync(
                "GetSubMerchantByZapperMerchantId",
                reader => new SubMerchant
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    ZapperMerchantId = reader.GetString(reader.GetOrdinal("ZapperMerchantId")),
                    SubMerchantAltId = reader.GetString(reader.GetOrdinal("ZapperMerchantId")),
                    TradingName = reader.GetString(reader.GetOrdinal("TradingName")),
                    City = reader.GetString(reader.GetOrdinal("City")),
                    Region = reader.GetString(reader.GetOrdinal("Region")),
                    Country = reader.GetString(reader.GetOrdinal("Country")),
                    MerchantCategoryCode = reader.GetString(reader.GetOrdinal("MerchantCategoryCode"))
                },
                parameters => parameters.Add("@ZapperMerchantId", System.Data.SqlDbType.NVarChar, 100).Value = zapperMerchantId,
                cancellationToken);
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