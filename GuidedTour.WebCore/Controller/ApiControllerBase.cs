using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GuidedTour.EF;
using Serilog;
using System.Net;
using GuidedTour.EF.Entity;

namespace GuidedTour.WebCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected ILogger Logger { get; set; }
        public ApiControllerBase()
        {
            Logger = Log.Logger;
        }

        protected string LoggedInUserEmail => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;

        protected string UserToken => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "token")?.Value;

        protected bool IsActiveToken => (!string.IsNullOrEmpty(LoggedInUserEmail) && !string.IsNullOrEmpty(UserToken));


        public PayloadCustom<T> GetPayloadStatus<T>(PayloadCustom<T> payload)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {

                    payload.MessageList.Add(state.Key, error.ErrorMessage);
                }
            }
            payload.IsSuccess = false;
            payload.Status = (int)HttpStatusCode.BadRequest;
            return payload;
        }
    }
}
