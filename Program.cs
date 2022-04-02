using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;







namespace Coding_Sessions_Tracker
{
    class Program
    {

       
        static void Main(string[] args)
        {





            using (SqliteConnection connection = DataAcessUtility.GetConnection("HabitTracker"))
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS coding_sessions(
                               Id INTEGER PRIMARY KEY AUTOINCREMENT,
                               Date TEXT,
                               Quantity INTEGER
                               )";

                cmd.ExecuteNonQuery();


            }





            GetUserInput();


        }


        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nChoose one option from the list and press enter:");
                Console.WriteLine("\n'0' - Close the app");
                Console.WriteLine("'1' - View your your coding sessions log");
                Console.WriteLine("'2' - Add coding sessions");
                Console.WriteLine("'3' - Delete coding sessions");
                Console.WriteLine("'4' - Update coding sessions log");


                string choice = Console.ReadLine();


                switch (choice)
                {
                    case "0":
                        closeApp = Exit();
                        break;

                    case "1":
                        ViewCodingSessionsLog();
                        break;

                    case "2":
                        AddCodingSessions();
                        break;

                    case "3":
                        DeleteCodingSessions();
                        break;

                    case "4":
                        UpdateCodingSessionsLog();
                        break;

                    default:
                        Console.WriteLine("Please choose from '0' , '1' , '2' , '3' ");
                        break;
                }


            }


        }

        private static bool Exit()
        {
            Console.Clear();
            Console.WriteLine("Goodbye!");
            bool closeApp = true;
            Environment.Exit(0);
            return closeApp;
        }


        private static void ViewCodingSessionsLog()
        {
            Console.Clear();
            using var connection = DataAcessUtility.GetConnection("HabitTracker");
            using var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = $"SELECT * FROM coding_sessions";

            List<CodingSessions> tableData = new();

            SqliteDataReader reader = selectCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                        new CodingSessions
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)

                        });
                }

            }
            else
            {
                Console.WriteLine("No rows found");
            }


            Console.WriteLine("---------------------------------\n");
            foreach (var dw in tableData)
            {
                Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MM-yyyy")} - Quantity: {dw.Quantity}");

            }
            Console.WriteLine("---------------------------------\n");
        }






        private static void AddCodingSessions()
        {
            Console.Clear();
            string date = GetDateInput();
            int quantity = GetQuantityInput("\n\nEnter the number of coding sessions(no decimals allowed)\n\n");

            using var connection = DataAcessUtility.GetConnection("HabitTracker");
            using var insertCmd = connection.CreateCommand();

            insertCmd.CommandText = $"INSERT INTO coding_sessions (date, quantity) VALUES ('{date}', {quantity})";

            insertCmd.ExecuteNonQuery();

        }

        internal static string GetDateInput()
        {
            Console.WriteLine("Enter today's date: (Format: dd-mm-yy). Type 0 to return to main menu.\n\n");
            string dateInput = Console.ReadLine();
            if (dateInput == "0")
            {
                GetUserInput();
            }
            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to the main menu or try again: \n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;

        }


        internal static int GetQuantityInput(string message)
        {
            Console.WriteLine(message);
            string quantityInput = Console.ReadLine();
            if (quantityInput == "0")
            {
                GetUserInput();
            }

            while (!Int32.TryParse(quantityInput, out _) || Convert.ToInt32(quantityInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again. \n\n");
                quantityInput = Console.ReadLine();
            }
            int finalInput = Convert.ToInt32(quantityInput);

            return finalInput;
        }



        private static void DeleteCodingSessions()
        {
            Console.Clear();
            ViewCodingSessionsLog();
            var recordId = GetQuantityInput("\n\nPlease type in the Id of the record your want to delete or type 0 to go back to the Main Menu");

            using var connection = DataAcessUtility.GetConnection("HabitTracker");
            using var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = $"DELETE FROM coding_sessions WHERE Id = {recordId}";
            int rowCount = deleteCmd.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                DeleteCodingSessions();
            }


            Console.WriteLine($"\n\nRecord with Id {recordId} has been deleted\n\n");
        }

        private static void UpdateCodingSessionsLog()
        {
            Console.Clear();
            ViewCodingSessionsLog();

            var recordId = GetQuantityInput("\n\nPlease type Id of the record you would like to update. Type 0 to return to the Main Menu.\n\n");

            using var connection = DataAcessUtility.GetConnection("HabitTracker");
            using var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM coding_sessions WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                connection.Close();
                UpdateCodingSessionsLog();
            }
            string date = GetDateInput();
            int quantity = GetQuantityInput("\n\nEnter the number of coding sessions (no decimals allowed) \n\n");

            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = $"UPDATE coding_sessions SET date = '{date}' , quantity = {quantity} WHERE Id = {recordId}";
            updateCmd.ExecuteNonQuery();





        }


    }

    public class CodingSessions
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
    
}

