using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace VkPlayer
{
    public partial class Form1 : Form
    {
        public List<Video> videolist=new List<Video>();
        public List<Photoalbum> albumlist = new List<Photoalbum>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AuthForm f = new AuthForm();
            f.Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth) { Thread.Sleep(1000); }
            
        }
        public class Photoalbum
        {
            public int id;
            public string title;
            public int size;
        }

        public class Photo
        {
            public int id;
            public int album_id;
            public int owner_id;
            /// <summary>
            /// who posted photo
            /// </summary>
            public int user_id;
        }

        public class Video
        {
            public int id;
            public int owner_id;
            public string title;
            public string player;
            public string access_key;
            public string description;
            public int duration;
            public int date;
            public int comments;
            public int views;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Settings1.Default.auth)
            {
                WebRequest req = WebRequest.Create("https://api.vk.com/method/photos.getAlbums?owner_id=" + Settings1.Default.id + "&access_token=" + Settings1.Default.token + "&v=5.80");
                WebResponse resp = req.GetResponse();
                Stream data = resp.GetResponseStream();
                StreamReader reader = new StreamReader(data);
                string response = reader.ReadToEnd();
                reader.Close();
                resp.Close();
                string DecodedResponse = HttpUtility.HtmlDecode(response);

                JToken token = JToken.Parse(DecodedResponse);
                var list = token["response"].Children().Skip(1).ToList();
                foreach (var item in list)
                {
                    var itCh = item.Children().Children();
                    foreach (var ot in itCh)
                    {
                        var ph = new Photoalbum
                        {
                            id = (int)ot["id"],
                            title = (string)ot["title"],
                            size = (int)ot["size"]
                        };
                        albumlist.Add(ph);
                        //var v = new Video
                        //{
                        //    id = (int)ot["id"],
                        //    title =(string)ot["title"],
                        //     player = (string)ot["player"],
                        //     access_key = (string)ot["access_key"],
                        //    description = (string)ot["description"]
                        //};
                        //videolist.Add(v);
                    }

                }
                listBox1.Items.Clear();
                //videolist = list;
                foreach (var item in albumlist)
                {
                    listBox1.Items.Add(item.title);
                }
            }


            //if (Settings1.Default.auth)
            //{
            //    WebRequest req = WebRequest.Create("https://api.vk.com/method/video.get?owner_id=" + Settings1.Default.id+ "&access_token=" + Settings1.Default.token+"&v=5.80");
            //    WebResponse resp = req.GetResponse();
            //    Stream data = resp.GetResponseStream();
            //    StreamReader reader = new StreamReader(data);
            //    string response = reader.ReadToEnd();
            //    reader.Close();
            //    resp.Close();
            //    string DecodedResponse = HttpUtility.HtmlDecode(response);

            //    JToken token = JToken.Parse(DecodedResponse);
            //    var list = token["response"].Children().Skip(1).ToList();
            //    foreach (var item in list)
            //    {
            //        var itCh = item.Children().Children();
            //        foreach (var ot in itCh)
            //        {
            //            //var v = new Video
            //            //{
            //            //    id = (int)ot["id"],
            //            //    title =(string)ot["title"],
            //            //     player = (string)ot["player"],
            //            //     access_key = (string)ot["access_key"],
            //            //    description = (string)ot["description"]
            //            //};
            //            //videolist.Add(v);
            //        }

            //    }
            //    listBox1.Items.Clear();
            //    //videolist = list;
            //    foreach (var item in videolist)
            //    {
            //        listBox1.Items.Add(item.title);
            //    }
            //}

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem.ToString();
            var album = albumlist.Where(it => it.title == item).FirstOrDefault();
            currAlbumId = album.id;
            getPhotoFromAlbum(album);
            //var video = videolist.Where(it => it.title == item).FirstOrDefault();
            //webBrowser1.Navigate(video.player);
            //Process.Start(video.player);
        }

        Dictionary<int, List<Photo> > photosInAlbum = new Dictionary<int, List<Photo> >();
        Dictionary<int, int> offsets = new Dictionary<int, int>();
        int currAlbumId = 0;

        void getPhotoFromAlbum(Photoalbum album, int offset= 0 )
        {
            List<Photo> photosInAlbumL = new List<Photo>();

            WebRequest req = WebRequest.Create("https://api.vk.com/method/photos.get?owner_id=" + Settings1.Default.id + "&album_id="+album.id + "&access_token=" + Settings1.Default.token + "&v=5.80&rev=0");
            WebResponse resp = req.GetResponse();
            Stream data = resp.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            string response = reader.ReadToEnd();
            reader.Close();
            resp.Close();
            string DecodedResponse = HttpUtility.HtmlDecode(response);

            JToken token = JToken.Parse(DecodedResponse);
            var list = token["response"].Children().Skip(1).ToList();
            foreach (var item in list)
            {
                var itCh = item.Children().Children();
                foreach (var ot in itCh)
                {
                    var ph = new Photo
                    {
                        id = (int)ot["id"],
                        album_id = (int)ot["album_id"],
                        //user_id = (int)ot["user_id"],
                        owner_id = (int)ot["owner_id"]
                    };
                    photosInAlbumL.Add(ph);
                    
                    //photosInAlbum.Add(ph);
                    //albumlist.Add(ph);
                    //var v = new Video
                    //{
                    //    id = (int)ot["id"],
                    //    title =(string)ot["title"],
                    //     player = (string)ot["player"],
                    //     access_key = (string)ot["access_key"],
                    //    description = (string)ot["description"]
                    //};
                    //videolist.Add(v);
                }

            }
            if (photosInAlbum.ContainsKey(album.id))
            {
                photosInAlbum[album.id] = photosInAlbumL;
                offsets[album.id] = 0;
            }
            else
            {
                photosInAlbum.Add(album.id, photosInAlbumL);
                offsets.Add(album.id, 0);
            }
            loadImage(album);
            label1.Text = "Photo #1 of " + album.size;
        }
        void loadImage(Photoalbum album, int offset = 0)
        {
            //load first image
            if (offset >= photosInAlbum[album.id].Count)
            {
                return;
            }
            var firstImage = photosInAlbum[album.id].ElementAt(offset);
            string photoId = firstImage.owner_id.ToString() + "_" + firstImage.id.ToString();
            //get photo
            WebRequest req = WebRequest.Create("https://api.vk.com/method/photos.getById?photos=" + photoId + "&access_token=" + Settings1.Default.token + "&v=5.80");
            WebResponse resp = req.GetResponse();
            Stream data = resp.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            string response = reader.ReadToEnd();
            reader.Close();
            resp.Close();
            string DecodedResponse = HttpUtility.HtmlDecode(response);

            JToken token = JToken.Parse(DecodedResponse);
            var list = token["response"].ToList();
            JToken photo = list[0];
            var sizes = photo["sizes"];
            string maxurl = "";
            int maxwidth = 0;
            foreach (var size in sizes.Children())
            {
                string url = (string)size["url"];
                int width=(int)size["width"];
                int height = (int)size["height"];
                if (width >= maxwidth)
                {
                    maxwidth = width;
                    maxurl = url;
                }
            }
            //get image from maxurl
            string path = Path.GetTempFileName();
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(maxurl), path);
                var lImage = Image.FromFile(path);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = lImage;
            }
            label1.Text = "Photo #"+(offset+1)+" of " + album.size;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //next photo from album
            int nextOffset = offsets[currAlbumId];
            nextOffset++;
            offsets[currAlbumId] = nextOffset;
            loadImage(albumlist.Where(a => a.id == currAlbumId).FirstOrDefault(), nextOffset);
        }
    }
}
