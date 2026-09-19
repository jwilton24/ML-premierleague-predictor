using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PremierPredictor
{
    internal class PlayerDatabase : Userdatabase
    {
        public void PlayerInitialise() 
        {
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    
                    cmd.CommandText = "CREATE TABLE Players (PlayerID INT PRIMARY KEY, PlayerName VARCHAR(100))";
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "CREATE TABLE PerformanceFinal (PerformanceID INT PRIMARY KEY, PlayerID INT, Game INT, Goals FLOAT, Assists FLOAT, Shots FLOAT, ShotsOnTarget FLOAT, FOREIGN KEY (PlayerID) REFERENCES Players (PlayerID));";
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        public void FillTable() 
        {
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    string[] players = { "Haaland", "Fred", "Mount", "Foden", "Saka" };
                    
                    for (int i = 1; i <= players.Length; i++)
                    {
                        cmd.CommandText = "INSERT INTO Players (PlayerID, PlayerName) VALUES (@id, @name)";
                        cmd.Parameters.AddWithValue("@id", i);
                        cmd.Parameters.AddWithValue("@name", players[i - 1]);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
            }
            catch { }

            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    Random random = new Random();
                    int game = 1;
                    int i = 1;
                    int numberOfPlayers = 5;
                    int noStatsPerPlayer = 200;
                    
                    for (int n = 1; n <= noStatsPerPlayer * numberOfPlayers; n++)
                    {
                        float intGoals = random.Next(0, 4);
                        float intAssists = random.Next(0, 4);
                        float intShots = random.Next(1, 5);
                        float intShotsOnTarget = random.Next(0, 5);
                        
                        float goals = intGoals / 10f;
                        float assists = intAssists / 10f;
                        float shots = intShots / 10f;
                        float shotsOnTarget = intShotsOnTarget / 10f;

                        using (SqlCommand cmd = sqlConnection.CreateCommand()) 
                        {
                            cmd.CommandText = "INSERT INTO PerformanceFinal (PerformanceID, PlayerID, Game, Goals, Assists, Shots, ShotsOnTarget) VALUES (@n, @i, @game, @goals, @assists, @shots, @shotsontarget)";
                            cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.Int)).Value = i;
                            cmd.Parameters.Add(new SqlParameter("@n", SqlDbType.Int)).Value = n;
                            cmd.Parameters.Add(new SqlParameter("@game", SqlDbType.Int)).Value = game;
                            cmd.Parameters.Add(new SqlParameter("@goals", SqlDbType.Float)).Value = goals;
                            cmd.Parameters.Add(new SqlParameter("@assists", SqlDbType.Float)).Value = assists;
                            cmd.Parameters.Add(new SqlParameter("@shots", SqlDbType.Float)).Value = shots;
                            cmd.Parameters.Add(new SqlParameter("@shotsontarget", SqlDbType.Float)).Value = shotsOnTarget;
                            cmd.ExecuteNonQuery();
                        }
                        
                        game++;
                        if (n % noStatsPerPlayer == 0) 
                        {
                            i++;
                            game = 1;
                        }
                    }
                }
            }
            catch { }
        }

        public DataTable ReturnSearch(string search) 
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT PlayerName FROM Players WHERE PlayerName LIKE @name";
                    cmd.Parameters.AddWithValue("@name", "%" + search + "%");
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch { }
            return dataTable;
        }

        public string FindPlayerID(string name) 
        {
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    string id = "";
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT PlayerID FROM Players WHERE PlayerName=@name";
                    cmd.Parameters.AddWithValue("@name", name);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader["PlayerID"].ToString();
                        }
                    }
                    return id;
                }
            }
            catch
            {
                return "";
            }
        }

        public string FindGoalscorer() 
        {
            try
            {
                string goalscorer = "";
                List<double> goals = new List<double>();
                
                using (SqlConnection sqlConnection = GetConnection())
                {
                    for (int i = 1; i < 6; i++)
                    {
                        SqlCommand cmd = sqlConnection.CreateCommand();
                        cmd.CommandText = "SELECT Goals FROM PerformanceFinal WHERE PlayerID=@i AND Game % 200 = 0";
                        cmd.Parameters.AddWithValue("@i", i);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double eachgoals = Convert.ToDouble(reader["Goals"]);
                                goals.Add(eachgoals);
                            }
                            reader.Close();
                        }
                    }
                }
                
                List<double> sortedlist = MergeSort(goals);
                double mostgoals = sortedlist[sortedlist.Count - 1];
                goalscorer = ReturnGoalscorer(mostgoals);
                
                return goalscorer;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string ReturnGoalscorer(double goals)
        {
            string goalscorer = "";
            int statsperplayer = 200;
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT Players.PlayerName FROM Players JOIN PerformanceFinal ON Players.PlayerID = PerformanceFinal.PlayerID WHERE PerformanceFinal.Goals = @goals AND PerformanceFinal.Game % @statsperplayer = 0";
                    cmd.Parameters.AddWithValue("@goals", goals);
                    cmd.Parameters.AddWithValue("@statsperplayer", statsperplayer);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            goalscorer = reader["playername"].ToString();
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return goalscorer;
        }

        public string FindShottaker() 
        {
            try
            {
                string shottaker = "";
                List<double> shots = new List<double>();
                
                using (SqlConnection sqlConnection = GetConnection())
                {
                    for (int i = 1; i < 6; i++)
                    {
                        SqlCommand cmd = sqlConnection.CreateCommand();
                        cmd.CommandText = "SELECT Shots FROM PerformanceFinal WHERE PlayerID=@i AND Game % 200 = 0";
                        cmd.Parameters.AddWithValue("@i", i);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double eachshots = Convert.ToDouble(reader["Shots"]);
                                shots.Add(eachshots);
                            }
                            reader.Close();
                        }
                    }
                }
                
                List<double> sortedlist = MergeSort(shots);
                double mostshots = sortedlist[sortedlist.Count - 1];
                shottaker = ReturnShottaker(mostshots);
                
                return shottaker;
            }
            catch
            {
                return "";
            }
        }

        public string ReturnShottaker(double shots)
        {
            string shottaker = "";
            int statsperplayer = 200;
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT Players.PlayerName FROM Players JOIN PerformanceFinal ON Players.PlayerID = PerformanceFinal.PlayerID WHERE PerformanceFinal.Shots = @shots AND PerformanceFinal.Game % @statsperplayer = 0";
                    cmd.Parameters.AddWithValue("@statsperplayer", statsperplayer);
                    cmd.Parameters.AddWithValue("@shots", shots);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shottaker = reader["playername"].ToString();
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return shottaker;
        }

        private List<double> MergeSort(List<double> list)
        {
            if (list.Count <= 1) return list;

            int middle = list.Count / 2;
            List<double> left = new List<double>();
            List<double> right = new List<double>();
            
            for (int i = 0; i < middle; i++)
                left.Add(list[i]);
                
            for (int i = middle; i < list.Count; i++)
                right.Add(list[i]);
                
            left = MergeSort(left);
            right = MergeSort(right);
            
            list = Merge(left, right);
            return list;
        }

        private List<double> Merge(List<double> left, List<double> right)
        {
            List<double> result = new List<double>();
            int i = 0, j = 0;
            
            while (i < left.Count && j < right.Count)
            {
                if (left[i] < right[j])
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }
            
            while (i < left.Count)
            {
                result.Add(left[i]);
                i++;
            }
            
            while (j < right.Count)
            {
                result.Add(right[j]);
                j++;
            }
            
            return result;
        }

        public List<int> GetYAxis(string playerID) 
        {
            List<int> goals = new List<int>();
            int playerIDint = -1;
            
            try
            {
                playerIDint = Convert.ToInt32(playerID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            int statsperplayer = 200;
            
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    for (int i = 0; i < 10; i++) 
                    {
                        int gamestart = (((playerIDint - 1) * statsperplayer) + 1);
                        int currentgame = gamestart + i;
                        
                        SqlCommand cmd = sqlConnection.CreateCommand();
                        cmd.CommandText = "SELECT Goals FROM PerformanceFinal WHERE PlayerID = @playerID AND Game = @currentgame";
                        cmd.Parameters.AddWithValue("@playerID", playerID);
                        cmd.Parameters.AddWithValue("@currentgame", currentgame);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double unsortedgoals = Convert.ToDouble(reader["Goals"]);
                                double roundedgoals = Math.Round(unsortedgoals, 2); 
                                int sortedgoals = Convert.ToInt32(roundedgoals * 10);
                                goals.Add(sortedgoals);
                            }
                            reader.Close();
                        }
                    }
                }
                return goals;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<int>();
            }
        }

        public List<string> GetPlayers() 
        {
            List<string> players = new List<string>();
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT PlayerName FROM Players";
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string playername = reader["PlayerName"].ToString();
                            players.Add(playername);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return players;
        }

        public List<double> ReturnPlayerStats(string player)
        {
            List<double> stats = new List<double>();
            double totalgoals = 0;
            double totalassists = 0;
            double totalshots = 0;
            double totalshotsontarget = 0;
            bool errorreturning = false;
            
            try
            {
                using (SqlConnection sqlConnection = GetConnection())
                {
                    SqlCommand cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = "SELECT SUM(PerformanceFinal.Goals) AS TotalGoals, SUM(PerformanceFinal.Assists) AS TotalAssists, SUM(PerformanceFinal.Shots) AS TotalShots, SUM(PerformanceFinal.ShotsOnTarget) AS TotalShotsOnTarget FROM Players JOIN PerformanceFinal ON Players.PlayerID = PerformanceFinal.PlayerID WHERE Players.PlayerName = @player"; 
                    cmd.Parameters.AddWithValue("@player", player);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalgoals = reader["TotalGoals"] != DBNull.Value ? Convert.ToDouble(reader["TotalGoals"]) : 0; 
                            totalassists = reader["TotalAssists"] != DBNull.Value ? Convert.ToDouble(reader["TotalAssists"]) : 0;
                            totalshots = reader["TotalShots"] != DBNull.Value ? Convert.ToDouble(reader["TotalShots"]) : 0;
                            totalshotsontarget = reader["TotalShotsOnTarget"] != DBNull.Value ? Convert.ToDouble(reader["TotalShotsOnTarget"]) : 0;
                        }
                    }
                }
            }
            catch 
            { 
                errorreturning = true; 
            }
            
            if (errorreturning == false) 
            {
                stats.Add(totalgoals);
                stats.Add(totalassists);
                stats.Add(totalshots);
                stats.Add(totalshotsontarget);
            }
            
            return stats;
        }
    }
}
