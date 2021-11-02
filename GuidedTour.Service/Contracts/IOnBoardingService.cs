
using System.Collections.Generic;
using System.Threading.Tasks;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;

namespace GuidedTour.Service.Contracts
{
    public interface IOnBoardingService
    {
        Task<bool> AddOnBoardingDataAsync(OnBoardingRequest onBoardingRequest);
        Task<List<OnBoardingSlidesResponse>> GetOnBoardingDataAsync();
        Task<OnBoardingControlResponse> OnBoardingControlDetailById(UserIdentity userIdentity);
        Task<UpdateOnboardingCountResponse> UpdateOnBoardingDataAsync(UpdateOnBoardingCountRequest updateOnBoardingCountRequest, UserIdentity userIdentity);

    }
}
