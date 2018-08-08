using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VkPlayer
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://oauth.vk.com/authorize?client_id=6654531&display=page&redirect_uri=http://api.vk.com/blank.html&scope=photos&response_type=token&v=5.80&state=123456");
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStrip1.Text = "loading";

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = webBrowser1.Url.ToString();
            //"http://api.vk.com/blank.html#access_token=59f085841e87ea06c9168b4245016389245f42c4663f91175752833acdb6bae55713b270991a67e04eaee&expires_in=86400&user_id=185812&state=123456"
            if (url.Contains("access_token"))
            {
                toolStrip1.Text = "loaded";
                string l = url.Split('#')[1];
                Settings1.Default.token = l.Split('&')[0].Split('=')[1];
                var spl = l.Split('=');
                Settings1.Default.auth = true;
                Settings1.Default.id = spl[3].Split('&')[0];
                this.Close();
            }
        }
    }
}
