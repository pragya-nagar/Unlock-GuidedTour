using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using GuidedTour.Common;
using GuidedTour.EF;
using GuidedTour.Service.Contracts;
using GuidedTour.ViewModel.Request;
using GuidedTour.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GuidedTour.EF.Entity;

namespace GuidedTour.Service
{
    [ExcludeFromCodeCoverage]
    public class CommonService : BaseService, ICommonService
    {
        public CommonService(IServicesAggregator servicesAggregateService) : base(servicesAggregateService)
        {
          
        }
        public async Task<UserIdentity> GetUserIdentity(string jwtToken)
        {
            UserIdentity loginUserDetail = new UserIdentity();
            if (jwtToken != "")
            {
                using var httpClient = GetHttpClient(jwtToken);
                using var response = await httpClient.PostAsync($"api/User/Identity", new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<PayloadCustom<UserIdentity>>(apiResponse);
                    loginUserDetail = user.Entity;
                }
            }
            return loginUserDetail;
        }
        public async Task<bool> IsAnyOkr(string jwtToken = null)
        {
            PayloadCustom<bool> payload = new PayloadCustom<bool>();
            using var httpClient = GetHttpClient(jwtToken);
            httpClient.BaseAddress = new Uri(Configuration.GetValue<string>("OkrService:BaseUrl"));
            using var response = await httpClient.GetAsync($"api/Dashboard/IsAnyOkr");
            if (response.IsSuccessStatusCode)
            {                
                payload = JsonConvert.DeserializeObject<PayloadCustom<bool>>(await response.Content.ReadAsStringAsync());
            }
            return payload.Entity;
        }
    }
}
