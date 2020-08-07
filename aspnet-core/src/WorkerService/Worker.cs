using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Task task = Run(stoppingToken);

                //这样调用ExecuteAsync方法的线程会立即返回，不会卡在这里被阻塞
                await Task.WhenAll(task);
            }
            catch (Exception ex)
            {
                //RunTaskOne、RunTaskTwo、RunTaskThree方法中，异常捕获后的处理逻辑，这里我们仅输出一条日志
                _logger.LogError(ex.Message);
            }
            finally
            {
                //Worker Service服务停止后，如果有需要收尾的逻辑，可以写在这里
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected Task Run(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                using (var bootstrapper = AbpBootstrapper.Create<WorkerServiceModule>())
                {
                    bootstrapper.Initialize();

                    //Getting a Tester object from DI and running it
                    using (var tester = bootstrapper.IocManager.ResolveAsDisposable<Executor>())
                    {
                        //如果服务被停止，那么下面的IsCancellationRequested会返回true，我们就应该结束循环
                        //while (!stoppingToken.IsCancellationRequested)
                        //{
                        tester.Object.Run();
                        Thread.Sleep(1000);
                        //}
                    }
                }
            }, stoppingToken);
        }
    }
}
