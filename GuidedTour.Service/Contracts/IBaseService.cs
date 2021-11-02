using System.Net.Http;
using GuidedTour.EF;
using GuidedTour.EF.Entity;

namespace GuidedTour.Service.Contracts
{
    public interface IBaseService
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
       GuidedTourDbContext GuidedTourDbContext { get; set; }
       string ConnectionString { get; set; }
        HttpClient GetHttpClient(string jwtToken);

    }
}
