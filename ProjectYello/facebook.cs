using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Facebook;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectYello
{

    public partial class facebook : Form
    {
        public FacebookOAuthResult _FacebookOAuthResult { get; private set; }
        string accesspath = System.Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) + @"\ProjectYello\cache\";
      
        public facebook()
        {
            InitializeComponent();
            string client_id = "#";
            string redirect_uri = "#";
            string link = string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&response_type=token&scope=publish_stream&display=touch",client_id, redirect_uri);
            webBrowser1.Navigate(link);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.pictureBox1.Hide();
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.pictureBox1.Hide();
            try
            {
                if (webBrowser1.Url.OriginalString.StartsWith("http://projectyello.blogspot.in/2013/06/project-yello-for-desktop.html"))
                {
                    string redirect_url = webBrowser1.Url.OriginalString;

                    System.IO.StreamWriter file = new System.IO.StreamWriter(accesspath + "web_access.bat");
                    file.WriteLine(redirect_url);
                    file.Close();

                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
