using System;
using System.Data.SQLite;
using System.Collections.Generic;


namespace BibliotekaSQL
{
    public class AccountDataBase
    {
        SQLiteConnection connect;
        /// <summary>
        /// Lista kont 
        /// </summary>
        List<Account> accounts = new List<Account>();
        /// <summary>
        /// Klasa obrazujaca jak wygladaja dane konta
        /// </summary>
        public class Account
        {
            public int ID { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
        }


        public void ustawieniePolaczenia(SQLiteConnection polacz)
        {
            connect = polacz;
            connect.Open();
        }
        /// <summary>
        /// Funkcja pobierajaca wszystkie dane o kontach i wpisujaca je do listy
        /// </summary>
        public List<Account> Accounts
        {
            get
            {
                var cons = connect.CreateCommand();
                cons.CommandText = "SELECT * FROM Accounts";
                var reader = cons.ExecuteReader();
                accounts.Clear();

                while(reader.Read())
                {
                    accounts.Add(new Account
                    {
                        ID = reader.GetInt32(0),
                        Login = reader.GetString(1),
                        Password = reader.GetString(2)
                    });
                }
                return accounts;
            }
        }
        /// <summary>
        /// aktualizacja bazy danych
        /// </summary>
        public void SaveOrUpdate()
        {
            var cons = connect.CreateCommand();
            foreach(var e in accounts)
            {
                try
                {
                    cons.CommandText =
                        $"INSERT INTO Accounts (username, password) VALUES ('{e.Login}', '{e.Password}');";
                    cons.ExecuteNonQuery();
                }
                catch (SQLiteException)
                {
                    cons.CommandText =
                       $"UPDATE Accounts SET username='{e.Login}', password='{e.Password}' WHERE id={e.ID};";
                    cons.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Tworzenie tabeli w bazie danych oraz stworzenie dwoch kont "defaultowych"
        /// </summary>
        public void CreateBase()
        {
            var cmd = connect.CreateCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS Accounts";
            cmd.ExecuteNonQuery();

            cmd.CommandText =
                @"CREATE TABLE Accounts (id INTEGER PRIMARY KEY AUTOINCREMENT, username VARCHAR(20) UNIQUE NOT NULL,password VARCHAR(64) NOT NULL);";
            cmd.ExecuteNonQuery();

            accounts.Add(new Account { Login = "Damian", Password = "super" });
            accounts.Add(new Account { Login = "admin", Password = "admin" });

            SaveOrUpdate();
        }
        /// <summary>
        /// metoda tworzaca nowe konto w bazie danych
        /// </summary>
        /// <param name="login">Login wpisany przez uzytkownika</param>
        /// <param name="password">Haslo wpisane przez uzytkownika</param>
        public void NewAccount(string login, string password)
        {
            accounts.Add(new Account { Login = login, Password = password });
            SaveOrUpdate();
        }
    }
}
