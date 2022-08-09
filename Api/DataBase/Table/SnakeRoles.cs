using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SnakeServerAPI.DataBase.Data
{
    [Table("Roles")]
    public class SnakeRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string? Name { get; set; }
        public ulong RoleId { get; set; }

        public List<SnakeUser> Users { get; set; }
        
    }
}
