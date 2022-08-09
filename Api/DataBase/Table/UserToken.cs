using System.ComponentModel.DataAnnotations;

namespace SnakeServerAPI.DataBase.Data
{
    public class UserToken
    {
        [Key]
        public long Id { get; set; }
        public string Token { get; set; }
        public SnakeUser SnakeUser { get; set; }
        public DateTime TokenExpirationTime { get; set; }
    }
}
