using System.IO;
using Abp.Castle.Logging.Log4Net;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Facilities.Logging;
using Microsoft.Extensions.Configuration;
using MyProject;
using MyProject.Configuration;
using MyProject.EntityFrameworkCore;

namespace WorkerService
{
    [DependsOn(
        typeof(MyProjectApplicationModule),
        typeof(MyProjectEntityFrameworkModule)
    )]
    public class WorkerServiceModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public WorkerServiceModule(MyProjectEntityFrameworkModule dataModule)
        {
            dataModule.SkipDbSeed = true;
            _appConfiguration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
               "Default"
            );
        }

        public override void Initialize()
        {
            IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseAbpLog4Net().WithConfig("log4net.config"));

            var thisAssembly = typeof(WorkerServiceModule).GetAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);
        }
    }
}
