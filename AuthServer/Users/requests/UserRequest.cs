using System.ComponentModel.DataAnnotations;

namespace AuthServer.Users.requests
{
    public class UserRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email deve ser válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 50 caracteres.")]
        public string Password { get; set; }

        public UserRequest(string? name, string? email, string? password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
