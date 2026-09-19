using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PremierPredictor
{
    internal class Userdatabase
    {
        protected const string connectionStr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\databaselibrary.mdf;Integrated Security=True"
        protected SqlConnection sqlConnection;

        public Userdatabase()
        {
            sqlConnection = GetConnection();
        }

        protected SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open(); 
            return connection;
        }

        protected SqlCommand GetCommand()
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open(); 
            }
            return sqlConnection.CreateCommand();
        }

        public void Initialise()
        {
            try
            {
                SqlCommand cmd = GetCommand();
                cmd.CommandText = "CREATE TABLE Users (username CHAR(50), password VARCHAR(80), recentsearch1 VARCHAR(50), recentsearch2 VARCHAR(50), recentsearch3 VARCHAR(50), signedin VARCHAR(50), PRIMARY KEY (username))";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        public void ResetSignIn()
        {
            try
            {
                SqlCommand cmd = GetCommand();
                cmd.CommandText = "UPDATE Users SET signedin = '0'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AdjustSignIn(string username)
        {
            try
            {
                using (SqlCommand cmd = GetCommand())
                {
                    cmd.CommandText = "UPDATE Users SET signedin=1 WHERE username = @Username";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<string> ReturnRecentSearches()
        {
            List<string> recentSearches = new List<string>();
            try
            {
                using (SqlCommand cmd = GetCommand())
                {
                    cmd.CommandText = "SELECT recentsearch1, recentsearch2, recentsearch3 FROM Users WHERE signedin = '1'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string recentSearch1 = reader["recentsearch1"].ToString();
                            string recentSearch2 = reader["recentsearch2"].ToString();
                            string recentSearch3 = reader["recentsearch3"].ToString();
                            
                            if (recentSearch1.Length > 0) recentSearches.Add(recentSearch1);
                            if (recentSearch2.Length > 0) recentSearches.Add(recentSearch2);
                            if (recentSearch3.Length > 0) recentSearches.Add(recentSearch3);
                        }
                    }
                }
                return recentSearches;
            }
            catch { return recentSearches; }
        }

        public string GetPassword(string usernameInput)
        {
            string username = usernameInput;
            string password = string.Empty;
            SqlCommand cmd = GetCommand();
            cmd.CommandText = "SELECT password FROM Users WHERE username = @Username";
            cmd.Parameters.AddWithValue("@Username", username);
            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        password = reader["password"].ToString();
                    }
                }
            }
            catch { }
            return password;
        }

        public string HashPassword(string passwordInput)
        {
            string password = passwordInput;
            long hash = 17;
            foreach (char c in password)
            {
                hash = (hash * 31) ^ ((hash << 5) + c);
            }
            return hash.ToString();
        }

        public void MakeAccount(string usernameInput, string passwordInput)
        {
            string password = HashPassword(passwordInput);
            string username = usernameInput;
            try
            {
                using (SqlCommand cmd = GetCommand())
                {
                    cmd.CommandText = "INSERT INTO Users(username, password, recentsearch1, recentsearch2, recentsearch3, signedin) VALUES (@Username, @Password, '', '', '', '1')";
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void updateRecentSearches(string chosenplayer)
        {
            try
            {
                using (SqlCommand cmd = GetCommand())
                {
                    cmd.CommandText = "SELECT username, password, recentsearch1, recentsearch2, recentsearch3 FROM Users WHERE signedin = '1'";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string recentSearch1 = reader["recentsearch1"].ToString();
                            string recentSearch2 = reader["recentsearch2"].ToString();
                            string recentSearch3 = reader["recentsearch3"].ToString();
                            string existingUsername = reader["username"].ToString();
                            
                            recentSearch3 = recentSearch2;
                            recentSearch2 = recentSearch1;
                            recentSearch1 = chosenplayer;
                            reader.Close();
                            
                            cmd.CommandText = "UPDATE Users SET recentsearch1 = @recentSearch1, recentsearch2 = @recentSearch2, recentsearch3 = @recentSearch3 WHERE username = @username AND signedin = '1'";
                            cmd.Parameters.AddWithValue("@recentSearch1", recentSearch1);
                            cmd.Parameters.AddWithValue("@recentSearch2", recentSearch2);
                            cmd.Parameters.AddWithValue("@recentSearch3", recentSearch3);
                            cmd.Parameters.AddWithValue("@username", existingUsername);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
