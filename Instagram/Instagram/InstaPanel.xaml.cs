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
    /// Interaction logic for InstaPanel.xaml
    /// </summary>
    public partial class InstaPanel : Window
    {
        private string username;
        private string password;

        Additonals adds = new Additonals();
        private string imgLink = string.Empty;
        public InstaPanel(string username,string password)
        {
            InitializeComponent();
            this.username = username;
            this.password = password;

            adds.homeKit(adds.getid(username), panelforall, Likeme2);
        }

        private void myprofile_clicked(object sender, RoutedEventArgs e)
        {
            adds.SetMyProfile(username, panelforall, changeProfile, followersList, followingList, Likeme);
        }
        protected void followersList(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            somthing s = new somthing(username, id);
            s.ShowDialog();
        }
        protected void followingList(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            somthing s = new somthing(username, id);
            s.ShowDialog();
        }
        protected void changeProfile(object sender, RoutedEventArgs e)
        {
            EditProfile edit = new EditProfile(username);
            edit.ShowDialog();
            adds.SetMyProfile(username, panelforall, changeProfile, followersList, followingList, Likeme);
        }
        private void upload_clicked(object sender, RoutedEventArgs e)
        {
            adds.SetUpload(username, panelforall,opendialog,uplaodmake);
        }
        private void opendialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Imange files (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                imgLink = openFileDialog.FileName.ToString();
                MessageBox.Show("photo shoosen!");
            }
        }
        private void uplaodmake(object sender, RoutedEventArgs e)
        {
            if(imgLink != string.Empty)
            {
                byte[] photo = adds.ImageToArray(imgLink);
                adds.upload(adds.getid(username), "Some photo", imgLink);
                imgLink = string.Empty;
                MessageBox.Show("success");
            }
            else
            {
                MessageBox.Show("First choose photo!");
            }
        }
        private void Likeme2(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            adds.like(id);
            adds.homeKit(adds.getid(username), panelforall,Likeme2);
        }
        private void Likeme(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            adds.like(id);
            adds.SetMyProfile(username, panelforall, changeProfile, followersList, followingList, Likeme);
        }
        private void search_clicked(object sender, RoutedEventArgs e)
        {
            adds.search(adds.getid(username),panelforall,OpenProfileOther);
        }
        private void OpenProfileOther(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            Followto followto = new Followto(adds.getid(username),id, followersList, followingList, Likeme);
            followto.ShowDialog();

            adds.homeKit(adds.getid(username), panelforall, Likeme2);
        }

        private void home_clicked(object sender, RoutedEventArgs e)
        {
            adds.homeKit(adds.getid(username), panelforall, Likeme2);
        }
    }
}
