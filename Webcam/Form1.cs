using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Accord.Video.FFMPEG;
using AForge.Video.VFW;
using Microsoft.Data.Sqlite;
using System.IO;
using SQLitePCL;
namespace Webcam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
  
        private FilterInfoCollection VideoCaptureDevices=null;//所有攝影機
        private VideoCaptureDeviceForm captureDevice=null;//選擇使用中攝影鏡頭
        private Bitmap video;
        //private AVIWriter AVIwriter = new AVIWriter();
        private VideoFileWriter FileWriter = new VideoFileWriter();
        private SaveFileDialog saveAvi;
        private VideoCaptureDevice FinalVideo = null;
        private SqliteConnection connection=null;

        private void button1_Click(object sender, EventArgs e)
        {
            if (captureDevice.ShowDialog(this) == DialogResult.OK)
            {
                //VideoCaptureDevice videoSource = captureDevice.VideoDevice;
                FinalVideo = captureDevice.VideoDevice;
                FinalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
                FinalVideo.Start();
            }
        }

        void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (butStop.Text == "Stop Record")
            {
                video = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
                //AVIwriter.Quality = 0;
                FileWriter.WriteVideoFrame(video);
                //AVIwriter.AddFrame(video);
            }
            else
            {
                video = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            captureDevice = new VideoCaptureDeviceForm();

            if (File.Exists("screen"))
            {
                
                connection = new SqliteConnection("Data Source=screen");
                connection.Open();
                /*SqliteCommand command =new SqliteCommand("select count(*) from packgimg where id='20240704_160712'", connection);
                if (command.ExecuteScalar().ToString() == "0")
                {
                    command.CommandText = $"insert into packgimg(id,cdate) values('20240704_160712','{DateTime.Today.ToString("yyyyMMdd")}')";
                    command.ExecuteNonQuery();
                }*/
            }
            else {
                MessageBox.Show("open fail !!!");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (FinalVideo == null)
            { return; }
            if (FinalVideo.IsRunning)
            {
                this.FinalVideo.Stop();
                FileWriter.Close();
                //this.AVIwriter.Close();
            }
        }

        private void butStop_Click(object sender, EventArgs e)
        {
            if (FinalVideo == null) return;
            if (butStop.Text == "Stop Record")
            {
                butStop.Text = "Stop";
                if (FinalVideo == null)
                { return; }
                if (FinalVideo.IsRunning)
                {
                    //this.FinalVideo.Stop();
                    FileWriter.Close();
                    //this.AVIwriter.Close();
                    pictureBox1.Image = null;
                }
            }
            else
            {
                this.FinalVideo.Stop();
                FileWriter.Close();
                //this.AVIwriter.Close();
                pictureBox1.Image = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 8)
            {
                int h = captureDevice.VideoDevice.VideoResolution.FrameSize.Height;
                int w = captureDevice.VideoDevice.VideoResolution.FrameSize.Width;
                FileWriter.Open($".\\mpeg\\{textBox1.Text.Trim()}.mpeg", w, h, 25, VideoCodec.MPEG4, 5000000);
                FileWriter.WriteVideoFrame(video);
                butStop.Text = "Stop Record";
                textBox1.Text = "";
                /*
                saveAvi = new SaveFileDialog();
                //saveAvi.Filter = "Avi Files (*.avi)|*.avi";
                saveAvi.Filter = "MPEG4 Files (*.mpeg4)|*.mpeg4";
                if (saveAvi.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int h = captureDevice.VideoDevice.VideoResolution.FrameSize.Height;
                    int w = captureDevice.VideoDevice.VideoResolution.FrameSize.Width;
                    FileWriter.Open(saveAvi.FileName, w, h, 25, VideoCodec.MPEG4, 5000000);
                    FileWriter.WriteVideoFrame(video);
                    //AVIwriter.Open(saveAvi.FileName, w, h);
                    butStop.Text = "Stop Record";
                    textBox1.Text = "";
                }
                */
            }
            else
            {
                MessageBox.Show("輸入條碼!!!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Save("IMG" + DateTime.Now.ToString("hhmmss") + ".jpg");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            player fp=new player();
            fp.ShowDialog();
        }

        /*private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                button1.Text = "Stop";
                Cam.Start();   // WebCam starts capturing images.
            }
            else
            {
                button1.Text = "Start";
                Cam.Stop();  // WebCam stops capturing images.
            }
        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (button1.Text == "Stop")
            {
                video = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
                //AVIwriter.Quality = 0;
                FileWriter.WriteVideoFrame(video);
                //AVIwriter.AddFrame(video);
            }
            else
            {
                video = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            }
            //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDeviceForm();
            USB_Webcams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (USB_Webcams.Count > 0)  // The quantity of WebCam must be more than 0.
            {
                button1.Enabled = true;
                Cam = new VideoCaptureDevice(USB_Webcams[0].MonikerString);
                Cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                //Cam.NewFrame += Cam_NewFrame;//Press Tab  to   create
            }
            else
            {
                button1.Enabled = false;
                MessageBox.Show("No video input device is connected.");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Cam != null)
            {
                if (Cam.IsRunning)  // When Form1 closes itself, WebCam must stop, too.
                {
                    Cam.Stop();   // WebCam stops capturing images.
                }
            }
        }*/
    }
}
