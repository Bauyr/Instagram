using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Instagram
{
    internal class Additonals
    {
        MySqlConnection conn;
        string myConnectionString = "server=127.0.0.1; uid=root; pwd=; database=insta;";
        static readonly string PasswordHash = "passwordbek";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        string fr = string.Empty;
        public Additonals()
        {
        }
        public string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public string login(string username, string pass)
        {
            string answer = "";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT password FROM users WHERE username = @username";
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = cmd.ExecuteReader();
                string p = "";

                while (reader.Read())
                {
                    p = Decrypt(reader[0].ToString());
                    if (pass == p)
                    {
                        answer = "OK";
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return answer;
        }
        public string createAccount(string name, string surname, string username, string password, string createdtime)
        {
            string answer = "";

            try
            {
                if (!check(username))
                {
                    conn = new MySqlConnection(myConnectionString);
                    conn.Open(); conn = new MySqlConnection(myConnectionString);
                    conn.Open();
                    MySqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "INSERT INTO users set " +
                                       "name = @name," +
                                       "surname = @surname, " +
                                       "username = @username, " +
                                       "password = @password, " +
                                       "status = @stat, " +
                                       "created_time = @created_time;"; 
                        
                    comm.Parameters.AddWithValue("@name", name);
                    comm.Parameters.AddWithValue("@surname", surname);
                    comm.Parameters.AddWithValue("@username", username);
                    comm.Parameters.AddWithValue("@password", Encrypt(password));
                    comm.Parameters.AddWithValue("@stat", "you can set you status clicking by edit button");
                    comm.Parameters.AddWithValue("@created_time", DateTime.Now);

                    comm.ExecuteNonQuery();
                    
                    answer = "OK";
                }
                else
                {
                    MessageBox.Show("This username " + username + " exists. Please create login using other username");
                }
                

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            conn.Close();

            return answer;
        }
        public bool check(string username)
        {
            bool s = false;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT username FROM users WHERE username = @username;";
                comm.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (username == (string)reader[0])
                    {
                        s = true;
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
            return s;
        }
        public int getid(string username)
        {
            int i = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select id from users where username = @user;";
                comm.Parameters.AddWithValue("@user", username);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    i = (int)reader[0];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return i;
        }
        public int getfollower(string username)
        {
            int i = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select follower_id from followers where user_id = (select id from users where username = @user)";
                comm.Parameters.AddWithValue("@users", username);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    i = (int)reader[0];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return i;
        }
        public int GetFollowersCount(string username)
        {
            int i = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select count(follower_id) from followers where user_id = (select id from users where username = @users)";
                comm.Parameters.AddWithValue("@users", username);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if(reader[0] is DBNull)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = int.Parse(reader[0].ToString()) ;
                    }
                    
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return i;
        }
        public int GetFollowingCount(string username)
        {
            int i = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select count(following_id) from following where user_id = (select id from users where username = @users)";
                comm.Parameters.AddWithValue("@users", username);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[0] is DBNull)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = int.Parse(reader[0].ToString());
                    }

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return i;
        }
        public void SetMyProfile(string username,Grid grid, RoutedEventHandler edit, RoutedEventHandler followerList, RoutedEventHandler followingList, RoutedEventHandler LikeMe)
        {
            grid.Children.Clear();
            grid.VerticalAlignment = VerticalAlignment.Top;

            int id = getid(username);
            System.Collections.Generic.List<Grid> list = posts(id,LikeMe);

            int i = 0;

            Grid gr = getMime(username,edit, followerList, followingList);
            Grid.SetRow(gr, i);
            i++;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Children.Add(gr);

            foreach (Grid g in list)
            {
                Grid.SetRow(g, i);
                i++;
                grid.RowDefinitions.Add(new RowDefinition());
                grid.HorizontalAlignment = HorizontalAlignment.Right;
                grid.Children.Add(g);
            }
        }
        public void foo(int users, Grid grid, RoutedEventHandler follow, RoutedEventHandler followerList, RoutedEventHandler followingList, RoutedEventHandler LikeMe,int own)
        {
            grid.Children.Clear();
            grid.VerticalAlignment = VerticalAlignment.Top;

            System.Collections.Generic.List<Grid> list = posts(users, LikeMe);

            int i = 0;
            string user = getusername(users);
            Grid gr = getMime(user, follow, followerList, followingList,1,own);
            Grid.SetRow(gr, i);
            i++;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Children.Add(gr);

            foreach (Grid g in list)
            {
                Grid.SetRow(g, i);
                i++;
                grid.RowDefinitions.Add(new RowDefinition());
                grid.HorizontalAlignment = HorizontalAlignment.Right;
                grid.Children.Add(g);
            }
        }
        public Grid getMime(string username, RoutedEventHandler edit, RoutedEventHandler followerList, RoutedEventHandler followingList,int xoxo = 0,int own = 0)
        {
            Grid gri = new Grid();

            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select photo,name,surname,status from users where username = @user";
                comm.Parameters.AddWithValue("@user", username);

                MySqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    int ii = 0;

                    Grid gr = new Grid();
                    gr.Background = System.Windows.Media.Brushes.Chocolate;
                    gr.Width = 365;

                    // image
                    Image img = new Image();
                    img.Width = 200;
                    img.Height = 200;
                    img.HorizontalAlignment = HorizontalAlignment.Center;

                    if(reader[0] is DBNull)
                    {

                    }
                    else
                    {
                        string photo = (string)reader[0];
                        img.Source = LoadImage(photo);

                        if (photo != null)
                        {
                            Grid.SetRow(img, ii);
                            ii++;
                            
                            gr.RowDefinitions.Add(new RowDefinition());
                            gr.Children.Add(img);
                        }
                    }
                    

                    //status
                    Label namesurname = new Label();
                    namesurname.HorizontalAlignment = HorizontalAlignment.Center;
                    namesurname.Content = (string)reader[1] + " " + (string)reader[2];

                    Grid.SetRow(namesurname, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.Children.Add(namesurname);

                    //status
                    Label stat = new Label();
                    stat.HorizontalAlignment = HorizontalAlignment.Center;
                    stat.Content = (string)reader[3];

                    Grid.SetRow(stat, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.Children.Add(stat);

                    Grid gro = new Grid();

                    Button follower = new Button();
                    follower.Background = System.Windows.Media.Brushes.Transparent;
                    follower.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    follower.Content = "      " + GetFollowersCount(username) + Environment.NewLine + "Followers";
                    follower.Click += followerList;
                    follower.Tag = 0;

                    Grid.SetColumn(follower, 0);
                    gro.ColumnDefinitions.Add(new ColumnDefinition());
                    gro.Children.Add(follower);

                    Button following = new Button();
                    following.Background = System.Windows.Media.Brushes.Transparent;
                    following.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    following.Content = "      " + GetFollowingCount(username) + Environment.NewLine + "Following";
                    following.Click += followingList;
                    following.Tag = 1;

                    Grid.SetColumn(following, 1);
                    gro.ColumnDefinitions.Add(new ColumnDefinition());
                    gro.Children.Add(following);

                    Grid.SetRow(gro, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.HorizontalAlignment = HorizontalAlignment.Center;
                    gr.Children.Add(gro);

                    //button edit
                    Button but = new Button();
                    but.Margin = new Thickness(0, 10,0,20);
                    but.Width = 200;
                    but.Height = 30;
                    but.HorizontalAlignment = HorizontalAlignment.Center;
                    but.Click += edit;
                    if(xoxo == 1)
                    {
                        if (!checkfollowing(own, getid(username)))
                        {
                            but.Content = "Follow";
                        }
                        else
                        {
                            but.Content = "you are following";
                            but.IsEnabled = false;
                        }
                        
                    }
                    else
                    {
                        but.Content = "Edit profile";
                    }
                    Grid.SetRow(but, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.HorizontalAlignment = HorizontalAlignment.Center;
                    gr.Children.Add(but);

                    gri = gr;

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Clone();

            return gri;
        }
        public System.Collections.Generic.List<Grid> posts(int user_id,RoutedEventHandler LikeMe)
        {
            System.Collections.Generic.List<Grid> list = new System.Collections.Generic.List<Grid>();

            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select posts.photo, posts.content, posts.likes, posts.created_time, users.username,posts.id from users inner join posts on posts.user_id = users.id where posts.user_id = @user";
                comm.Parameters.AddWithValue("@user", user_id);

                MySqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    int ii = 0;

                    Grid gr = new Grid();
                    gr.Background = System.Windows.Media.Brushes.Aqua;
                    gr.Width = 365;


                    // username + created time
                    Grid labs = new Grid();

                    Label le = new Label();
                    le.HorizontalAlignment = HorizontalAlignment.Left;
                    le.Content = (string)reader[4];

                    Label le2 = new Label();
                    le2.HorizontalAlignment = HorizontalAlignment.Right;
                    string s = reader[3].ToString();
                    char[] delimiterChars = {' '};
                    string[] words = s.Split(delimiterChars);
                    le2.Content = words[0];

                    Grid.SetColumn(le, 0);
                    Grid.SetColumn(le2, 1);

                    labs.ColumnDefinitions.Add(new ColumnDefinition());
                    labs.Children.Add(le);

                    labs.ColumnDefinitions.Add(new ColumnDefinition());
                    labs.Children.Add(le2);


                    Grid.SetRow(labs, ii);
                    ii++;

                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.HorizontalAlignment = HorizontalAlignment.Right;
                    gr.Children.Add(labs);

                    // image
                    Image img = new Image();
                    img.Width = 365;
                    img.Height = 300;
                    img.HorizontalAlignment = HorizontalAlignment.Center;

                    string photo = (string)reader[0];
                    img.Source = LoadImage(photo);

                    Grid.SetRow(img, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.HorizontalAlignment = HorizontalAlignment.Center;
                    gr.Children.Add(img);

                    //button like
                    Button but = new Button();
                    but.Width = 365;
                    but.Click += LikeMe;
                    but.Tag = ((int)reader[5]).ToString();
                    but.Content = "Likes: " + ((int)reader[2]).ToString();

                    Grid.SetRow(but, ii);
                    ii++;
                    gr.RowDefinitions.Add(new RowDefinition());
                    gr.HorizontalAlignment = HorizontalAlignment.Center;
                    gr.Children.Add(but);
                    

                    list.Add(gr);

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Clone();

            return list;
        }
        public BitmapImage LoadImage(string imageData)
        {
            if(imageData != string.Empty)
            {
                BitmapImage image = new BitmapImage(new Uri(imageData));
                return image;
            }
            else
            {
                BitmapImage image = null;
                return image;
            }
            
        }
        public byte[] ImageToArray(string image)
        {
            FileStream fS = new FileStream(image, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fS.Length];
            fS.Read(b, 0, (int)fS.Length);
            fS.Close();
            return b;
        }

        public void change(string user, string name, string surname, string password, string status,string photo)
        {
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "UPDATE users SET name = @name, " +
                                   "surname = @surname, " + 
                                   "password = @password, " +
                                   "status = @status, " +
                                   "updated_time = @updated_time, " + 
                                   "photo = @photo " + 
                                   "WHERE username = @user;";
                comm.Parameters.AddWithValue("@name", name);
                comm.Parameters.AddWithValue("@surname", surname);
                comm.Parameters.AddWithValue("@password", Encrypt(password));
                comm.Parameters.AddWithValue("@status", status);
                comm.Parameters.AddWithValue("@updated_time", DateTime.Now);
                comm.Parameters.AddWithValue("@photo", photo);
                comm.Parameters.AddWithValue("@user", user);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void getAllForChange(string username,string photo,Image img,TextBox name,TextBox surname,TextBox status,TextBox password,string photomoto)
        {
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = comm.CommandText = "select name,surname,status,password,photo from users where username = @users";
                comm.Parameters.AddWithValue("@users", username);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    name.Text = (string)reader[0];
                    surname.Text = (string)reader[1];
                    status.Text = (string)reader[2];
                    password.Text = Decrypt((string)reader[3]);

                    if (reader[4] is DBNull)
                    {
                        img.Source = null;
                    }
                    else
                    {
                        if(reader[4].ToString() != string.Empty)
                        {
                            photo = reader[4].ToString();
                            img.Source = new BitmapImage(new Uri((string)reader[4]));
                        }
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

        }
        public string getImage(string user)
        {
            string img = "";
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT photo FROM users WHERE username = @user;";
                comm.Parameters.AddWithValue("@user", user);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[0] is System.DBNull)
                    {
                        img = null;
                    }
                    else
                    {
                        img = reader[0].ToString();
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return img;
        }
        public byte[] getImageByBytes(string user)
        {
            byte[] ooo = null;

            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT photo FROM users WHERE username = @user;";
                comm.Parameters.AddWithValue("@user", user);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[0] is System.DBNull)
                    {
                        ooo = null;
                    }
                    else
                    {
                        ooo = (byte[])reader[0];
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return ooo;
        }
        public void SetUpload(string user,Grid grid, RoutedEventHandler opendialog, RoutedEventHandler uploadmake)
        {
            grid.Children.Clear();
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;

            int i = 0;
            Button open = new Button();
            open.Margin = new Thickness(0, 0, 0, 30);
            open.Content = "Choose photo";
            open.Click += opendialog;

            Grid.SetRow(open, i);
            i++;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Children.Add(open);

            Button up = new Button();
            up.Margin = new Thickness(0, 0, 0, 30);
            up.Content = "Upload";
            up.Click += uploadmake;

            Grid.SetRow(up, i);
            i++;
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Children.Add(up);

        }
        public void upload(int id, string content,string photo)
        {
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Insert into posts SET content = @con, "+
                                   "created_time = @updated_time, " +
                                   "likes = @like, " +
                                   "photo = @photo, " +
                                   "user_id = @user;";
                comm.Parameters.AddWithValue("@con", content);
                comm.Parameters.AddWithValue("@updated_time", DateTime.Now);
                comm.Parameters.AddWithValue("@photo", photo);
                comm.Parameters.AddWithValue("@user", id);
                comm.Parameters.AddWithValue("@like", 0);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void like(int id)
        {
            int likenum = selectLike(id);
            likenum++;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "update posts SET likes = @like " +
                                   "where id = @id;";
                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@like", likenum);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public int selectLike(int id)
        {
            int i = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT likes FROM posts WHERE id = @user;";
                comm.Parameters.AddWithValue("@user", id);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    i = (int)reader[0];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return i;
        }
        public void homeKit(int id,Grid grid,RoutedEventHandler likeme)
        {
            grid.Children.Clear();
            grid.VerticalAlignment = VerticalAlignment.Top;

            System.Collections.Generic.List<int> flling = getFollowing(id);
            
            foreach (int asd in flling)
            {
                System.Collections.Generic.List<Grid> list = posts(asd, likeme);
                int i = 0;
                foreach (Grid g in list)
                {
                    Grid.SetRow(g, i);
                    i++;
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                    grid.Children.Add(g);
                }
            }
            
        }
        public System.Collections.Generic.List<int> getFollowing(int id)
        {
            System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();

            list.Add(id);

            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT following_id FROM following WHERE user_id = @user;";
                comm.Parameters.AddWithValue("@user", id);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[0] is DBNull)
                    {

                    }
                    else
                    {
                        list.Add((int)reader[0]);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return list;
        }
        public int getAll()
        {
            int all = 0;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT count(id) from users;";
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[0] is DBNull)
                    {

                    }
                    else
                    {
                        all = int.Parse(reader[0].ToString());
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return all;
        }
        public void search(int id,Grid grid,RoutedEventHandler click)
        {
            grid.Children.Clear();
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            
            
            int users = getAll();

            for(int i = 0; i <= users; i++)
            {
                if (i != 0)
                {
                    Button but = new Button();
                    but.Height = 50;
                    but.Width = 365;
                    but.Background = System.Windows.Media.Brushes.White;
                    but.Content = getnamesurname(i);
                    but.Click += click;
                    but.Tag = i;

                    Grid.SetRow(but, i);
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.Children.Add(but);
                }
                else
                {
                    Label l = new Label();
                    l.HorizontalAlignment = HorizontalAlignment.Center;
                    l.Content = Environment.NewLine + "                                                 All users:";
                    l.Width = 365;
                    l.Height = 50;

                    Grid.SetRow(l, i);
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.Children.Add(l);
                }
            }
            
        }
        public string getnamesurname(int id)
        {
            string name = "";
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT username,name,surname from users where id = @id";
                comm.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    name = "(username: " + (string)reader[0] + ")"+ " " + (string)reader[1] + " " + (string)reader[2];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return name;
        }
        public string getusername(int id)
        {
            string name = "";
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT username from users where id = @id";
                comm.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    name = (string)reader[0];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return name;
        }
        public bool checkfollowing(int own,int following)
        {
            bool ret = false;
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {
                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT following_id from following where user_id = @id";
                comm.Parameters.AddWithValue("@id", own);
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if((int)reader[0] == following)
                    {
                        ret = true;
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();

            return ret;
        }
        public void dofollowing(int user,int ff)
        {
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Insert into following SET user_id = @user, " +
                                   "following_id = @ff;";
                comm.Parameters.AddWithValue("@user", user);
                comm.Parameters.AddWithValue("@ff", ff);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void dofollower(int user, int ff)
        {
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "Insert into followers SET user_id = @user, " +
                                   "following_id = @ff;";
                comm.Parameters.AddWithValue("@user", user);
                comm.Parameters.AddWithValue("@ff", ff);
                comm.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public System.Collections.Generic.List<string> getfollowers(string username)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "select follower_id from followers where user_id =(select id from users where username = @user)";
                comm.Parameters.AddWithValue("@user", username);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    string name = getnamesurname((int)reader[0]);
                    list.Add(name);
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }
        public System.Collections.Generic.List<string> getfollowing(string username)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            conn = new MySqlConnection(myConnectionString);
            conn.Open();
            try
            {

                MySqlCommand comm = conn.CreateCommand();
                comm.CommandText = "select following_id from following where user_id =(select id from users where username = @user)";
                comm.Parameters.AddWithValue("@user", username);

                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    string name = getnamesurname((int)reader[0]);
                    list.Add(name);
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }
    }
}