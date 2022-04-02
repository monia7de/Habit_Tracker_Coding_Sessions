using System.Configuration;
using Microsoft.Data.Sqlite;



namespace Coding_Sessions_Tracker
{
	public class DataAcessUtility
    {
		
		
        public static SqliteConnection GetConnection(string name)
        {

            string connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;

            SqliteConnection connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;

        }



    }
}

