namespace AuthServer.Users.responses
{
    public class UserResponse
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }

        public UserResponse(long id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
