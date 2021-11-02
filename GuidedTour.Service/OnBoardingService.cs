using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuidedTour.Common;
using GuidedTour.EF.Entity;
using GuidedTour.Service.Contracts;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;
using Microsoft.EntityFrameworkCore;

namespace GuidedTour.Service
{
    public class OnBoardingService : BaseService, IOnBoardingService
    {

        private readonly ICommonService commonService;
        private readonly IRepositoryAsync<GuidedTour.EF.Entity.OnBoardingControl> onBoardingControlRepositoryAsync;
        private readonly IRepositoryAsync<GuidedTour.EF.Entity.OnBoardingScreen> onBoardingScreenRepositoryAsync;

        public OnBoardingService(IServicesAggregator servicesAggregateService, ICommonService commonServices) : base(servicesAggregateService)
        {
            commonService = commonServices;
            onBoardingControlRepositoryAsync = UnitOfWorkAsync.RepositoryAsync<GuidedTour.EF.Entity.OnBoardingControl>();
            onBoardingScreenRepositoryAsync = UnitOfWorkAsync.RepositoryAsync<GuidedTour.EF.Entity.OnBoardingScreen>();
        }


        /// <summary>
        /// Method to save data against new user added into the system for maintaining skipCount and ReadyCount for onboarding
        /// </summary>
        /// <param name="onBoardingRequest"></param>
        /// <returns></returns>
        public async Task<bool> AddOnBoardingDataAsync(OnBoardingRequest onBoardingRequest)
        {
            var onBoardingControl = new OnBoardingControl()
            {
                EmployeeId = onBoardingRequest.EmployeeId,
                SkipCount = 0,
                ReadyCount = 0,
                CreatedBy = onBoardingRequest.CreatedBy,
                CreatedOn = onBoardingRequest.CreatedOn,
                IsActive = true
            };

            onBoardingControlRepositoryAsync.Add(onBoardingControl);
            await UnitOfWorkAsync.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Returns all the static data for all the screens
        /// </summary>
        /// <returns></returns>
        public async Task<List<OnBoardingSlidesResponse>> GetOnBoardingDataAsync()
        {
            var onBoardingDataList = new List<OnBoardingSlidesResponse>();
            var onBoardingData = await onBoardingScreenRepositoryAsync.GetAllAsync();
            var onBoardingSortedData = onBoardingData.GroupBy(x => x.PageId).ToList();
            foreach (var item in onBoardingSortedData)
            {
                var onBoardingDataByPageId = onBoardingData.Where(x => x.PageId == item.Key).ToList();
                OnBoardingSlidesResponse onBoardingSlidesResponse = new OnBoardingSlidesResponse();

                var button = string.Empty;
                foreach (var data in onBoardingDataByPageId)
                {
                    onBoardingSlidesResponse.Id = data.ScreenId;
                    onBoardingSlidesResponse.PageId = data.PageId;
                    onBoardingSlidesResponse.IsMainPage = data.Flow == 0 ? true : false;
                    onBoardingSlidesResponse.AlreadyKnow = data.Flow == 2 ? true : false;
                    onBoardingSlidesResponse.Video =
                        data.ControlType.Equals(ControlType.Video.ToString()) ? data.ControlValue : "";
                    if (data.ControlType.Equals(ControlType.Body.ToString()))
                    {
                        var body = data.ControlValue;
                        string[] sentences = body.Split(new char[] { '~' });
                        sentences = sentences.SkipLast(1).ToArray();
                        onBoardingSlidesResponse.Body = sentences;
                    }
                    if (data.ControlType.Equals(ControlType.Button.ToString()))
                    {
                        button = button + data.ControlValue;
                        string[] sentences = button.Split(new char[] { '~' });
                        sentences = sentences.SkipLast(1).ToArray();
                        onBoardingSlidesResponse.Button = sentences;
                    }

                }

                onBoardingDataList.Add(onBoardingSlidesResponse);
            }

            return onBoardingDataList;

        }

        /// <summary>
        /// Will Update skip count and Readcount on the basis of employeeid
        /// </summary>
        /// <param name="updateOnBoardingCountRequest"></param>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public async Task<UpdateOnboardingCountResponse> UpdateOnBoardingDataAsync(UpdateOnBoardingCountRequest updateOnBoardingCountRequest, UserIdentity userIdentity)
        {
            var updatedResponse = new UpdateOnboardingCountResponse();
            var employeeDetails = onBoardingControlRepositoryAsync.GetQueryable()
                .FirstOrDefault(x => x.EmployeeId == updateOnBoardingCountRequest.EmployeeId && x.IsActive);
            if (employeeDetails != null)
            {
                if (updateOnBoardingCountRequest.ActionType == (int)ActionType.Skip)
                {
                    employeeDetails.SkipCount = employeeDetails.SkipCount + 1;
                    employeeDetails.UpdatedBy = userIdentity.EmployeeId;
                    employeeDetails.UpdatedOn = DateTime.UtcNow;

                }
                else if (updateOnBoardingCountRequest.ActionType == (int)ActionType.Ready)
                {

                    employeeDetails.ReadyCount = employeeDetails.ReadyCount + 1;
                    employeeDetails.UpdatedBy = userIdentity.EmployeeId;
                    employeeDetails.UpdatedOn = DateTime.UtcNow;

                }

                onBoardingControlRepositoryAsync.Update(employeeDetails);
                UnitOfWorkAsync.SaveChanges();

                updatedResponse.EmployeeId = employeeDetails.EmployeeId;
                updatedResponse.SkipCount = employeeDetails.SkipCount;
                updatedResponse.ReadyCount = employeeDetails.ReadyCount;
            }
            return updatedResponse;
        }

        /// <summary>
        /// Will Return skipCount and ReadCount against any employeeId 
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public async Task<OnBoardingControlResponse> OnBoardingControlDetailById(UserIdentity userIdentity)
        {
            var employeeDetails = await onBoardingControlRepositoryAsync.GetQueryable()
                .FirstOrDefaultAsync(x => x.EmployeeId == userIdentity.EmployeeId && x.IsActive);
            OnBoardingControlResponse onBoardingControlResponse = new OnBoardingControlResponse();
            if (employeeDetails != null)
            {
                onBoardingControlResponse.Id = employeeDetails.Id;
                onBoardingControlResponse.EmployeeId = employeeDetails.EmployeeId;
                onBoardingControlResponse.SkipCount = employeeDetails.SkipCount;
                onBoardingControlResponse.ReadyCount = employeeDetails.ReadyCount;

            }

            //var onBoardingControlCountDetails = Mapper.Map<OnBoardingControlResponse>(employeeDetails);
            return onBoardingControlResponse;
        }
    }
}


