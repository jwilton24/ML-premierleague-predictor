using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace PremierPredictor
{
    public partial class Homescreen : Form
    {
        private bool isSearchButtonClicked = false;
        private bool ignoreSelectedIndex = false;

        public Homescreen()
        {
            InitializeComponent();
        }

        private void Homescreen_Load(object sender, EventArgs e)
        {
            PlayerDatabase playerDatabase = new PlayerDatabase();
            
            // Top goalscorer and shottaker's name is text on each button
            goalscorerbutton.Text = playerDatabase.FindGoalscorer(); 
            shottakerbutton.Text = playerDatabase.FindShottaker();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Stores entered search
            string search = searchbar.Text;
            List<string> recentsearches = new List<string>();
            DataTable dt = new DataTable();

            if (search.Length > 0)
            {
                PlayerDatabase playerDb = new PlayerDatabase();
                // Fill datatable with all players matching search
                dt = playerDb.ReturnSearch(search);
            }

            Userdatabase userdatabase = new Userdatabase();
            recentsearches = userdatabase.ReturnRecentSearches();

            if (recentsearches.Count > 0)
            {
                // Add any recent searches to table
                foreach (string recentSearch in recentsearches) 
                {
                    DataRow newrow = dt.NewRow();
                    newrow["PlayerName"] = recentSearch;
                    dt.Rows.Add(newrow);
                }
            }

            // Add all rows of datatable to the listbox
            searchresultsbox.Items.Clear(); // Added to prevent duplicate stacking on multiple searches
            foreach (DataRow row in dt.Rows) 
            {
                if (dt.Columns.Count > 0)
                {
                    string playerName = row["PlayerName"].ToString();
                    searchresultsbox.Items.Add(playerName);
                }
            }
            
            isSearchButtonClicked = true;
        }

        private void searchresultsbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When a player is pressed
            if (!ignoreSelectedIndex)
            {
                if (isSearchButtonClicked == true)
                {
                    if (searchresultsbox.SelectedIndex > -1)
                    {
                        string chosenPlayer = searchresultsbox.SelectedItem.ToString();
                        Userdatabase userdatabase = new Userdatabase();
                        
                        // Selected player is added as the user's most recent search
                        userdatabase.updateRecentSearches(chosenPlayer); 
                        
                        // Selected player is passed into playerprofile constructor
                        playerProfile playerprofile = new playerProfile(chosenPlayer);
                        playerprofile.ShowDialog();
                    }
                }
                isSearchButtonClicked = false;
            }
            ignoreSelectedIndex = false;
        }

        private void searchresultsbox_DropDown(object sender, EventArgs e)
        {
            ignoreSelectedIndex = true;
        }

        private void goalscorerbutton_Click(object sender, EventArgs e)
        {
            Userdatabase userdatabase = new Userdatabase();
            string player = goalscorerbutton.Text;
            
            // Add player as user's most recent search
            userdatabase.updateRecentSearches(player); 
            
            // Pass selected player into playerprofile constructor
            playerProfile playerprofile = new playerProfile(player); 
            playerprofile.ShowDialog();
        }

        private void shottakerbutton_Click(object sender, EventArgs e)
        {
            Userdatabase userdatabase = new Userdatabase();
            string player = shottakerbutton.Text;
            
            // Add player as user's most recent search
            userdatabase.updateRecentSearches(player); 
            
            // Pass selected player into playerprofile constructor
            playerProfile playerprofile = new playerProfile(player); 
            playerprofile.ShowDialog();
        }

        private void logoutButton_Click(object sender, EventArgs e) 
        {
            // Skip button pressed / Logout
            Userdatabase userdatabase = new Userdatabase();
            // Resets table so no users are currently signed in
            userdatabase.ResetSignIn(); 
            
            Login form = new Login();
            this.Hide(); 
            form.ShowDialog();
            this.Close();
        }

        private void comparebutton_Click(object sender, EventArgs e)
        {
            playerComparison playerComparison = new playerComparison();
            this.Hide(); 
            playerComparison.ShowDialog(); 
            this.Close();
        }
    }
}
