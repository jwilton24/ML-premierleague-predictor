using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PremierPredictor
{
    public partial class playerProfile : Form
    {
        public string selectedItem = "";
        public string playerID;

        public playerProfile(string chosenplayer)
        {
            InitializeComponent();
            playerName.Text = chosenplayer;
            PlayerDatabase playerDatabase = new PlayerDatabase();
            playerID = playerDatabase.FindPlayerID(chosenplayer);
            CreateGraph(playerID);
            errorpercentbox.Hide();
            predictionresultsbox.Hide();
        }

        private void HandleData(string selecteditem)
        {
            machineLearning machineLearning = new machineLearning();
            List<string> predictedreturn = new List<string>();
            predictedreturn = machineLearning.DataHandling(playerID, selecteditem);
            
            predictionresultsbox.Show();
            predictionresultsbox.Text = predictedreturn[0];
            
            if (predictedreturn[1].Length > 0)
            {
                errorpercentbox.Show();
                errorpercentbox.Text = "This models current error percent is " + predictedreturn[1] + "%";
            }
        }

        private void CreateGraph(string playerID)
        {
            PlayerDatabase playerDatabase = new PlayerDatabase();
            int games = 10;
            List<int> goals = new List<int>();
            goals = playerDatabase.GetYAxis(playerID);
            
            for (int i = 0; i < games; i++)
            {
                goalsgraph.Series[0].Points.AddXY(games - i, goals[199 - i]);
            }
            
            goalsgraph.Series[0].ChartType = SeriesChartType.Line;
            goalsgraph.Series[0].Color = Color.Black;
            goalsgraph.ChartAreas[0].AxisX.Title = "Game Number";
            goalsgraph.ChartAreas[0].AxisY.Title = "Goals";
            goalsgraph.Titles.Clear();
            goalsgraph.Titles.Add("Goals Over Last 10 Games");
        }

        private void goals_Click(object sender, EventArgs e)
        {
            selectedItem = "Goals";
            HandleData(selectedItem);
        }

        private void assists_Click(object sender, EventArgs e)
        {
            selectedItem = "Assists";
            HandleData(selectedItem);
        }

        private void shots_Click(object sender, EventArgs e)
        {
            selectedItem = "Shots";
            HandleData(selectedItem);
        }

        public void shotsontarget_Click(object sender, EventArgs e)
        {
            selectedItem = "ShotsOnTarget";
            HandleData(selectedItem);
        }

        private void newplayerbutton_Click(object sender, EventArgs e)
        {
            Homescreen homescreen = new Homescreen();
            this.Hide();
            homescreen.ShowDialog();
            this.Close();
        }
    }
}
