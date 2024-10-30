using Microsoft.EntityFrameworkCore;

namespace AuthServer.Users
{
    public interface IUsersRepository
    {
        User Save(User user);
        User GetById(long id);
        List<User> FindAll();
    }

    public class UsersRepository : IUsersRepository
    {
        private readonly AuthServerContext _context; // Substitua YourDbContext pelo nome do seu DbContext

        public UsersRepository(AuthServerContext context)
        {
            _context = context;
        }

        public User Save(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); // Salva as alterações no banco de dados
            return user;
        }

        public User GetById(long id)
        {
            return _context.Users.FirstOrDefault(u => u.ID == id);
        }

        public List<User> FindAll()
        {
            return _context.Users.ToList();
        }
    }
}
