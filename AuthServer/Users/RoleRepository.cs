using System.Collections.Generic;

namespace AuthServer.Users
{
    public interface IRoleRepository
    {
        int Count();
        Role Save(Role role);
        Role? FindByName(string name);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly AuthServerContext _context;

        public RoleRepository(AuthServerContext context)
        {
            _context = context;
        }

        public int Count() => _context.Roles.Count();

        public Role Save(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role;
        }

        public Role? FindByName(string name)
        {
            return _context.Roles.FirstOrDefault(r => r.Name == name);
        }
    }
}
