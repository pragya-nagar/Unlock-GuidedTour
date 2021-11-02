using System.Threading.Tasks;

namespace GuidedTour.EF.Entity
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<IOperationStatus> SaveChangesAsync();
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IObjectState;
    }
}
