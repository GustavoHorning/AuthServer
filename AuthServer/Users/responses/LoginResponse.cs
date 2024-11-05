using AuthServer.Users.responses;

public class LoginResponse
{
    public string Token { get; set; }
    public UserResponse User { get; set; }

    public LoginResponse(string token, UserResponse user)
    {
        Token = token;
        User = user;
    }
}
