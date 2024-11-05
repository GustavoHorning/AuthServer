using AuthServer.Users.responses;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Users
{
    [Table("TblUser")] // Define o nome da tabela como "TblUser"
    public class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required] // Validação para não permitir valor nulo
        [EmailAddress] // Validação para garantir que o campo seja um email
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(50)")] // Define o tamanho máximo de 50 caracteres
        public string Password { get; set; }

        [Required] // Garante que o campo não seja nulo
        public string Name { get; set; } = string.Empty;

        // Relacionamento muitos-para-muitos com Role
        public HashSet<Role> Roles { get; set; } = new HashSet<Role>();
        public UserResponse ToResponse()
        {
            return new UserResponse(ID, Name, Email);
        }



    }
}
