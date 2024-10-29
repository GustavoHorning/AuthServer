namespace AuthServer.Users
{
    public class UsersService
    {
        private readonly UsersRepository _repository;

        // Injeção de dependência via construtor
        public UsersService(UsersRepository repository)
        {
            _repository = repository;
        }

        public User Save(User user) => _repository.Save(user);

        public User GetById(long id) => _repository.GetById(id);

        public List<User> FindAll() => _repository.FindAll();
    }
}
