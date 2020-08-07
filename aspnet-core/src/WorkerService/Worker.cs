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

                //��������ExecuteAsync�������̻߳��������أ����Ῠ�����ﱻ����
                await Task.WhenAll(task);
            }
            catch (Exception ex)
            {
                //RunTaskOne��RunTaskTwo��RunTaskThree�����У��쳣�����Ĵ����߼����������ǽ����һ����־
                _logger.LogError(ex.Message);
            }
            finally
            {
                //Worker Service����ֹͣ���������Ҫ��β���߼�������д������
            }
        }

        /// <summary>
        /// ִ������
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
                        //�������ֹͣ����ô�����IsCancellationRequested�᷵��true�����Ǿ�Ӧ�ý���ѭ��
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
