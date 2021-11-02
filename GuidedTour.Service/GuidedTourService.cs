using GuidedTour.EF.Entity;
using GuidedTour.Service.Contracts;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GuidedTour.Service
{
    public class GuidedTourService : BaseService, IGuidedTourService
    {
        private readonly ICommonService commonService;
        private readonly IRepositoryAsync<GuidedTour.EF.Entity.GuidedTour> guidedRepo;
        private readonly IOnBoardingService onBoardingService;
        private readonly IRepositoryAsync<GuidedTour.EF.Entity.GuidedTourControl> guidedTourControlRepo;
        public GuidedTourService(IServicesAggregator servicesAggregateService, ICommonService commonServices, IOnBoardingService onBoardingServices) : base(servicesAggregateService)
        {
            commonService = commonServices;
            guidedRepo = UnitOfWorkAsync.RepositoryAsync<GuidedTour.EF.Entity.GuidedTour>();
            onBoardingService = onBoardingServices;
            guidedTourControlRepo = UnitOfWorkAsync.RepositoryAsync<GuidedTour.EF.Entity.GuidedTourControl>();
        }
        public async Task<List<GuidedTourResponse>> GetGuidedTour()
        {
            List<GuidedTourResponse> guidedList = new List<GuidedTourResponse>();
            var guidedTourDetails = await guidedRepo.GetQueryable().ToListAsync();

            if (guidedTourDetails.Count > 0)
            {
                foreach (var itemResult in guidedTourDetails)
                {
                    var guided = new GuidedTourResponse
                    {
                        GuidedId = itemResult.Id,
                        GuidedTourName = itemResult.Name
                    };
                    guidedList.Add(guided);
                }
            }
            return guidedList;
        }
        //public async Task<GuidedTourAnyActivityStatusResponse> IsToShowGuidedTour(bool isAnyActivity, UserIdentity identity)
        //{
        //    var isAnyOkr = await commonService.IsAnyOkr(UserToken);
        //    var activitydResponse = new GuidedTourAnyActivityStatusResponse();
        //    var onboardingControlDetails = await onBoardingService.OnBoardingControlDetailById(identity);
        //    if (onboardingControlDetails != null)
        //    {
        //        if (isAnyActivity)
        //        {
        //            activitydResponse.ToShowGuidedTour = false;
        //        }
        //        else
        //        {
        //            if ((onboardingControlDetails.SkipCount == 2 || onboardingControlDetails.ReadyCount == 2)
        //                || (onboardingControlDetails.SkipCount + onboardingControlDetails.ReadyCount) == 2)
        //            {
        //                activitydResponse.ToShowGuidedTour = false;
        //            }
        //            else if ((onboardingControlDetails.SkipCount > 0 || onboardingControlDetails.ReadyCount > 0
        //                || (onboardingControlDetails.SkipCount + onboardingControlDetails.ReadyCount) > 0 &&
        //                (onboardingControlDetails.EmployeeId != 0))
        //                && (isAnyOkr))
        //            {
        //                activitydResponse.ToShowGuidedTour = true;
        //            }
        //        }
        //    }
        //    return activitydResponse;
        //}

        public async Task<GuidedTourAnyActivityStatusResponse> IsToShowGuidedTour(UserIdentity identity)
        {           
            bool isAnyOkr = await commonService.IsAnyOkr(UserToken);
            var activitydResponse = new GuidedTourAnyActivityStatusResponse();
            activitydResponse.ToShowGuidedTour = true;
            var onboardingControlDetails = await onBoardingService.OnBoardingControlDetailById(identity);
            if (onboardingControlDetails != null)
            {
                if (((onboardingControlDetails.SkipCount >= 1 || onboardingControlDetails.ReadyCount >= 1)
                    || ((onboardingControlDetails.SkipCount + onboardingControlDetails.ReadyCount) >= 1)) && (isAnyOkr))
                {

                    var employeeDetails = guidedTourControlRepo.GetQueryable()
                        .FirstOrDefault(x => x.EmployeeId == identity.EmployeeId && x.IsActive);
                    if (employeeDetails != null && employeeDetails.Count > 0)
                    {
                        if (employeeDetails.Count >= 1)
                        {
                            employeeDetails.Count = employeeDetails.Count + 1;
                            employeeDetails.UpdatedBy = identity.EmployeeId;
                            employeeDetails.UpdatedOn = DateTime.UtcNow;
                            guidedTourControlRepo.Update(employeeDetails);
                            UnitOfWorkAsync.SaveChanges();
                            activitydResponse.ToShowGuidedTour = false;
                        }                        

                    }
                    else
                    {
                        var guideTour = new GuidedTourControl()
                        {
                            Count = 1,
                            EmployeeId = identity.EmployeeId,
                            CreatedBy = identity.EmployeeId,
                            CreatedOn = DateTime.UtcNow,
                            IsActive = true

                        };
                        guidedTourControlRepo.Add(guideTour);
                        UnitOfWorkAsync.SaveChanges();
                        activitydResponse.ToShowGuidedTour = true;
                    }



                }
                else
                {
                    activitydResponse.ToShowGuidedTour = false;
                }

            }
            else
            {
                activitydResponse.ToShowGuidedTour = false;
            }
            return activitydResponse;
        }


    }
}