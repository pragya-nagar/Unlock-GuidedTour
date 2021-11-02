using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using GuidedTour.EF;
using GuidedTour.Service;
using GuidedTour.Service.AutoMapper;
using GuidedTour.Service.Contracts;
using GuidedTour.WebCore.Filter;
using GuidedTour.WebCore.Middleware;
using Serilog;
using Serilog.Events;
using GuidedTour.EF.Entity;
using GuidedTour.ViewModel.Response;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using GuidedTour.Common;
using Polly;

namespace GuidedTour.WebCore
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public static IWebHostEnvironment AppEnvironment { get; private set; }
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            AppEnvironment = env;
            Configuration = configuration;
            var envName = env?.EnvironmentName;
            Console.Write("********* Environment :" + envName);
            Console.Write("AppEnvironment- " + AppEnvironment?.EnvironmentName);
            Log.Logger = new LoggerConfiguration()
               .Enrich.WithProperty("Environment", envName)
               .Enrich.WithMachineName()
               .Enrich.WithProcessId()
               .Enrich.WithThreadId()
               .WriteTo.Console()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.Write("ConfigureServices called ");
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddNewtonsoftJson(opt =>
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddLogging();
            var keyVault = new DatabaseVaultResponse();
            services.AddDbContext<GuidedTourDbContext>((serviceProvider, options) =>
            {
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor != null)
                {
                    var httpContext = httpContextAccessor.HttpContext;                                   
                    var hasTenant = httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
                    if ((!hasTenant && httpContext.Request.Host.Value.Contains("localhost")))
                        tenantId = Configuration.GetValue<string>("TenantId");

                    if (!string.IsNullOrEmpty(tenantId))
                    {
                        var tenantString = DecryptRijndael(tenantId, Configuration.GetValue<string>("PrivateKey"));
                        var key = tenantString + "-Connection" + Configuration.GetValue<string>("KeyVaultConfig:PostFix");
                        keyVault.ConnectionString = Configuration.GetValue<string>(key);
                        var retryPolicy = Policy.Handle<Exception>().Retry(2, (ex, count, context) =>
                        {
                            (Configuration as IConfigurationRoot)?.Reload();
                            keyVault.ConnectionString = Configuration.GetValue<string>(key);
                        });
                        retryPolicy.Execute(() =>
                        {
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(keyVault?.ConnectionString);
                        });
                    }
                    else
                    {
                        Console.WriteLine("Invalid tenant is received");
                    }
                }
                //var conn = Configuration.GetConnectionString("ConnectionString");
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(conn);
            });
            services.AddScoped<IOperationStatus, OperationStatus>();
            services.AddScoped<IDataContextAsync>(opt => opt.GetRequiredService<GuidedTourDbContext>());
            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddTransient<IServicesAggregator, ServicesAggregator>();


            services.AddAutoMapper(Assembly.Load("GuidedTour.Service"));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddMvc(options => options.Filters.Add(typeof(ExceptionFiltersAttribute)))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<CustomHeaderSwaggerFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "GuidedTour Service",
                    Description = "GuidedTour Service",
                });
            });
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IGuidedTourService, GuidedTourService>();
            services.AddTransient<IOnBoardingService, OnBoardingService>();
            services.AddTransient<TokenManagerMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration.GetSection("SwaggerEndpoint").Value, "GuidedTour Service");
            });
            app.UseRouting();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseMiddleware<TokenManagerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


     

        #region Rijndael Encryption

        private string DecryptRijndael(string cipherinput, string salt)
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
                   Regex.IsMatch(base64String, AppConstants.Base64Regex, RegexOptions.None);

        }
        #endregion
    }
}
