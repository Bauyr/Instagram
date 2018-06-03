using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NewAccountPage.xaml
    /// </summary>
    public partial class NewAccountPage : Window
    {
        Additonals adds = new Additonals();
        public NewAccountPage()
        {
            InitializeComponent();
        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Crate(object sender, RoutedEventArgs e)
        {
            List<TextBox> list = new List<TextBox>();
            list.Add(textBox);
            list.Add(textBox1);
            list.Add(textBox2);

            foreach (TextBox t in list)
            {
                if (t.Text == string.Empty)
                {
                    t.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    labelworn.Content = "";
                }
            }

            List<PasswordBox> list1 = new List<PasswordBox>();
            list1.Add(passwordBox);
            list1.Add(passwordBox1);

            foreach (PasswordBox p in list1)
            {
                if (p.Password == string.Empty)
                {
                    p.BorderBrush = System.Windows.Media.Brushes.Transparent;
                }
            }

            if (textBox.Text != string.Empty && textBox1.Text != string.Empty && textBox2.Text != string.Empty && passwordBox1.Password != string.Empty && passwordBox.Password != string.Empty)
            {
                if (!Has(textBox2.Text))
                {
                    if (passwordBox.Password.Length > 8)
                    {
                        if (passwordBox.Password == passwordBox1.Password)
                        {
                            string s = adds.createAccount(textBox.Text, textBox1.Text, textBox2.Text, passwordBox.Password, DateTime.Now.ToString("M/d/yyyy").ToString());
                            if (s == "OK")
                            {
                                MessageBox.Show("Successfully created!!!");
                                this.Close();
                            }
                            else
                            {
                                textBox2.Text = string.Empty;
                            }


                        }
                        else
                        {
                            MessageBox.Show("Confirm password doesn't match!!!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password must have minimum 8 digits");
                    }
                }
                else
                {
                    MessageBox.Show("Phone must have only numbers");
                }
            }
            else
            {
                List<TextBox> list11 = new List<TextBox>();
                list11.Add(textBox);
                list11.Add(textBox1);
                list11.Add(textBox2);

                foreach (TextBox t in list11)
                {
                    if (t.Text == string.Empty)
                    {
                        t.BorderBrush = System.Windows.Media.Brushes.Red;
                        labelworn.Content = "Fill all red blanks";
                    }
                }

                List<PasswordBox> list12 = new List<PasswordBox>();
                list12.Add(passwordBox);
                list12.Add(passwordBox1);

                foreach (PasswordBox p in list12)
                {
                    if (p.Password == string.Empty)
                    {
                        p.BorderBrush = System.Windows.Media.Brushes.Red;
                        labelworn.Content = "Fill all red blanks";
                    }
                }
            }
        }
        public static bool Has(string str)
        {
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();

            int index = str.IndexOfAny(specialCharactersArray);
            if (index == -1)
                return false;
            else
                return true;
        }

    }
}
