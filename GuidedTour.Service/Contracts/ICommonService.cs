using GuidedTour.EF;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GuidedTour.Service.Contracts
{
    public interface ICommonService
    {
        Task<UserIdentity> GetUserIdentity(string jwtToken);
        Task<bool> IsAnyOkr(string jwtToken = null);
    }

}
