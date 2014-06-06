using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using Facebook;
using System.Web;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Threading;


namespace ProjectYello
{
    public partial class Form1 : Form
    {
        #region design
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            ShowScrollBar(this.tabPage3.Handle, (int)ScrollBarDirection.SB_HORZ, false);
            base.WndProc(ref m);
        }

        #endregion

        private DataTable dt;

        string accesstoken;
        string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\ProjectYello\cache\";
        //%appdata%
        string socialpath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\ProjectYello\cache\social\";
        string accesspath = System.Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + @"\ProjectYello\cache\";
        //%LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files\
        string fullpath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\ProjectYello\cache\socialfull\";
        public int count = 14;
        string searchterm = "juetguna";
        string enrollno;
        string pin;
        int fcount;

        WebBrowser webkiosk = new WebBrowser();
        LinkLabel[] links1 = new LinkLabel[14];
        PictureBox[] pbox1 = new PictureBox[14];
        RichTextBox[] rbox1 = new RichTextBox[14];

        public Form1()
        {
            InitializeComponent();

            #region folderexist
            bool isExists = System.IO.Directory.Exists(path);

            if (!isExists)
                System.IO.Directory.CreateDirectory(path);

            bool isExists2 = System.IO.Directory.Exists(socialpath);

            if (!isExists2)
                System.IO.Directory.CreateDirectory(socialpath);

            bool isExists1 = System.IO.Directory.Exists(fullpath);

            if (!isExists1)
                System.IO.Directory.CreateDirectory(fullpath);

            bool isExists3 = System.IO.Directory.Exists(accesspath);

            if (!isExists3)
                System.IO.Directory.CreateDirectory(accesspath);

            if (File.Exists(fullpath + "fblink0.rtf"))
            {
                fcount = Directory.GetFiles(fullpath, "fblinktext*.rtf", SearchOption.AllDirectories).Length;
            }
            else { fcount = 0; }

            #endregion

            var tab = new TabPadding(tabControl1);

            if (File.Exists(path + "Name.txt"))
            {
                StreamReader str = new StreamReader(path + "Name.txt");
                this.label6.Text = str.ReadLine();
                str.Close();
            }

            #region Updates
            if (File.Exists(path + "JUET.rtf"))
            {
                try
                {
                    richTextBox2.LoadFile(path + "JUET.rtf", RichTextBoxStreamType.RichText);
                }
                catch
                {
                    using (FileStream aFile = new FileStream(path + "JUET.rtf", FileMode.Create, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                    richTextBox2.Text = File.ReadAllText(path + "JUET.rtf");
                }
            }

            else
            {
                using (FileStream aFile = new FileStream(path + "JUET.rtf", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                richTextBox2.Text = File.ReadAllText(path + "JUET.rtf");
            }

            if (File.Exists(path + "Projects.rtf"))
            {
                try
                {
                    richTextBox1.LoadFile(path + "Projects.rtf", RichTextBoxStreamType.RichText);
                }
                catch
                {
                    using (FileStream aFile = new FileStream(path + "Projects.rtf", FileMode.Create, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                    richTextBox1.Text = File.ReadAllText(path + "Projects.rtf");
                }
            }
            else
            {
                using (FileStream aFile = new FileStream(path + "Projects.rtf", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                richTextBox1.Text = File.ReadAllText(path + "Projects.rtf");
            }

            RichTextBox[] richboxes = { richTextBox3, richTextBox4, richTextBox5, richTextBox6, richTextBox7, richTextBox11 };
            PictureBox[] boxes = { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox17 };
            for (int i = 0; i < 6; i++)
            {
                if (File.Exists(socialpath + "fb" + i + ".rtf"))
                {
                    richboxes[i].LoadFile(socialpath + "fb" + i + ".rtf", RichTextBoxStreamType.RichText);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (File.Exists(socialpath + "fb" + i + ".jpg"))
                {
                    using (FileStream fs = File.OpenRead(socialpath + "fb" + i + ".jpg"))
                    {
                        boxes[i].Image = Bitmap.FromStream(fs);
                    }
                }
            }
            picLoading.Hide();
            picLoading1.Hide();
            picLoading2.Hide();

            if (File.Exists(path + "ProfilePic.jpg"))
            {
                using (FileStream fs = File.OpenRead(path + "ProfilePic.jpg"))
                {
                    this.pictureBox14.Image = Bitmap.FromStream(fs);
                }
            }
            #endregion

            #region Projects
            this.picLoading4.Hide();
            if (File.Exists(path + "Projects3.rtf"))
            {
                try
                {
                    richTextBox10.LoadFile(path + "Projects3.rtf", RichTextBoxStreamType.RichText);
                }
                catch
                {
                    using (FileStream aFile = new FileStream(path + "Projects3.rtf", FileMode.Create, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                    richTextBox10.Text = File.ReadAllText(path + "Projects3.rtf");
                }
            }
            else
            {
                using (FileStream aFile = new FileStream(path + "Projects3.rtf", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(aFile)) { sw.WriteLine("Please Click on Refresh button to update.."); }
                richTextBox10.Text = File.ReadAllText(path + "Projects3.rtf");
            }

            #endregion

            #region FullUpdates

            if (File.Exists(accesspath + "web_access.bat"))
            {
                this.pictureBox18.Hide();
                string url = File.ReadAllText(accesspath + "web_access.bat");
                url = url.Remove(url.LastIndexOf("&expires"));
                url = url.Substring(url.IndexOf('=') + 1);

                accesstoken = url;
            }
            else
            {
                this.pictureBox18.Show();
                ((Control)this.tabPage3).Enabled = false;
            }


            this.picLoading3.Hide();
            if (File.Exists(fullpath + "fbmsg0.rtf"))
            {
                MakeSocial(fcount);
            }
            else
            {
                //this.picLoading3.Show();
                //MakeSocial();
            }

            #endregion

            #region webkiosk
            if (File.Exists(accesspath + "usr.bat"))
            {
                StreamReader str = new StreamReader(accesspath + "usr.bat");
                enrollno = str.ReadLine();
                str.Close();
            }
            if (File.Exists(accesspath + "piz.bat"))
            {
                StreamReader str = new StreamReader(accesspath + "piz.bat");
                pin = str.ReadLine();
                str.Close();
            }
            this.button6.Enabled = false;
            this.pictureBox19.Hide(); this.pictureBox20.Hide();
            this.pictureBox21.Hide(); this.pictureBox22.Hide(); this.pictureBox23.Hide();
            this.pictureBox24.Hide();

            this.webBrowser1.Hide();

            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                picLoading.Show();
                picLoading1.Show();
                picLoading2.Show();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        public void GetNews()
        {
            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                htmlCode = client.DownloadString("http://www.jiet.ac.in/");
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(htmlCode);
            dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            int count = 0;
            richTextBox2.DetectUrls = true;
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//div[@id='ann123']//table"))
            {

                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    DataRow dr = dt.NewRow();

                    foreach (HtmlNode cell in row.SelectNodes("td//a"))
                    {
                        string url = cell.GetAttributeValue("href", "unknown");
                        string pattern = @"\b \b";
                        string replace = "%20";
                        string result = Regex.Replace(url, pattern, replace);
                        int index1 = url.IndexOf("jiit");

                        if (result.Contains("http"))
                        {
                            dr["Value"] = cell.InnerText.Replace("&amp;", "&") + "\n" + result + "\n\n";
                        }
                        else
                        {
                            dr["Value"] = cell.InnerText.Replace("&amp;", "&");
                            string link = "\nhttp://www.jiet.ac.in/" + result + "\n\n";
                            dr["Value"] += link;
                        }

                        dt.Rows.Add(dr);
                        count++;
                    }
                }
                this.richTextBox2.Clear();

                foreach (DataRow dataRow in dt.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        if (item is string)
                        {
                            this.richTextBox2.Text += (string)item + "\n";
                        }
                    }
                }
            }
            this.richTextBox2.SaveFile(path + "JUET.rtf", RichTextBoxStreamType.RichText);

            picLoading.Hide();
        }

        public void GetProjects()
        {
            string Code = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                Code = client.DownloadString("http://projectyello.blogspot.in/p/projects.html");
            }
            HtmlAgilityPack.HtmlDocument over = new HtmlAgilityPack.HtmlDocument();

            over.LoadHtml(Code);

            this.richTextBox1.Clear();
            this.richTextBox1.Text += "Project's by student's------------------------------------\n\n";
            foreach (HtmlNode name in over.DocumentNode.SelectNodes("//div[@id='over']//ul"))
            {
                foreach (HtmlNode li in name.SelectNodes("li"))
                {
                this.richTextBox1.Text += "\n " + Regex.Replace(li.InnerText.Replace("&amp;", "&"), @"\s+", " ");
                }
                this.richTextBox1.Text += "\n\n\n\n";
            }

            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                htmlCode = client.DownloadString("http://www.jiet.ac.in/Research/SProjects.php");
            }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(htmlCode);

            //this.richTextBox1.Clear();
            this.richTextBox1.Text += "Project's by college--------------------------------------\n\n";
            foreach (HtmlNode name in doc.DocumentNode.SelectNodes("//table[@id='middletext']//td"))
            {
                foreach (HtmlNode p in name.SelectNodes("div//p"))
                {
                    this.richTextBox1.Text += Regex.Replace(p.InnerText.Replace("&amp;", "&"), @"\s+", " ");
                    this.richTextBox1.Text += "\n\n";
                }
            }
            int index1 = richTextBox1.Text.IndexOf("9");
            if (index1 > -1)
            {
                richTextBox1.Select(index1, 1);
                richTextBox1.SelectedText = "\n\n9";
            }
            richTextBox1.SaveFile(path + "Projects.rtf", RichTextBoxStreamType.RichText);
            picLoading1.Hide();
        }

        public void GetSocial()
        {
            #region elsecode
            #endregion

            if (string.IsNullOrEmpty(accesstoken))
            {
                throw new Exception("");
            }

            Dictionary<string, object> searchParams = new Dictionary<string, object>();
            searchParams.Add("type", "post");
            searchParams.Add("q", searchterm);
            searchParams.Add("limit", "6");
            FacebookClient fbClient = new FacebookClient(accesstoken);
            var rslt = fbClient.Get("/search", searchParams);

            var js = new JavaScriptSerializer();
            dynamic d = js.Deserialize<dynamic>(Convert.ToString(rslt));
            int cut = 50;

            //int x = ;

            RichTextBox[] richboxes = { richTextBox3, richTextBox4, richTextBox5, richTextBox6, richTextBox7, richTextBox11 };
            PictureBox[] boxes = { pictureBox0, pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox17 };
            LinkLabel[] links = { linkLabel1, linkLabel2, linkLabel3, linkLabel4, linkLabel5, linkLabel6 };

            try
            {
                for (int i = 0; i < 6; i++)
                {
                    links[i].Text = "";
                    links[i].Links.Clear();

                    string name;
                    if (d["data"][i]["from"]["name"] != null)
                    {
                        name = d["data"][i]["from"]["name"];
                    }
                    else
                    {
                        name = "null";
                    }

                    string message;
                    if (d["data"][i]["message"] != null)
                    {
                        message = d["data"][i]["message"];
                    }
                    else
                    {
                        message = "null";
                    }

                    string link = "https://facebook.com/" + d["data"][i]["from"]["id"];
                    links[i].Text = d["data"][i]["from"]["name"];
                    links[i].Links.Add(0, links[i].Text.Length, link);

                    Regex.Replace(message, @"\s+", " ");

                    if (message.Length > cut)
                    {
                        message = message.Remove(cut, message.Length - cut);
                        message = message.Insert(cut, "...");
                    }
                    richboxes[i].Clear();

                    richboxes[i].Text = message; // Joe

                    if (d["data"][i]["from"]["id"] != null)
                    {
                        var request = WebRequest.Create("https://graph.facebook.com/" + d["data"][i]["from"]["id"] + "/picture");

                        using (var response = request.GetResponse())
                        using (var stream = response.GetResponseStream())
                        {
                            boxes[i].Image = Bitmap.FromStream(stream);
                        }
                    }
                    else
                    {
                        boxes[i].Image = null;
                    }
                }
            }
            catch
            {
            }

            for (int i = 0; i < 6; i++)
            {
                string wordToFind = searchterm;
                int index = richboxes[i].Text.IndexOf(wordToFind);
                while (index != -1)
                {
                    richboxes[i].Select(index, wordToFind.Length);
                    richboxes[i].SelectionColor = Color.DodgerBlue;

                    index = richboxes[i].Text.IndexOf(wordToFind, index + wordToFind.Length);
                }
                richboxes[i].SaveFile(socialpath + "fb" + i + ".rtf", RichTextBoxStreamType.RichText);
            }
            for (int i = 0; i < 6; i++)
            {
                if (boxes[i].Image != null)
                {
                    boxes[i].Image.Save(socialpath + "fb" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                {
                }
            }
            picLoading2.Hide();
        }

        public void MakeSocial(int cou)
        {
            int y = 71;
            int z = 87;
            int diff = 80;
            for (int i = 0; i < cou; i++)
            {
                rbox1[i] = new RichTextBox();
                this.tabPage3.Controls.Add(rbox1[i]);
                rbox1[i].BorderStyle = System.Windows.Forms.BorderStyle.None;
                rbox1[i].Location = new System.Drawing.Point(86, z);
                rbox1[i].Name = "rbox" + i;
                rbox1[i].Size = new System.Drawing.Size(630, 59);
                rbox1[i].Text = "";
                rbox1[i].ReadOnly = true;
                z += diff;
            }

            for (int i = 0; i < cou; i++)
            {
                links1[i] = new LinkLabel();
                this.tabPage3.Controls.Add(links1[i]);
                links1[i].AutoSize = true;
                links1[i].Location = new System.Drawing.Point(86, y);
                links1[i].Name = "links" + i;
                links1[i].Size = new System.Drawing.Size(55, 13);
                links1[i].Text = "";
                links1[i].LinkColor = Color.SteelBlue;
                links1[i].LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
                links1[i].Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                links1[i].LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
                y += diff;
            }
            int x = 71;
            for (int i = 0; i < cou; i++)
            {
                pbox1[i] = new PictureBox();
                pbox1[i].Image = null;
                this.tabPage3.Controls.Add(pbox1[i]);
                pbox1[i].Location = new System.Drawing.Point(32, x);
                pbox1[i].Name = "pbox" + i;
                //pbox1[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pbox1[i].Size = new System.Drawing.Size(48, 40);
                x += diff;
            }

            for (int i = 0; i < cou; i++)
            {
                if (File.Exists(fullpath + "fbmsg" + i + ".rtf"))
                {
                    rbox1[i].Text = File.ReadAllText(fullpath + "fbmsg" + i + ".rtf");
                }
            }

            for (int i = 0; i < cou; i++)
            {
                if (File.Exists(fullpath + "fb" + i + ".jpg"))
                {
                    using (FileStream fs = File.OpenRead(fullpath + "fb" + i + ".jpg"))
                    {
                        pbox1[i].Image = Bitmap.FromStream(fs);
                    }
                }
            }
            for (int i = 0; i < cou; i++)
            {
                if (File.Exists(fullpath + "fblinktext" + i + ".rtf"))
                {
                    StreamReader strlink = new StreamReader(fullpath + "fblink" + i + ".rtf");
                    string link = strlink.ReadToEnd();
                    strlink.Close();

                    StreamReader strname = new StreamReader(fullpath + "fblinktext" + i + ".rtf");
                    string name = strname.ReadToEnd();
                    strlink.Close();

                    links1[i].Text = name;
                    links1[i].Links.Add(0, links1[i].Text.Length, link);
                }
            }

            this.tabPage3.AutoScroll = true;
            this.tabPage3.AutoScrollMargin = new System.Drawing.Size(20, 20);
            this.tabPage3.HorizontalScroll.Enabled = false;
            this.tabPage3.HorizontalScroll.Visible = false;
            this.tabPage3.AutoScrollMinSize = new System.Drawing.Size(this.tabPage3.Width, this.tabPage3.Height);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(target);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                System.Threading.Thread.Sleep(500);

                GetNews();
                GetProjects();
                GetSocial();
                // Perform a time consuming operation and report progress.
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label4.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled == true)
            {
                label4.Text = "Canceled!";
            }

            else if (e.Error != null)
            {

                if (e.Error.Message.StartsWith("(OAuthException - #190)"))
                {
                    facebook fb = new facebook();
                    fb.ShowDialog();
                    string url = File.ReadAllText(path + "web_access.txt");
                    url = url.Remove(url.LastIndexOf("&expires"));
                    url = url.Substring(url.IndexOf('=') + 1);

                    accesstoken = url;
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {

                    label4.Text = e.Error.Message;
                    this.picLoading.Hide();
                    this.picLoading1.Hide();
                    this.picLoading2.Hide();
                }
            }

            else
            {
                label4.Text = "Done!";
            }
            //timer1.Stop();
        }

        #region Design



        private void rectangleShape1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                ReleaseCapture();

                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);

            }
        }


        private void richTextBox2_LinkClicked_1(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            this.textBox1.Font = new Font(textBox1.Font.FontFamily, 10);
            this.textBox1.ForeColor = Color.Black;
            this.textBox1.Clear();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

            this.textBox1.Font = new Font(textBox1.Font.FontFamily, 18);
            this.textBox1.ForeColor = Color.DarkGray;
            this.textBox1.Text = "What's happening? #juetguna";
        }

        #region tabs
        private void pictureBox7_MouseClick(object sender, MouseEventArgs e)
        {
            this.pictureBox7.BackColor = Color.DodgerBlue;
            this.tabPage3.Hide();
            this.tabPage1.Hide();
            this.tabPage2.Show();
        }

        private void pictureBox5_MouseClick(object sender, MouseEventArgs e)
        {
            this.pictureBox5.BackColor = Color.DodgerBlue;
            this.tabPage3.Hide();
            this.tabPage2.Hide();
            this.tabPage1.Show();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.tabPage1.Hide();
            this.tabPage2.Hide();
            this.tabPage3.Show();
        }

        private void pictureBox7_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox7.BackColor = Color.LightBlue;
            this.pictureBox7.Location = new Point(+2, this.pictureBox7.Location.Y);
        }

        private void pictureBox7_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox7.BackColor = Color.White;
            this.pictureBox7.Location = new Point(1, this.pictureBox7.Location.Y);
        }

        private void pictureBox6_MouseClick(object sender, MouseEventArgs e)
        {
            this.pictureBox6.BackColor = Color.DodgerBlue;
        }

        private void pictureBox5_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox5.BackColor = Color.LightBlue;
            this.pictureBox5.Location = new Point(+2, this.pictureBox5.Location.Y);
        }

        private void pictureBox6_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox6.BackColor = Color.LightBlue;
            this.pictureBox6.Location = new Point(+2, this.pictureBox6.Location.Y);
        }

        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox6.BackColor = Color.White;
            this.pictureBox6.Location = new Point(1, this.pictureBox6.Location.Y);
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox5.BackColor = Color.White;
            this.pictureBox5.Location = new Point(1, this.pictureBox5.Location.Y);
        }

        private void pictureBox8_MouseClick(object sender, MouseEventArgs e)
        {
            this.pictureBox8.BackColor = Color.DodgerBlue;
        }
        private void pictureBox8_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox8.BackColor = Color.LightBlue;
            this.pictureBox8.Location = new Point(+2, this.pictureBox8.Location.Y);
        }

        private void pictureBox8_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox8.BackColor = Color.Cornsilk;
            this.pictureBox8.Location = new Point(1, this.pictureBox8.Location.Y);
        }

        #endregion

        private void pictureBox12_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox12.BackColor = Color.Black;
        }

        private void pictureBox12_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox12.BackColor = Color.SteelBlue;
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.facebook.com/dialog/feed?app_id=358249084301839&name=Facebook%20Dialogs&caption=ProjectYello.&redirect_uri=https://facebook.com/&message=" + textBox1.Text.Replace(" ", "+"));
        }

        private void pictureBox13_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox13.BackColor = Color.Black;
        }

        private void pictureBox13_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox13.BackColor = Color.DeepSkyBlue;
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("#juetguna"))
            {
                int index = textBox1.Text.IndexOf("#juetguna");
                textBox1.Text = textBox1.Text.Remove(index, 9);
                System.Diagnostics.Process.Start("https://twitter.com/intent/tweet?text=" + textBox1.Text + "%20%23juetguna");
            }
            else
            {
                System.Diagnostics.Process.Start("https://twitter.com/intent/tweet?text=" + textBox1.Text + "%20%23juetguna");
            }
        }


        private void pictureBox14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(path + "ProfilePic.jpg");
        }

        private void rectangleShape6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                ReleaseCapture();

                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);

            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://projectyello.blogspot.in");
        }

        private void pictureBox10_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox10.Location = new Point(this.pictureBox10.Location.X, this.pictureBox10.Location.Y - 1);
        }

        private void pictureBox10_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox10.Location = new Point(2, 2);
        }

        private void pictureBox9_MouseHover(object sender, EventArgs e)
        {
            this.pictureBox9.BackColor = Color.SlateGray;
        }

        private void pictureBox9_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox9.BackColor = Color.LightSteelBlue;
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            this.label5.BackColor = Color.Gainsboro;
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            this.label5.BackColor = Color.White;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label7_MouseHover(object sender, EventArgs e)
        {
            this.label7.BackColor = Color.Gainsboro;
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            this.label7.BackColor = Color.White;
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.google.com/#sclient=psy-ab&q=" + this.textBox2.Text + "+site:jiet.ac.in");
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            this.textBox2.ForeColor = Color.Black;
            this.textBox2.Clear();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            this.textBox2.ForeColor = Color.Gray;
            this.textBox2.Text = "Search jiet.ac.in...";
        }


        #endregion

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            this.pictureBox9.BackColor = Color.SlateGray;
            Settings f2 = new Settings(label6.Text, this.pictureBox14.Image, enrollno, pin);
            f2.ShowDialog();
            this.label6.Text = ControlID.TextData;
            enrollno = EnrollID.TextData;
            pin = PinID.TextData;
            this.pictureBox14.Image = ProfilePicID.pic;

            StreamWriter stw = new StreamWriter(path + "Name.txt");
            stw.WriteLine(this.label6.Text);
            stw.Close();

            StreamWriter usn = new StreamWriter(accesspath + "usr.bat");
            usn.WriteLine(enrollno);
            usn.Close();

            StreamWriter piz = new StreamWriter(accesspath + "piz.bat");
            piz.WriteLine(pin);
            piz.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            add_project f2 = new add_project();
            f2.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.picLoading3.Show();

            if (this.tabPage3.Controls.Contains(rbox1[13]))
            {
                //do nothing
            }
            else
            {
                for (int i = 0; i < fcount; i++)
                {
                    this.tabPage3.Controls.Remove(rbox1[i]);
                    this.tabPage3.Controls.Remove(pbox1[i]);
                }
                MakeSocial(count);
            }

            if (backgroundWorker3.IsBusy != true)
            {
                this.picLoading3.Show();
                // Start the asynchronous operation.
                backgroundWorker3.RunWorkerAsync();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker2.IsBusy != true)
            {
                // Start the asynchronous operation.
                picLoading4.Show();
                backgroundWorker2.RunWorkerAsync();
            }

        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://webkiosk.juet.ac.in/");
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            this.richTextBox10.Clear();

            string link = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                link = client.DownloadString("http://projectyello.blogspot.in/p/projects.html");
            }
            HtmlAgilityPack.HtmlDocument over = new HtmlAgilityPack.HtmlDocument();

            over.LoadHtml(link);
            string link1 = "";

            foreach (HtmlNode name in over.DocumentNode.SelectNodes("//div[@id='link']"))
            {
                link1 = name.InnerText;
            }

            using (WebClient Client = new WebClient())
            {
                Client.DownloadFile(link1, path + "Projects3.rtf");
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.richTextBox10.LoadFile(path + "Projects3.rtf");
            this.picLoading4.Hide();
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {

            WebClient client = new WebClient();
            string getString = client.DownloadString("https://graph.facebook.com/search?q=" + searchterm + "&type=post&limit=" + count + "&access_token=" + accesstoken);
            var js = new JavaScriptSerializer();
            var d = js.Deserialize<dynamic>(getString);
            for (int i = 0; i < count; i++)
            {
                links1[i].Text = "";
                links1[i].Links.Clear();

                string name;
                if (d["data"][i]["from"]["name"] != null)
                {
                    name = d["data"][i]["from"]["name"];
                }
                else
                {
                    name = "null";
                }

                string message;
                if (d["data"][i]["message"] != null)
                {
                    message = d["data"][i]["message"];
                }
                else
                {
                    message = "null";
                }

                string link;
                if (d["data"][i]["from"]["id"] != null)
                {
                    link = "https://facebook.com/" + d["data"][i]["from"]["id"];
                }
                else
                {
                    link = "https://facebook.com/";
                }

                System.IO.StreamWriter file = new System.IO.StreamWriter(fullpath + "fblinktext" + i + ".rtf");
                file.WriteLine(name);
                file.Close();

                System.IO.StreamWriter file1 = new System.IO.StreamWriter(fullpath + "fblink" + i + ".rtf");
                file1.WriteLine(link);
                file1.Close();

                System.IO.StreamWriter file2 = new System.IO.StreamWriter(fullpath + "fbmsg" + i + ".rtf");
                file2.WriteLine(message);
                file2.Close();

                if (d["data"][i]["from"]["id"] != null)
                {
                    var request = WebRequest.Create("https://graph.facebook.com/" + d["data"][i]["from"]["id"] + "/picture");

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        var bitmap = new Bitmap(stream);
                        bitmap.Save(fullpath + "fb" + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else
                {

                }
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    links1[i].Text = "";
                    links1[i].Links.Clear();

                    rbox1[i].Text = File.ReadAllText(fullpath + "fbmsg" + i + ".rtf");
                    pbox1[i].Image = Bitmap.FromFile(fullpath + "fb" + i + ".jpg");

                    StreamReader strlink = new StreamReader(fullpath + "fblink" + i + ".rtf");
                    string link = strlink.ReadToEnd();
                    strlink.Close();

                    StreamReader strname = new StreamReader(fullpath + "fblinktext" + i + ".rtf");
                    string name = strname.ReadToEnd();
                    strlink.Close();

                    links1[i].Text = name;
                    links1[i].Links.Add(0, links1[i].Text.Length, link);
                }
            }
            catch
            {
            }
            this.picLoading3.Hide();
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {

            facebook fb = new facebook();
            fb.ShowDialog();
            this.pictureBox18.Hide();
            try
            {
                string url = File.ReadAllText(accesspath + "web_access.txt");
                url = url.Remove(url.LastIndexOf("&expires"));
                url = url.Substring(url.IndexOf('=') + 1);

                accesstoken = url;
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    picLoading.Show();
                    picLoading1.Show();
                    picLoading2.Show();
                    backgroundWorker1.RunWorkerAsync();
                }
                ((Control)this.tabPage3).Enabled = true;
            }
            catch
            {
            }
        }

        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        #region webkisok

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            this.webkiosk.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webkiosk_DocumentCompleted);
            if (enrollno==null)
            {
                MessageBox.Show("Please enter your Enrollment No. and Password in Settings");
            }
            else
            {
                this.pictureBox19.Show();
                this.tabPage1.Hide();
                this.tabPage2.Hide();
                this.tabPage3.Hide();
                this.tabPage4.Show();
                string postData = string.Format("txtInst=Institute&InstCode=JUET&txtuType=Member%2BType&UserType=S&txtCode=Enrollment%2BNo&MemberCode={0}&txtPin=Password%2FPin&Password={1}&BTNSubmit=Submit", enrollno, pin);

                ASCIIEncoding enc = new ASCIIEncoding();
                //  we are encoding the postData to a byte array
                webkiosk.Navigate("https://webkiosk.juet.ac.in/CommonFiles/UserAction.jsp", "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
            }
        }

        private void webkiosk_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath)
            {
                webkiosk.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted1);
                webkiosk.Navigate("https://webkiosk.juet.ac.in/StudentFiles/Academic/StudentAttendanceList.jsp");
            }

            //The page is finished loading 
        }

        private void webBrowser1_DocumentCompleted1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            this.button6.Enabled = true;

            this.richTextBox13.Clear();
            this.richTextBox14.Clear();
            this.richTextBox15.Clear();
            this.richTextBox16.Clear();
            this.richTextBox17.Clear();
            this.richTextBox18.Clear();


            string Code = webkiosk.DocumentText;

            HtmlAgilityPack.HtmlDocument over = new HtmlAgilityPack.HtmlDocument();

            over.LoadHtml(Code);
            //this.richTextBox12.Clear();

            #region details
            foreach (HtmlNode name in over.DocumentNode.SelectNodes("//table[@rules='groups']//tr[1]//td[1]"))
            {
                string text = name.InnerText;
                text = text.Remove(text.LastIndexOf("["));
                text = text.Substring(text.IndexOf(':') + 1);
                this.richTextBox14.Text = Regex.Replace(text.Replace("&nbsp;", " "), @"\s+", " ");
            }

            foreach (HtmlNode name in over.DocumentNode.SelectNodes("//table[@rules='groups']//tr[1]"))
            {
                string eno, course;
                foreach (HtmlNode tap in name.SelectNodes("td[1]"))
                {
                    eno = Regex.Replace(tap.InnerText.Replace("&nbsp;", " "), @"\s+", " ");
                    eno = eno.Remove(eno.LastIndexOf("]"));
                    eno = eno.Substring(eno.IndexOf('[') + 1);
                    this.richTextBox12.Text = " " + eno + ", ";
                }
                foreach (HtmlNode tap in name.SelectNodes("td[2]"))
                {
                    course = Regex.Replace(tap.InnerText.Replace("&nbsp;", " "), @"\s+", " ");
                    course = course.Substring(course.IndexOf(": ") + 1);
                    this.richTextBox12.Text += course + ", ";
                }
                foreach (HtmlNode tap in name.SelectNodes("td[3]"))
                {
                    this.richTextBox12.Text += Regex.Replace(tap.InnerText.Replace("&nbsp;", " "), @"\s+", " ");
                }
            }
            #endregion

            #region attendence
            foreach (HtmlNode name in over.DocumentNode.SelectNodes("//table[@class='sort-table']/tbody"))
            {
                foreach (HtmlNode tap in name.SelectNodes("tr"))
                {
                    foreach (HtmlNode node in tap.SelectNodes("td[2]"))
                    {
                        string sub = Regex.Replace(node.InnerText.Replace("&nbsp;", " "), @"\s+", " ");
                        string code = sub.Substring(sub.IndexOf('-') + 1);
                        string subject = sub.Remove(sub.LastIndexOf("-"));
                        this.richTextBox13.Text += "\n\n" + subject;
                    }

                    foreach (HtmlNode node in tap.SelectNodes("td[3]"))
                    {
                        if (node.InnerText == "&nbsp;")
                        {
                            string percent = Regex.Replace(node.InnerText.Replace("&nbsp;", "\n\n"), @"\s+", " ");
                            this.richTextBox15.Text += "\n\n" + percent;
                        }
                        else
                        {
                            string url = node.GetAttributeValue("href", "unknown");
                            string link = "https://webkiosk.juet.ac.in/StudentFiles/Academic/" + url;

                            string percent = Regex.Replace(node.InnerText.Replace("&nbsp;", "\n\n"), @"\s+", " ");
                            this.richTextBox15.Text += "\n\n" + percent;
                        }
                    }

                    foreach (HtmlNode node in tap.SelectNodes("td[4]"))
                    {
                        string percent = Regex.Replace(node.InnerText.Replace("&nbsp;", " "), @"\s+", " ");
                        this.richTextBox16.Text += "\n\n" + percent;
                    }

                    foreach (HtmlNode node in tap.SelectNodes("td[5]"))
                    {
                        string percent = Regex.Replace(node.InnerText.Replace("&nbsp;", "\n\n"), @"\s+", " ");
                        this.richTextBox17.Text += "\n\n" + percent;
                    }

                    foreach (HtmlNode node in tap.SelectNodes("td[6]"))
                    {
                        string percent = Regex.Replace(node.InnerText.Replace("&nbsp;", "\n\n"), @"\s+", " ");
                        this.richTextBox18.Text += "\n\n" + percent;
                    }
                }
            }
            #endregion


            this.pictureBox19.Hide();
            this.pictureBox20.Show(); this.pictureBox21.Show(); this.pictureBox22.Show();
            this.pictureBox23.Show(); this.pictureBox24.Show(); this.pictureBox25.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.pictureBox19.Show();
            string postData = string.Format("txtInst=Institute&InstCode=JUET&txtuType=Member%2BType&UserType=S&txtCode=Enrollment%2BNo&MemberCode={0}&txtPin=Password%2FPin&Password={1}&BTNSubmit=Submit", enrollno, pin);

            ASCIIEncoding enc = new ASCIIEncoding();
            //  we are encoding the postData to a byte array
            webkiosk.Navigate("https://webkiosk.juet.ac.in/CommonFiles/UserAction.jsp", "", enc.GetBytes(postData), "Content-Type: application/x-www-form-urlencoded\r\n");
            //do some shit
        }

        #endregion

        public class TabPadding : NativeWindow
        {
            private const int WM_PAINT = 0xF;

            private TabControl tabControl;

            public TabPadding(TabControl tc)
            {
                tabControl = tc;
                tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
                AssignHandle(tc.Handle);
            }

            void tabControl_Selected(object sender, TabControlEventArgs e)
            {
                tabControl.Invalidate();
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_PAINT)
                {
                    using (Graphics g = Graphics.FromHwnd(m.HWnd))
                    {

                        //Replace the outside white borders:
                        if (tabControl.Parent != null)
                        {
                            g.SetClip(new Rectangle(0, 0, tabControl.Width - 3, tabControl.Height - 6), System.Drawing.Drawing2D.CombineMode.Exclude);
                            using (SolidBrush sb = new SolidBrush(tabControl.Parent.BackColor))
                                g.FillRectangle(sb, new Rectangle(0,
                                                                  tabControl.ItemSize.Height + 2,
                                                                  tabControl.Width,
                                                                  tabControl.Height - (tabControl.ItemSize.Height + 2)));
                        }

                        //Replace the inside white borders:
                        if (tabControl.SelectedTab != null)
                        {
                            g.ResetClip();
                            Rectangle r = tabControl.SelectedTab.Bounds;
                            g.SetClip(r, System.Drawing.Drawing2D.CombineMode.Exclude);
                            using (SolidBrush sb = new SolidBrush(tabControl.SelectedTab.BackColor))
                                g.FillRectangle(sb, new Rectangle(r.Left - 5,
                                                                  r.Top - 4,
                                                                  r.Width + 4,
                                                                  r.Height + 3));
                        }
                    }
                }
            }


        }

    }
}
