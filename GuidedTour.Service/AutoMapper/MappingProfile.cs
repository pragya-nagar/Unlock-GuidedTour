using AutoMapper;
using GuidedTour.EF.Entity;
using GuidedTour.ViewModel.Response;

namespace GuidedTour.Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OnBoardingControlResponse, OnBoardingControl>();
        }

    }
}
