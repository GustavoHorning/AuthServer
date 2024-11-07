using AuthServer.Users.requests;
using AuthServer.Users.responses;
using AuthServer.Security;
using AuthServer; 



namespace AuthServer.Users
{
    public class UsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UsersService> _logger;
        private readonly IJwt _jwt;


        public UsersService(IJwt jwt,IUsersRepository repository, IRoleRepository roleRepository, 
            ILogger<UsersService> logger)
        {
            _jwt = jwt;
            _repository = repository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public User Save(UserRequest req)
        {
            var user = new User
            {
                Email = req.Email,
                Password = req.Password,
                Name = req.Name
            };

            if (_repository.FindAll().Count == 0)
            {
                // Se for o primeiro usuário, atribui a role "ADMIN"
                var adminRole = _roleRepository.FindByName("ADMIN");
                if (adminRole != null)
                {
                    user.Roles.Add(adminRole);
                }
            }
            else
            {
                // Caso contrário, atribui a role padrão "USER"
                var userRole = _roleRepository.FindByName("USER");
                if (userRole != null)
                {
                    user.Roles.Add(userRole);
                }
            }

            return _repository.Save(user);
        }


        public User GetById(long id) => _repository.GetById(id);

        public List<User> FindAll() => _repository.FindAll();

        public List<User> FindAll(string? role = null)
        {
            return role == null ? _repository.FindAll() : _repository.FindAllByRole(role);
        }

        public LoginResponse? Login(LoginRequest credentials)
        {
            var user = _repository.FindByEmail(credentials.Email);
            if (user == null || user.Password != credentials.Password)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", credentials.Email);
                return null;
            }
            _logger.LogInformation("User logged in. id={Id} name={Name}", user.ID, user.Name);
            var token = _jwt.CreateToken(user);
            return new LoginResponse(token, user.ToResponse());
        }

        public bool Delete(long id)
        {
            var user = _repository.GetById(id);

            if (user == null)
            {
                _logger.LogWarning("Attempted to delete non-existing user. id={Id}", id);
                return false; 
            }

            if (user.Roles.Any(r => r.Name == "ADMIN"))
            {
                var adminCount = _repository.FindAll()
                    .Where(u => u.Roles.Any(r => r.Name == "ADMIN"))
                    .Count();

                if (adminCount <= 1)
                {
                    _logger.LogWarning("Attempted to delete the last system admin. id={Id}", id);
                    throw new BadRequestException("Cannot delete the last system admin!");
                }
            }

            _repository.Delete(user);
            _logger.LogWarning("User deleted. id={Id} name={Name}", user.ID, user.Name);
            return true;
        }

    }
}
