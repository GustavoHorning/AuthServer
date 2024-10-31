using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Users
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Long { get; set; }
        [Required] // Garante que o campo não seja nulo
        public string Name { get; set; } = string.Empty;
        // Relacionamento muitos-para-muitos com User
        public HashSet<User> Users { get; set; } = new HashSet<User>();


    }
}
