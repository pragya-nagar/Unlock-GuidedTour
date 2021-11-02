using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GuidedTour.Common;
using GuidedTour.EF.Entity;
using GuidedTour.Service.Contracts;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;

namespace GuidedTour.WebCore.Controller
{
    public class OnBoardingController : ApiControllerBase
    {
        private readonly IOnBoardingService onBoardingService;
        private readonly ICommonService commonService;
        public OnBoardingController(IOnBoardingService onBoardingServices, ICommonService commonServices)
        {
            onBoardingService = onBoardingServices;
            commonService = commonServices;
        }


        [Route("OnBoarding")]
        [HttpGet]
        public async Task<IActionResult> GetOnBoardingDataAsync()
        {
            var payloadOutputSave = new PayloadCustom<List<OnBoardingSlidesResponse>>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                payloadOutputSave.Entity = await onBoardingService.GetOnBoardingDataAsync();
                if (payloadOutputSave.Entity != null)
                {
                    payloadOutputSave.MessageType = MessageType.Success.ToString();
                    payloadOutputSave.IsSuccess = true;
                    payloadOutputSave.Status = (int)HttpStatusCode.OK;
                }
            }
            else
            {
                payloadOutputSave = GetPayloadStatus(payloadOutputSave);
            }

            return Ok(payloadOutputSave);
        }

        [Route("OnBoardingActions")]
        [HttpPost]
        public async Task<IActionResult> AddOnBoardingDataAsync(OnBoardingRequest onBoardingRequest)
        {
            var payloadOutputSave = new PayloadCustom<bool>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                payloadOutputSave.IsSuccess = await onBoardingService.AddOnBoardingDataAsync(onBoardingRequest);
                if (payloadOutputSave.IsSuccess)
                {
                    payloadOutputSave.MessageType = MessageType.Success.ToString();
                    payloadOutputSave.IsSuccess = true;
                    payloadOutputSave.Status = (int)HttpStatusCode.OK;
                }
            }
            else
            {
                payloadOutputSave = GetPayloadStatus(payloadOutputSave);
            }

            return Ok(payloadOutputSave);
        }

        [Route("OnBoardingActionsCount")]
        [HttpPut]
        public async Task<IActionResult> UpdateOnBoardingDataAsync(UpdateOnBoardingCountRequest onBoardingCountRequest)
        {
            var payloadOutputSave = new PayloadCustom<UpdateOnboardingCountResponse>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                payloadOutputSave.Entity = await onBoardingService.UpdateOnBoardingDataAsync(onBoardingCountRequest, loginUserMyGoalAsync);
                if (payloadOutputSave.Entity!=null)
                {
                    payloadOutputSave.MessageType = MessageType.Success.ToString();
                    payloadOutputSave.IsSuccess = true;
                    payloadOutputSave.Status = (int)HttpStatusCode.OK;
                }
            }
            else
            {
                payloadOutputSave = GetPayloadStatus(payloadOutputSave);
            }

            return Ok(payloadOutputSave);
        }

        [Route("OnBoardingControlDetailById")]
        [HttpGet]
        public async Task<IActionResult> OnBoardingControlDetailById()
        {
            var payloadOutputSave = new PayloadCustom<OnBoardingControlResponse>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                payloadOutputSave.Entity = await onBoardingService.OnBoardingControlDetailById(loginUserMyGoalAsync);
                if (payloadOutputSave.Entity != null)
                {
                    payloadOutputSave.MessageType = MessageType.Success.ToString();
                    payloadOutputSave.IsSuccess = true;
                    payloadOutputSave.Status = (int)HttpStatusCode.OK;
                }
            }
            else
            {
                payloadOutputSave = GetPayloadStatus(payloadOutputSave);
            }

            return Ok(payloadOutputSave);
        }
    }
}
