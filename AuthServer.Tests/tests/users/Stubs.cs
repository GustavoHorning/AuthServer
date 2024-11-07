using AuthServer.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthServer.tests.users
{
    public static class Stubs
    {
        public static User UserStub(long? id = null, List<string> roles = null)
        {
            id ??= new Random().Next(1, 1000);
            roles ??= new List<string> { "USER" };

            return new User
            {
                ID = id.Value,
                Name = $"user-{id}",
                Email = $"user-{id}@example.com",
                Password = RandomString(),
                Roles = roles.Select((role, i) => new Role { Long = i + 1, Name = role }).ToHashSet()
            };
        }

        private static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
