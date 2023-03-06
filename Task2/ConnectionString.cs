using System.Data;
using System.Data.SqlClient;

namespace Task2
{
    public static class DbConnectionFactory
    {
        public static IDbConnection CreateConnection()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyDatabase;Integrated Security=True";
            return new SqlConnection(connectionString);
        }
    }

}
