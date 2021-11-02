using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GuidedTour.Common;
using GuidedTour.EF.Entity;
using GuidedTour.Service.Contracts;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GuidedTour.Service
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseService : IBaseService
    {
        public IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        public IOperationStatus OperationStatus { get; set; }
       public GuidedTourDbContext GuidedTourDbContext { get; set; }
        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        protected ILogger Logger { get; private set; }

        protected IMapper Mapper { get; private set; }

        protected HttpContext HttpContext => new HttpContextAccessor().HttpContext;
        protected string LoggedInUserEmail => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        protected string UserToken => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "token")?.Value;
        protected bool IsTokenActive => (!string.IsNullOrEmpty(LoggedInUserEmail) && !string.IsNullOrEmpty(UserToken));

        public string ConnectionString
        {
            get => GuidedTourDbContext?.Database.GetDbConnection().ConnectionString;
            set
            {
                if (GuidedTourDbContext != null)
                    GuidedTourDbContext.Database.GetDbConnection().ConnectionString = value;
            }
        }


        protected BaseService(IServicesAggregator servicesAggregateService)
        {
            UnitOfWorkAsync = servicesAggregateService.UnitOfWorkAsync;
            GuidedTourDbContext = UnitOfWorkAsync.DataContext as GuidedTourDbContext;
            OperationStatus = servicesAggregateService.OperationStatus;
            Configuration = servicesAggregateService.Configuration;
            HostingEnvironment = servicesAggregateService.HostingEnvironment;
            Logger = Log.Logger;
            Mapper = servicesAggregateService.Mapper;

        }
        public HttpClient GetHttpClient(string jwtToken)
        {          
            var hasTenant = HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if ((!hasTenant && HttpContext.Request.Host.Value.Contains("localhost")))
                tenantId = Configuration.GetValue<string>("TenantId");
            string domain;
            var hasOrigin = HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && HttpContext.Request.Host.Value.Contains("localhost"))
                domain = Configuration.GetValue<string>("FrontEndUrl").ToString();
            else
                domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("User:BaseUrl"))
            };
            var token = !string.IsNullOrEmpty(jwtToken) ? jwtToken : UserToken;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            httpClient.DefaultRequestHeaders.Add("TenantId", tenantId.ToString());
            httpClient.DefaultRequestHeaders.Add("OriginHost", domain);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }
       
        #region Rijndael Encryption
        public string DecryptRijndael(string cipherinput, string salt)
        {
            if (string.IsNullOrEmpty(cipherinput))
                throw new ArgumentNullException("cipherinput");
            if (!IsBase64String(cipherinput))
                throw new FormatException("The cipherText input parameter is not base64 encoded");
            string text;
            var aesAlg = NewRijndaelManaged(salt);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(cipherinput);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            text = srDecrypt.ReadToEnd();

            return text;
        }
        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            string InputKey = "99334E81-342C-4900-86D9-07B7B9FE5EBB";
            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(InputKey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
            return aesAlg;
        }
        private static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, Constants.Base64Regex, RegexOptions.None);

        }
        #endregion

    }
}
