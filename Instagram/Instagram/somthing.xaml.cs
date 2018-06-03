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
    /// Interaction logic for somthing.xaml
    /// </summary>
    public partial class somthing : Window
    {
        Additonals adds = new Additonals();
        public somthing(string username,int id)
        {
            InitializeComponent();

            if(id != 1)
            {
                listBox.ItemsSource = adds.getfollowers(username);
            }
            else
            {
                listBox.ItemsSource = adds.getfollowing(username);
            }
        }
    }
}
