using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace Instagram
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private string user;
        private string photo;
        private string photonot = string.Empty;

        Additonals adds = new Additonals();
        public EditProfile(string user)
        {
            InitializeComponent();
            this.user = user;
            this.photo = adds.getImage(user);
            adds.getAllForChange(user,photo,image,name,surname,status,password, photonot);
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Imange files (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                image.Height = 140;
                image.Width = 140;
                image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                photo = openFileDialog.FileName.ToString();
            }
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button1SaveClick(object sender, RoutedEventArgs e)
        {
            if (name.Text != string.Empty && password.Text != string.Empty && surname.Text != string.Empty && status.Text != string.Empty)
            {
                if (photo != string.Empty)
                {
                    adds.change(user, name.Text, surname.Text,password.Text,status.Text, photo);
                    MessageBox.Show("Successfully changed");
                    Close();
                }
                else
                {
                    adds.change(user, name.Text, surname.Text, password.Text,status.Text, photo);
                    MessageBox.Show("Successfully changed");
                    Close();
                }

            }
            else
            {
                MessageBox.Show("Blanks must be not empty");
            }
        }
    }
}
