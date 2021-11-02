using System.Collections.Generic;
using System.Threading.Tasks;
using GuidedTour.EF.Entity;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;

namespace GuidedTour.Service.Contracts
{
    public interface IGuidedTourService
    {
        Task<List<GuidedTourResponse>> GetGuidedTour();
        Task<GuidedTourAnyActivityStatusResponse> IsToShowGuidedTour(UserIdentity identity);
    }

}
