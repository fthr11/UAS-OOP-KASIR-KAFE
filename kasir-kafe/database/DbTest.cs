using System;
using MySqlConnector;

public static class DbTest
{
    public static void Test()
    {
        string connString = "server=localhost;database=kasirkafe;user=root;password=;";

        try
        {
            using var conn = new MySqlConnection(connString);
            conn.Open();
            Console.WriteLine("MySQL Connected Successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("MySQL Connection Failed: " + ex.Message);
        }
    }
}
