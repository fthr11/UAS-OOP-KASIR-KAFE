using System;
using MySqlConnector;

public class DbTest
{
    public static void Test()
    {
        string connString = "server=localhost;database=kasirkafe;user=root;password=;";

        using var conn = new MySqlConnection(connString);

        try
        {
            conn.Open();
            Console.WriteLine("Database Connected Successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database Connection Failed:");
            Console.WriteLine(ex.Message);
        }
    }
}
