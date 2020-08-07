using System;
using System.IO;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyProject.Authorization.Users;
using MyProject.Configuration;

namespace WorkerService
{
    public class Executor : ITransientDependency
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<User, long> _useRepository;
        private readonly UserManager _userManager;

        public Executor(
            IRepository<User, long> useRepository,
            UserManager userManager)
        {
            _useRepository = useRepository;
            _userManager = userManager;

            _appConfiguration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }


        public async void Run()
        {
            try
            {
                var list = await _useRepository.GetAllListAsync();
                foreach (var item in list)
                {
                    Console.WriteLine($"{item.Id},{item.UserName}");
                }

                await _userManager.CreateAsync(new User
                {
                    UserName = "test1"
                }, "123qwe");

            }
            catch (Exception ex)
            {
                Console.WriteLine("执行发生错误：" + ex.Message);
            }
        }
    }
}