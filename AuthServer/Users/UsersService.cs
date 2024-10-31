using AuthServer.Users.requests;

namespace AuthServer.Users
{
    public class UsersService
    {
        private readonly UsersRepository _repository;
        private readonly RoleRepository _roleRepository;

        // Injeção de dependência via construtor
        public UsersService(UsersRepository repository, RoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public User Save(UserRequest req)
        {
            // Criação do usuário
            var user = new User
            {
                Email = req.Email,
                Password = req.Password,
                Name = req.Name
            };

            // Busca a role "USER" e adiciona ao usuário
            var userRole = _roleRepository.FindByName("USER");
            if (userRole == null)
                throw new InvalidOperationException("Role 'USER' not found!");

            user.Roles.Add(userRole);

            return _repository.Save(user);
        }

        public User GetById(long id) => _repository.GetById(id);

        public List<User> FindAll() => _repository.FindAll();
    }
}
