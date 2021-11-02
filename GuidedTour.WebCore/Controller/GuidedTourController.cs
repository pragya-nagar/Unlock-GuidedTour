using GuidedTour.Common;
using GuidedTour.EF.Entity;
using GuidedTour.Service.Contracts;
using GuidedTour.ViewModel.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace GuidedTour.WebCore.Controller
{
    public class GuidedTourController : ApiControllerBase
    {
        private readonly IGuidedTourService guidedTourService;
        private readonly ICommonService commonService;

        public GuidedTourController(IGuidedTourService GuidedTourService, ICommonService commonServices)
        {
            guidedTourService = GuidedTourService;
            commonService = commonServices;

        }

        [Route("GuidedTour")]
        [HttpGet]
        public async Task<IActionResult> GuidedToursAsync()
        {
            var payloadOutputSave = new PayloadCustom<List<GuidedTourResponse>>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {
                payloadOutputSave.Entity = await guidedTourService.GetGuidedTour();
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

        [Route("GuidedTourAnyActivity")]
        [HttpPut]
        public async Task<IActionResult> GuidedTourAnyActivityStatusAsync()
        {
            var payloadOutputSave = new PayloadCustom<GuidedTourAnyActivityStatusResponse>();
            var loginUserMyGoalAsync = await commonService.GetUserIdentity(UserToken);
            if (loginUserMyGoalAsync == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                payloadOutputSave.Entity = await guidedTourService.IsToShowGuidedTour(loginUserMyGoalAsync);

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