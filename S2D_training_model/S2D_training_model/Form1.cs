using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.IO;

namespace S2D_training_model
{
    public partial class Form1 : Form
    {
        //public static string createPuri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/persongroups/loser/persons";
        OpenFileDialog openFileDialog1;

        

        public Form1()
        {
            InitializeComponent();
        }
        private async void UploadPicture(string [] uri,string AddPersonFace, string TrainingGroup)
        {
            

            //txtPicturePath.Text = "";
            //pic.Image = null;
            //txtGuid.Text = "";

            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "df30d486a01b4ee9bbf913a324795d62");
            HttpResponseMessage response;
            foreach(string line in uri)
            {
                byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + line + "\"}");


                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(AddPersonFace, content);
                }

            }
            byte[] byteDataTrain = Encoding.UTF8.GetBytes("");

            using (var content = new ByteArrayContent(byteDataTrain))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(TrainingGroup, content);
            }
            MessageBox.Show(response.ToString());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt Files|*.txt";
            openFileDialog1.Title = "Please select the loser list";
            string addPersonFaceloser = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/loser/persons/6449c1a2-988b-46f7-b07a-da037c175f29/persistedFaces";
            string TrainingGroup = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/loser/train";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                UploadPicture(lines, addPersonFaceloser, TrainingGroup);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt Files|*.txt";
            openFileDialog1.Title = "Please select the winner list";
            string addPersonFacewinner = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/winer/persons/383b0c3a-cfc7-4e12-a992-0d7328cee0a0/persistedFaces";
            string TrainingGroup = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/persongroups/winer/train";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                UploadPicture(lines, addPersonFacewinner, TrainingGroup);
            }
        }
    }
}
