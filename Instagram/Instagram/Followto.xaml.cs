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
    /// Interaction logic for Followto.xaml
    /// </summary>
    public partial class Followto : Window
    {
        private int user;
        private int id;

        RoutedEventHandler followerList;
        RoutedEventHandler followingList;
        RoutedEventHandler LikeMe;
        Additonals adds = new Additonals();
        public Followto(int user,int id, RoutedEventHandler followerList, RoutedEventHandler followingList, RoutedEventHandler LikeMe)
        {
            InitializeComponent();
            this.user = user;
            this.id = id;

            this.followerList = followerList;
            this.followingList = followingList;
            this.LikeMe = LikeMe;
            adds.foo(id, panelforall, follow, followerList, followingList, LikeMe,user);
        }

        private void follow(object sender, RoutedEventArgs e)
        {
            adds.dofollowing(user, id);
            adds.dofollower(id, user);
            adds.foo(id, panelforall, follow, followerList, followingList, LikeMe, user);
        }
    }
}
