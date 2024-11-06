using AuthServer.Users.requests;
using AuthServer.Users.responses;
using AuthServer.Security;
using AuthServer; // Ou o namespace específico onde Jwt está localizada



namespace AuthServer.Users
{
    public class UsersService
    {
        private readonly UsersRepository _repository;
        private readonly RoleRepository _roleRepository;
        private readonly ILogger<UsersService> _logger;
        private readonly Jwt _jwt;


        // Injeção de dependência via construtor
        public UsersService(Jwt jwt,UsersRepository repository, RoleRepository roleRepository, ILogger<UsersService> logger)
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

            // Verifica se há usuários no banco
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
                return null; // Retorna null se o usuário não existir ou a senha estiver incorreta
            }
            _logger.LogInformation("User logged in. id={Id} name={Name}", user.ID, user.Name);
            var token = _jwt.CreateToken(user);
            return new LoginResponse(token, user.ToResponse());
        }

        public bool Delete(long id)
        {
            // Obtém o usuário a ser excluído
            var user = _repository.GetById(id);

            if (user == null)
            {
                _logger.LogWarning("Attempted to delete non-existing user. id={Id}", id);
                return false; // Retorna false se o usuário não for encontrado
            }

            // Verifica se o usuário possui a role de "ADMIN"
            if (user.Roles.Any(r => r.Name == "ADMIN"))
            {
                // Conta quantos administradores estão associados
                var adminCount = _repository.FindAll()
                    .Where(u => u.Roles.Any(r => r.Name == "ADMIN"))
                    .Count();

                // Verifica se o usuário é o único administrador
                if (adminCount <= 1)
                {
                    _logger.LogWarning("Attempted to delete the last system admin. id={Id}", id);
                    throw new BadRequestException("Cannot delete the last system admin!");
                }
            }

            // Se a exclusão foi permitida, deleta o usuário
            _repository.Delete(user); // Deleta o usuário
            _logger.LogWarning("User deleted. id={Id} name={Name}", user.ID, user.Name);
            return true; // Retorna true se a exclusão foi bem-sucedida
        }





        //public bool Delete(long id)
        //{
        //    // Obtém o usuário a ser excluído
        //    var user = _repository.GetById(id);

        //    if (user == null)
        //    {
        //        return false; // Retorna false se o usuário não for encontrado
        //    }

        //    // Verifica se o usuário possui a role de "ADMIN"
        //    if (user.Roles.Any(r => r.Name == "ADMIN"))
        //    {
        //        // Conta quantos administradores estão associados
        //        var adminCount = _repository.FindAll()
        //            .Where(u => u.Roles.Any(r => r.Name == "ADMIN"))
        //            .Count();

        //        // Verifica se o usuário é o único administrador
        //        if (adminCount <= 1)
        //        {
        //            return false; // Não pode excluir o último administrador
        //        }
        //    }

        //    // Se a exclusão foi permitida, deleta o usuário
        //    _repository.Delete(user); // Deleta o usuário
        //    return true; // Retorna true se a exclusão foi bem-sucedida
        //}
    }
}
