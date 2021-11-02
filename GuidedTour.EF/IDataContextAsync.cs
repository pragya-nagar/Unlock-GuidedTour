
using System.Threading;
using System.Threading.Tasks;

namespace GuidedTour.EF.Entity
{
    public interface IDataContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
