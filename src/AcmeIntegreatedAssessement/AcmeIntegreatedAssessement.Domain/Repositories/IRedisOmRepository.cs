using System.Linq.Expressions;
using AcmeIntegreatedAssessement.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmRepositoryInterface", Version = "1.0")]

namespace AcmeIntegreatedAssessement.Domain.Repositories
{
    public interface IRedisOmRepository<TDomain, TDocumentInterface> : IRepository<TDomain>
    {
        IRedisOmUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}