using Newtonsoft.Json.Linq;

namespace SnakeServerAPI.Assets
{
    public class ServerData
    {
        public static List<string>  DataBase = new();
        public static List<string>  CurrentX_Clients = new();
        public static List<JObject> SubscriptedUsers = new();
        public static List<string>  Roles = new();
    }
}
