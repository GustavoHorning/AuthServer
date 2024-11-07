using Microsoft.EntityFrameworkCore;


namespace AuthServer.Users
{
    public interface IUsersRepository
    {
        User Save(User user);
        User GetById(long id);
        List<User> FindAll();
        List<User> FindAllByRole(string role); 
        User? FindByEmail(string email); 
        void Delete(User user); 

    }

    public class UsersRepository : IUsersRepository
    {
        private readonly AuthServerContext _context;

        public UsersRepository(AuthServerContext context)
        {
            _context = context;
        }

        public User Save(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges(); 
            return user;
        }


        public User GetById(long id)
        {

            return _context.Users
                .Include(u => u.Roles) 
                .FirstOrDefault(u => u.ID == id);
        }


        public List<User> FindAll()
        {
            return _context.Users.Include(user => user.Roles).ToList();
        }

        public List<User> FindAllByRole(string role)
        {
            return _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.Name == role)) 
                .OrderBy(u => u.Name) 
                .Distinct()
                .ToList();
        }

        public User FindByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Roles) 
                .FirstOrDefault(u => u.Email == email);
        }

        public void Delete(User user) 
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public int CountUsersByRole(string roleName)
        {
            var roleId = _context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => r.Long)
                .FirstOrDefault();

            var sql = "SELECT COUNT(*) FROM UserRole WHERE RoleId = {0}";
            return _context.Database.ExecuteSqlRaw(sql, roleId);
        }




    }

}
