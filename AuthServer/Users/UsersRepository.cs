using Microsoft.EntityFrameworkCore;


namespace AuthServer.Users
{
    public interface IUsersRepository
    {
        User Save(User user);
        User GetById(long id);
        List<User> FindAll();
        List<User> FindAllByRole(string role); // Novo método para buscar usuários por role
        User? FindByEmail(string email); // Novo método para buscar usuário por email
        void Delete(User user); // Adiciona o método de deletar usuário

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

        //public User GetById(long id)
        //{
         //   return _context.Users.FirstOrDefault(u => u.ID == id);
        //}

        public User GetById(long id)
        {
            // Inclui as roles associadas ao usuário
            return _context.Users
                .Include(u => u.Roles) // Inclui as roles do usuário
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
                .Where(u => u.Roles.Any(r => r.Name == role)) // Filtra pela role específica
                .OrderBy(u => u.Name) // Ordena por nome
                .Distinct()
                .ToList();
        }

        //public User? FindByEmail(string email) // Implementação do novo método
        //{
        //    return _context.Users.FirstOrDefault(u => u.Email == email);
        //}
        public User FindByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Roles) // Incluindo as roles na consulta
                .FirstOrDefault(u => u.Email == email);
        }

        public void Delete(User user) // Implementação do método Delete
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
