using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProjectYello
{
    public partial class Settings : Form
    {
        string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\ProjectYello\cache\";
        public string username
        {
            get { return this.richTextBox1.Text; }
        }

        public Settings(string qs, Image qp, string enroll, string password)
        {
            InitializeComponent();
            this.maskedTextBox3.PasswordChar = '*';
            this.maskedTextBox1.Text = qs;
            this.pictureBox1.Image = qp;
            this.maskedTextBox2.Text = enroll;
            this.maskedTextBox3.Text = password;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'database1DataSet.Settings' table. You can move, or remove it, as needed.
            
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "jpg files (*.jpg)|*.jpg";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.pictureBox1.Image = new Bitmap(dlg.FileName);
                    
                }
            }
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == null)
            {
                ControlID.TextData = "";
            }
            else
            {
                ControlID.TextData = maskedTextBox1.Text;
            }

            if (maskedTextBox2.Text == null)
            {
                EnrollID.TextData = "";
            }
            else
            {
                EnrollID.TextData = maskedTextBox2.Text;
            }

            if (maskedTextBox3.Text == null)
            {
                PinID.TextData = "";
            }
            else
            {
                PinID.TextData = maskedTextBox3.Text;
            }

            if (this.pictureBox1.Image == null)
            {
                ProfilePicID.pic = null;
            }
            else
            {
                ProfilePicID.pic = new Bitmap(this.pictureBox1.Image);
                Bitmap bmp = new Bitmap(this.pictureBox1.Image);
                bmp.Save(path + "ProfilePic" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.facebook.com/ProjectYello");
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            //ControlID.TextData = maskedTextBox1.Text;
            //ProfilePicID.pic = new Bitmap(this.pictureBox1.Image);
            //Bitmap bmp = new Bitmap(this.pictureBox1.Image);
            //bmp.Save(path + "ProfilePic" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            if (maskedTextBox1.Text == null)
            {
                ControlID.TextData = "";
            }
            else
            {
                ControlID.TextData = maskedTextBox1.Text;
            }
            if (this.pictureBox1.Image == null)
            {
                ProfilePicID.pic = null;
            }
            else
            {
                ProfilePicID.pic = new Bitmap(this.pictureBox1.Image);
                Bitmap bmp = new Bitmap(this.pictureBox1.Image);
                bmp.Save(path + "ProfilePic" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://projectyello.blogspot.in/#download");
        }
    }
    public static class ControlID
    {

        public static string TextData { get; set; }
    }
    public static class ProfilePicID
    {

        public static Bitmap pic { get; set; }
    }
    public static class EnrollID
    {

        public static string TextData { get; set; }
    }
    public static class PinID
    {

        public static string TextData { get; set; }
    }
}
