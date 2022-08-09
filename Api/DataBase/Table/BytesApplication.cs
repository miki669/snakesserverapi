using System.ComponentModel.DataAnnotations;

namespace SnakeServerAPI.DataBase.Data
{
    public class BytesApplication
    {
        [Key]
        public int Id { get; set; }
        public string Data { get; set; }

        public DateTime DateTime { get; set; }
    }
}
