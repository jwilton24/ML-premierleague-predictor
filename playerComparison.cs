using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PremierPredictor
{
    public partial class playerComparison : Form
    {
        public playerComparison()
        {
            InitializeComponent();
            player1box.Hide();
            player2box.Hide();
            errorbox.Hide();
            PopulateLists();
        }

        private void PopulateLists()
        {
            // Adds all players to each listbox
            List<string> playerlist = new List<string>();
            PlayerDatabase playerDatabase = new PlayerDatabase();
            playerlist = playerDatabase.GetPlayers();
            
            foreach (string player in playerlist)
            {
                listBox1.Items.Add(player);
                listBox2.Items.Add(player);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) 
        {
            // If a player is selected
            string selectedPlayer = "";
            if (listBox1.SelectedItems.Count == 1)
            {
                foreach (var selectedItem in listBox1.SelectedItems)
                {
                    // Store selected player in variable
                    selectedPlayer = selectedItem.ToString(); 
                }
            }
            
            player1box.Show();
            player1box.Text = selectedPlayer;
            Compare();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) 
        {
            // If a player is selected
            string selectedPlayer = "";
            if (listBox2.SelectedItems.Count == 1)
            {
                foreach (var selectedItem in listBox2.SelectedItems)
                {
                    // Store selected player in variable
                    selectedPlayer = selectedItem.ToString(); 
                }
            }
            
            player2box.Show();
            player2box.Text = selectedPlayer;
            Compare();
        }

        public void Compare()
        {
            if ((player1box.Text.Length > 0) && (player2box.Text.Length > 0))
            {
                string player1 = player1box.Text;
                string player2 = player2box.Text;
                List<double> player1list = new List<double>();
                List<double> player2list = new List<double>();
                
                if (player1 != player2)
                {
                    errorbox.Hide(); // Hide any previous error messages
                    PlayerDatabase playerDatabase = new PlayerDatabase();
                    
                    // Returns list with sum of each statistic
                    player1list = playerDatabase.ReturnPlayerStats(player1);
                    player2list = playerDatabase.ReturnPlayerStats(player2);
                    
                    if ((player1list.Count > 0) && (player2list.Count > 0))
                    {
                        // Rounds to 1dp
                        double player1Goalsstat = Math.Round(player1list[0], 1);
                        double player1Assistsstat = Math.Round(player1list[1], 1);
                        double player1Shotsstat = Math.Round(player1list[2], 1);
                        double player1ShotsOnTargetstat = Math.Round(player1list[3], 1);
                        
                        double player2Goalsstat = Math.Round(player2list[0], 1);
                        double player2Assistsstat = Math.Round(player2list[1], 1);
                        double player2Shotsstat = Math.Round(player2list[2], 1);
                        double player2ShotsOnTargetstat = Math.Round(player2list[3], 1);
                        
                        // Compares player stats and colour codes textboxes accordingly
                        player1Goals.BackColor = player1Goalsstat > player2Goalsstat ? Color.Green : player1Goalsstat < player2Goalsstat ? Color.Red : Color.Transparent;
                        player2Goals.BackColor = player2Goalsstat > player1Goalsstat ? Color.Green : player2Goalsstat < player1Goalsstat ? Color.Red : Color.Transparent;
                        equalsboxgoals.Text = player1Goalsstat == player2Goalsstat ? "=" : "";
                        
                        player1Assists.BackColor = player1Assistsstat > player2Assistsstat ? Color.Green : player1Assistsstat < player2Assistsstat ? Color.Red : Color.Transparent;
                        player2Assists.BackColor = player2Assistsstat > player1Assistsstat ? Color.Green : player2Assistsstat < player1Assistsstat ? Color.Red : Color.Transparent;
                        equalsboxassists.Text = player1Assistsstat == player2Assistsstat ? "=" : "";
                        
                        player1Shots.BackColor = player1Shotsstat > player2Shotsstat ? Color.Green : player1Shotsstat < player2Shotsstat ? Color.Red : Color.Transparent;
                        player2Shots.BackColor = player2Shotsstat > player1Shotsstat ? Color.Green : player2Shotsstat < player1Shotsstat ? Color.Red : Color.Transparent;
                        equalsboxshots.Text = player1Shotsstat == player2Shotsstat ? "=" : "";
                        
                        player1ShotsOT.BackColor = player1ShotsOnTargetstat > player2ShotsOnTargetstat ? Color.Green : player1ShotsOnTargetstat < player2ShotsOnTargetstat ? Color.Red : Color.Transparent;
                        player2ShotsOT.BackColor = player2ShotsOnTargetstat > player1ShotsOnTargetstat ? Color.Green : player2ShotsOnTargetstat < player1ShotsOnTargetstat ? Color.Red : Color.Transparent;
                        equalsboxshotsOT.Text = player1ShotsOnTargetstat == player2ShotsOnTargetstat ? "=" : "";
                    }
                }
                else
                {
                    errorbox.Show();
                    errorbox.Text = "Please select two different players";
                }
            }
        }
    }
}
