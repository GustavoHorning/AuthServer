public class UserToken
{
    public long Id { get; set; }
    public string Name { get; set; }
    public HashSet<string> Roles { get; set; } = new HashSet<string>();

    public UserToken(long id, string name, HashSet<string> roles)
    {
        Id = id;
        Name = name;
        Roles = roles;
    }
}
