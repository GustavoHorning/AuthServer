using AuthServer.Users.responses;

namespace AuthServer.Users
{
    public class User
    {
       
        public long ID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }

        public UserResponse ToResponse() => new UserResponse(ID, Name, Email);



    }
}
