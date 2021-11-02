using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using GuidedTour.Service.Contracts;
using System.Diagnostics.CodeAnalysis;
using GuidedTour.EF.Entity;


namespace GuidedTour.Service
{
    [ExcludeFromCodeCoverage]
    public class ServicesAggregator : IServicesAggregator
    {
        public IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        public IOperationStatus OperationStatus { get; set; }
        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }

        public IMapper Mapper { get; set; }

        public ServicesAggregator(IUnitOfWorkAsync unitOfWorkAsync, IOperationStatus operationStatus, IConfiguration configuration, IHostingEnvironment environment,IMapper mapper)
        {
            UnitOfWorkAsync = unitOfWorkAsync;
            OperationStatus = operationStatus;
            Configuration = configuration;
            HostingEnvironment = environment;
            Mapper = mapper;
        }
    }
}
