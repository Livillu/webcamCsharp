using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Webcam
{
    public partial class player : Form
    {
        public player()
        {
            InitializeComponent();
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(listBox1.SelectedItem.ToString());
            axWindowsMediaPlayer1.URL = $".\\mpeg\\{listBox1.SelectedItem.ToString()}.mp4";
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void player_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            SqliteConnection connection = new SqliteConnection("Data Source=screen");
            connection.Open();
            SqliteCommand command = new SqliteCommand("select id from packgimg", connection);
            var dr= command.ExecuteReader();
            while (dr.Read())
            {
                listBox1.Items.Add(dr[0].ToString());
            }
        }
    }
}
