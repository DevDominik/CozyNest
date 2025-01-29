using MySql.Data.MySqlClient;
using System.Reflection.Metadata.Ecma335;

namespace CozyNestAPIHub.Handlers
{
    public class UserHandler
    {
        static UserHandler instance;
        static MySqlConnection connection;
        UserHandler(string name, string password) 
        {
            connection = new MySqlConnection($"Server=localhost;Database=cozynest;User ID={name};Password={password};");
            connection.Open();
        }
        ~UserHandler() { connection.Close(); }

        public static void Initialize(string name, string password) 
        {
            if (instance != null) { throw new TaskSchedulerException("There's already an UserHandler instance running."); }
            instance = new UserHandler(name, password);
        }
    }
}
