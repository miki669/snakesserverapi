using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SnakeServerAPI.DataBase.Data
{
    [Table("Users")]
    public class SnakeUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public ulong Discord_Id { get; set; }
        public string DS_Nick { get; set; } = null!;
        public string? Procces_Id { get; set; }
        public bool Subsripted { get; set; }
        public bool Ban { get; set; }
        public string? Ip { get; set; }
        public DateTime Subscripted_At { get; set; }
        public string? Disc_Info { get; set; } 
        public string? Gpu_Name { get; set; }
        public string? PC_Name { get; set; }
        public string? HWID { get; set; }
        public List<SnakeRole> Roles { get; set; }
        public List<UserToken> Tokens { get; set; }

        //public SnakeUsersRoles SnakeRoles { get; set; }
    }
}
