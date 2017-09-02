
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.ProjectOxford.Face;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;


namespace Single2Double
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        // string urid;
        string faceid;
        float Confidence_s;
        float Confidence_ns;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>


        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //連接資料庫
            var cb = new SqlConnectionStringBuilder();
            cb.DataSource = "s2dchat.database.windows.net";
            cb.UserID = "yezifa2005";
            cb.Password = "E.860527e";
            cb.InitialCatalog = "S2D";



            Activity reply = activity.CreateReply();
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

                if (activity.Attachments?.Count > 0 && activity.Attachments.First().ContentType.StartsWith("image"))
                {
                    //User傳送一張照片
                    ImageTemplate(reply, activity.Attachments.First().ContentUrl);

                }
                else if (activity.Text.ToString() == "請上傳一張照片")
                {
                    reply.Text = "請上傳一張照片";
                }
                else if (activity.Text.ToString() == "測試異性對我的喜好")
                {
                    await InvokeRequestResponseService(reply);
                }
                else if (activity.Text == "我好飢渴")
                {
                    string url = "";

                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {   //隨機選擇照片
                            Random rnd = new Random();
                            //目前資料庫的照片張數
                            int rd = rnd.Next(1, 10);
                            //建立資料庫連線
                            connection.Open();
                            //撰寫query
                            StringBuilder sb = new StringBuilder();
                            sb.Append("SELECT [url] ");
                            sb.Append("FROM [dbo].[save_url]");
                            sb.Append("Where [id]=" + rd);
                            String sql = sb.ToString();
                            using (var cmd = new SqlCommand(sql, connection))
                            {
                                url = (string)cmd.ExecuteScalar();
                            }


                        }
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }



                    Attachment att = new Attachment();
                    att.ContentType = "image/png";
                    att.ContentUrl = url;
                    reply.Text = "請慢慢享用^^";

                    reply.Attachments.Add(att);

                }
                else
                {
                    if (activity.ChannelId == "facebook")
                    {
                        var fbData = JsonConvert.DeserializeObject<FBChannelModel>(activity.ChannelData.ToString());
                        Rootobject ttt = new Rootobject();

                        // ttt.faceId = faceid;
                        ttt.personGroupId = "loser";
                        ttt.personId = "6449c1a2-988b-46f7-b07a-da037c175f29";
                        Rootobject winer = new Rootobject();
                        winer.personGroupId = "winer";
                        winer.personId = "383b0c3a-cfc7-4e12-a992-0d7328cee0a0";
                        if (fbData.postback != null && fbData.postback.payload.StartsWith("Analyze"))
                        {
                            //辨識圖片
                            var url = fbData.postback.payload.Split('>')[1];
                            //reply.Text = $"{url}";
                            FaceServiceClient client = new FaceServiceClient("df30d486a01b4ee9bbf913a324795d62", "https://southeastasia.api.cognitive.microsoft.com/face/v1.0");
                            var faces = await client.DetectAsync(
                                url,
                                true,
                                false
                                );

                            foreach (var face in faces)
                            {
                                var id = face.FaceId;

                                //reply.Text = "HI";
                                //reply.Text = $"{id}";
                                ttt.faceId = id.ToString();
                                winer.faceId = id.ToString();
                                string body = JsonConvert.SerializeObject(ttt).ToString();
                                string body2 = JsonConvert.SerializeObject(winer).ToString();
                                MakeRequest(body, activity, "單身", Confidence_s);
                                MakeRequest(body2, activity, "不是單身", Confidence_ns);
                                // await connector.Conversations.ReplyToActivityAsync(reply);
                                Confidence_s = Confidence_s - Confidence_ns;
                                if (Confidence_s > 0)
                                {
                                    reply.Text = "這個人應該是單身";
                                    await connector.Conversations.ReplyToActivityAsync(reply);
                                }
                                if (Confidence_s < 0)
                                {

                                    reply.Text = "這個人應該不是是單身";
                                    await connector.Conversations.ReplyToActivityAsync(reply);
                                }

                            }
                            // Console.WriteLine("Hit ENTER to exit...");
                            // Console.ReadLine();

                        }

                    }


                }

                await connector.Conversations.ReplyToActivityAsync(reply);

            }

            else
            {
                // HandleSystemMessage(activity);

            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        //verify
        static async void MakeRequest(string body, Activity activity, string status, float Confidence)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            // Activity activity=new Activity();
            Activity datareply = activity.CreateReply();
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "df30d486a01b4ee9bbf913a324795d62");

            var uri = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/verify?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            //response = await client.PostAsync(uri, new StringContent(body, Encoding.UTF8, "application/json"));
            //datareply.Text = await response.Content.ReadAsStringAsync();
            //await connector.Conversations.ReplyToActivityAsync(datareply);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                datareply.Text = await response.Content.ReadAsStringAsync();
                JObject rss = JObject.Parse(datareply.Text);
                Confidence = (float)rss["confidence"];

                datareply.Text = $"他跟{status}的人的長相有 {Confidence} 的相似度";
                await connector.Conversations.ReplyToActivityAsync(datareply);
            }

        }

        private void ImageTemplate(Activity reply, string url)

        {

            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {

                Title = "Cognitive services",

                Subtitle = "Select from below",

                Images = new List<CardImage>() { new CardImage(url) },

                Buttons = new List<CardAction>()

                {

                    //new CardAction(ActionTypes.PostBack, "男女生", value: $"Face>{url}"),

                    new CardAction(ActionTypes.PostBack, "確認感情狀態", value: $"Analyze>{url}")

                }

            }.ToAttachment());



            reply.Attachments = att;

        }
        private void CreateButton(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "主人早安~我口以腫摩幫你呢?<3",
                Buttons = new List<CardAction>()

                {

                    new CardAction(ActionTypes.PostBack, "我好飢渴", value: "Hunger"),
                    new CardAction(ActionTypes.PostBack, "我只是想打招呼", value: "Hi")

                }

            }.ToAttachment());



            reply.Attachments = att;

        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
        private static async Task InvokeRequestResponseService(Activity reply)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"gender", "dec_o", "age", "go_out", "sports", "tvsports", "exercise", "dining", "museums", "art", "hiking", "gaming", "clubbing", "reading", "tv", "theater", "movies", "concerts", "music", "shopping", "yoga", "attr1_1", "sinc1_1", "intel1_1", "fun1_1", "amb1_1", "shar1_1"},
                                Values = new string[,] {  { "Female", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  { "Female", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "czD5H2ubumwhorFcBHRaTCBmAniN65q6llAAxYJA2ERXT/Z9rEW1wfQy5hp2dLOh+KBG2RRq4LPeEYpIZ0Cang=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/8438100c4a1042d8a0d4fc3e67721cbe/services/44a57a39f0774185b77b65d8f2c00998/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string Scored;
                    string result = await response.Content.ReadAsStringAsync();
                      
                    var res = JsonConvert.DeserializeObject<resultobject>(result);
                    Scored = res.Results.output1.value.Values[0][27];
                    reply.Text = Scored;
                    

                    
                    
                }
                else
                {
                    reply.Text = "幹你娘";
                    /*Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);*/
                }
            }
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

    }

}
