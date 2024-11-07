using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Users
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Long { get; set; }
        [Required] // [required] garante que o campo não seja nulo, e [key] serve para definir primary key
        public string Name { get; set; } = string.Empty;
        public HashSet<User> Users { get; set; } = new HashSet<User>();

    }
}
