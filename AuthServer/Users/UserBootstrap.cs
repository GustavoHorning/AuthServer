using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthServer.Users
{
    public class UsersBootstrap : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public UsersBootstrap(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var roleRepository = scope.ServiceProvider.GetRequiredService<RoleRepository>();
                var userRepository = scope.ServiceProvider.GetRequiredService<UsersRepository>();

                if (roleRepository.Count() == 0)
                {
                    var adminRole = new Role { Name = "ADMIN" };
                    roleRepository.Save(adminRole);
                    roleRepository.Save(new Role { Name = "USER" });
                }

                if (userRepository.FindAll().Count == 0)
                {
                    var adminRole = roleRepository.FindByName("ADMIN");
                    if (adminRole != null)
                    {
                        var adminUser = new User
                        {
                            Email = "admin@authserver.com",
                            Password = "admin",
                            Name = "Auth Server Administrator",
                        };

                        adminUser.Roles.Add(adminRole); // Adiciona a role ao usuário
                        userRepository.Save(adminUser);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
