using System.Data.SQLite;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace SharpChatServer{
    class Database{
        public static void initDatabase(){
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS [LOGIN] (
                            [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [USERNAME] NVARCHAR(2048)  NULL,
                            [PSWDHASH] VARCHAR(2048)  NULL
                            )";
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=database.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database
            
                    com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();                  // Execute the query
            
                    //com.CommandText = "INSERT INTO LOGIN (USERNAME,PSWDHASH) Values ('john','test')";     // Add the first entry into our database 
                    //com.ExecuteNonQuery();      // Execute the query
                    //com.CommandText = "INSERT INTO LOGIN (USERNAME,PSWDHASH) Values ('greg','test2')";   // Add another entry into our database 
                    //com.ExecuteNonQuery();      // Execute the query
                    con.Close();        // Close the connection to the database
                }
            }
        }

        public static bool pswdChecker(String username, String passwordHash){
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=database.db3");
            System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con);
            con.Open();
            com.CommandText = $"Select * FROM LOGIN WHERE USERNAME = @usr";
            //com.Parameters.AddWithValue("@pswd", passwordHash);
            com.Parameters.AddWithValue("@usr",username);
            bool result = false;
            using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (Equals((string)reader["PSWDHASH"],passwordHash)){
                        result = true;
                    
                   }
                }
            }
            con.Close();
            switch (result){
                case true:
                    return true;
                case false:
                    return false;
            }
            
        }

        public static void addUser(String username, String passwordHash){
            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=database.db3");
            System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con);
            con.Open();
            com.CommandText = $"INSERT INTO LOGIN (USERNAME,PSWDHASH) Values (@usr,@pswd)";
            com.Parameters.AddWithValue("@usr",username);
            com.Parameters.AddWithValue("@pswd",passwordHash);
            com.ExecuteNonQuery();
            con.Close();
            return;
        }
    }
}