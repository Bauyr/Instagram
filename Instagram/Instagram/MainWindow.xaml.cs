using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Instagram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            username.Text = "dimali";
            password.Password = "aaa123456";
        }
        Additonals adds = new Additonals();
        private void login(object sender, RoutedEventArgs e)
        {
            if (username.Text != string.Empty && password.Password != string.Empty)
            {
                string verify = adds.login(username.Text, password.Password);
                if (verify == "OK")
                {
                    InstaPanel messanger = new InstaPanel(username.Text, password.Password);
                    messanger.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login or Password incorrect!!!");
                }
            }
        }
        private void newAccountOpen(object sender, MouseButtonEventArgs e)
        {
            NewAccountPage page = new NewAccountPage();
            this.Hide();
            page.ShowDialog();
            this.Show();
        }
    }
}
