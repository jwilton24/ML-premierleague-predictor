using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PremierPredictor
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create all tables if they don't exist
            Userdatabase userdatabase = new Userdatabase();
            userdatabase.Initialise();
            userdatabase.ResetSignIn();

            PlayerDatabase playerdatabase = new PlayerDatabase();
            playerdatabase.PlayerInitialise(); 
            playerdatabase.FillTable();
            
            Errorbox.Hide();
        }

        private void Loginbutton_Click(object sender, EventArgs e)
        {
            // Stores entered inputs into the text boxes
            string usernameInput = usernametextbox.Text;
            string passwordInput = passwordtextbox.Text;

            Userdatabase database = new Userdatabase();
            
            // Gets the password currently saved with the entered username
            string passwordRecieved = database.GetPassword(usernameInput);
            
            // Hash the inputted password to compare it with existing password
            string passwordHashed = database.HashPassword(passwordInput);

            Homescreen homescreen = new Homescreen();

            // If neither inputs are empty
            if (!string.IsNullOrEmpty(usernameInput) && !string.IsNullOrEmpty(passwordInput))
            {
                // No existing password for the username - create account and sign in
                if (string.IsNullOrEmpty(passwordRecieved))
                {
                    database.MakeAccount(usernameInput, passwordInput);
                    database.AdjustSignIn(usernameInput);
                    this.Hide(); 
                    homescreen.Show();
                }
                else
                {
                    // Hashed password entered is the same as password from database - log in
                    if (passwordHashed == passwordRecieved)
                    {
                        Errorbox.Show();
                        Errorbox.Text = "Sucessful log in";
                        database.AdjustSignIn(usernameInput);
                        this.Hide();
                        homescreen.Show();
                    }
                    else
                    {
                        Errorbox.Show();
                        Errorbox.Text = "This password is incorrect";
                    }
                }
            }
            else
            {
                Errorbox.Show();
                Errorbox.Text = "Username or password is not of valid length";
            }
        }
    }
}
