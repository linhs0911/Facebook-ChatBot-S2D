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
        private string faceid;

        private float Confidence_s;
        private float Confidence_ns;

        public string[,] inputValues = new string[2, 11];

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
                    CreateButtonOne(reply);
                    //await InvokeRequestResponseService(reply, inputValues);
                }
                else if (activity.Text.ToString() == "Female>1" || activity.Text.ToString() == "Male>1")
                {
                    // answer
                    inputValues[0, 0] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 0] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',1,'" + inputValues[0, 0] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                    //show botton
                    CreateButtonTwo(reply);
                }
                //上傳年紀
                else if (activity.Text.ToString() == "1>2" || activity.Text.ToString() == "2>2" || activity.Text.ToString() == "3>2" || activity.Text.ToString() == "4>2" || activity.Text.ToString() == "5>2" || activity.Text.ToString() == "6>2" || activity.Text.ToString() == "7>2")
                {
                    inputValues[0, 2] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 2] = activity.Text.ToString().Split('>')[0];
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);
                    reply.Text = "請問你的年紀(0~200)?";
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',2,'" + inputValues[0, 2] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        
                        
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }
               
                else if (Int32.Parse("activity.Text")>0)
                {
                    reply.Text = Int32.Parse("activity.Text").ToString();
                    // answer
                    inputValues[0, 3] = activity.Text.ToString();
                    inputValues[1, 3] = activity.Text.ToString();
                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',3,'" + inputValues[0, 3] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonThree(reply);
                }
                else if (activity.Text.ToString() == "1>3" || activity.Text.ToString() == "2>3" || activity.Text.ToString() == "3>3" || activity.Text.ToString() == "4>3" || activity.Text.ToString() == "5>3" || activity.Text.ToString() == "6>3" || activity.Text.ToString() == "7>3" || activity.Text.ToString() == "8>3" || activity.Text.ToString() == "9>3" || activity.Text.ToString() == "10>3")
                {
                    // answer
                    inputValues[0, 4] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 4] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',4,'" + inputValues[0, 4] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonFour(reply);
                }
                else if (activity.Text.ToString() == "1>4" || activity.Text.ToString() == "2>4" || activity.Text.ToString() == "3>4" || activity.Text.ToString() == "4>4" || activity.Text.ToString() == "5>4" || activity.Text.ToString() == "6>4" || activity.Text.ToString() == "7>4" || activity.Text.ToString() == "8>4" || activity.Text.ToString() == "9>4" || activity.Text.ToString() == "10>4")
                {
                    // answer
                    inputValues[0, 5] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 5] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',5,'" + inputValues[0, 5] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonFive(reply);
                }
                else if (activity.Text.ToString() == "1>5" || activity.Text.ToString() == "2>5" || activity.Text.ToString() == "3>5" || activity.Text.ToString() == "4>5" || activity.Text.ToString() == "5>5" || activity.Text.ToString() == "6>5" || activity.Text.ToString() == "7>5" || activity.Text.ToString() == "8>5" || activity.Text.ToString() == "9>5" || activity.Text.ToString() == "10>5")
                {
                    // answer
                    inputValues[0, 6] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 6] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',6,'" + inputValues[0, 6] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonSix(reply);
                }
                else if (activity.Text.ToString() == "1>6" || activity.Text.ToString() == "2>6" || activity.Text.ToString() == "3>6" || activity.Text.ToString() == "4>6" || activity.Text.ToString() == "5>6" || activity.Text.ToString() == "6>6" || activity.Text.ToString() == "7>6" || activity.Text.ToString() == "8>6" || activity.Text.ToString() == "9>6" || activity.Text.ToString() == "10>6")
                {
                    // answer
                    inputValues[0, 7] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 7] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',7,'" + inputValues[0, 7] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonSeven(reply);
                }
                
                else if (activity.Text.ToString() == "1>7" || activity.Text.ToString() == "2>7" || activity.Text.ToString() == "3>7" || activity.Text.ToString() == "4>7" || activity.Text.ToString() == "5>7" || activity.Text.ToString() == "6>7" || activity.Text.ToString() == "7>7" || activity.Text.ToString() == "8>7" || activity.Text.ToString() == "9>7" || activity.Text.ToString() == "10>7")
                {
                    // answer
                    inputValues[0, 8] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 8] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',8,'" + inputValues[0, 8] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonEight(reply);
                }
                else if (activity.Text.ToString() == "1>8" || activity.Text.ToString() == "2>8" || activity.Text.ToString() == "3>8" || activity.Text.ToString() == "4>8" || activity.Text.ToString() == "5>8" || activity.Text.ToString() == "6>8" || activity.Text.ToString() == "7>8" || activity.Text.ToString() == "8>8" || activity.Text.ToString() == "9>8" || activity.Text.ToString() == "10>8")
                {
                    // answer
                    inputValues[0, 9] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 9] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',9,'" + inputValues[0, 9] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    // show botton
                    CreateButtonNine(reply);
                }
                else if (activity.Text.ToString() == "1>9" || activity.Text.ToString() == "2>9" || activity.Text.ToString() == "3>9" || activity.Text.ToString() == "4>9" || activity.Text.ToString() == "5>9" || activity.Text.ToString() == "6>9" || activity.Text.ToString() == "7>9" || activity.Text.ToString() == "8>9" || activity.Text.ToString() == "9>9" || activity.Text.ToString() == "10>9")
                {
                    // answer
                    inputValues[0, 10] = activity.Text.ToString().Split('>')[0];
                    inputValues[1, 10] = activity.Text.ToString().Split('>')[0];

                    // user ID
                    string ChanData = activity.ChannelData.ToString();
                    string ChanData2 = ChanData.Remove(0, 29);
                    string ChanData3 = ChanData2.Remove(16, ChanData2.Length - 16);

                    // insert value into database
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            string sql = "INSERT INTO [dbo].[Message] ([id],[question],[message]) VALUES ('" + ChanData3 + "',10,'" + inputValues[0, 10] + "')";
                            //撰寫query
                            SqlCommand insertValue = new SqlCommand(sql, connection);
                            insertValue.ExecuteNonQuery();
                        }

                        //reply.Text = inputValues[0, 0];
                    }
                    catch (SqlException e)
                    {
                        reply.Text = "沒成功";
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                   

                    //reply.Text = "55555";
                    /*
                    try
                    {
                        using (var connection = new SqlConnection(cb.ConnectionString))
                        {
                            //建立資料庫連線
                            connection.Open();
                            //撰寫query
                            StringBuilder sb = new StringBuilder();
                            sb.Append("SELECT [answer] ");
                            sb.Append("FROM [dbo].[Message]");
                            sb.Append("Where [id]=" + );
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
                    */
                    //await InvokeRequestResponseService(reply, inputValues);
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
                            int rd = rnd.Next(1, 15);
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
        private static async void MakeRequest(string body, Activity activity, string status, float Confidence)
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

        private void CreateButtonOne(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "What is your gender?",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "Male", value: "Male>1"),
                    new CardAction(ActionTypes.PostBack, "Female", value: "Female>1"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonTwo(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How often do you go out?",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "Many times / week", value: "1>2"),
                    new CardAction(ActionTypes.PostBack, "Twice / week", value: "2>2"),
                    new CardAction(ActionTypes.PostBack, "Once / week", value: "3>2"),
                    new CardAction(ActionTypes.PostBack, "Twice / month", value: "4>2"),
                    new CardAction(ActionTypes.PostBack, "Once / month", value: "5>2"),
                    new CardAction(ActionTypes.PostBack, "Many times / year", value: "6>2"),
                    new CardAction(ActionTypes.PostBack, "Almost never", value: "7>2"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonThree(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Playing Sports?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>3"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>3"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>3"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>3"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>3"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>3"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>3"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>3"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>3"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>3"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonFour(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Watching Sports?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>4"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>4"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>4"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>4"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>4"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>4"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>4"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>4"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>4"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>4"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonFive(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Hiking/Camping?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>5"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>5"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>5"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>5"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>5"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>5"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>5"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>5"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>5"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>5"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonSix(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Gaming?)",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>6"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>6"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>6"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>6"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>6"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>6"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>6"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>6"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>6"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>6"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonSeven(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Watching TV?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>7"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>7"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>7"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>7"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>7"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>7"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>7"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>7"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>7"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>7"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonEight(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Shopping?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>8"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>8"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>8"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>8"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>8"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>8"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>8"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>8"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>8"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>8"),
                }
            }.ToAttachment());

            reply.Attachments = att;
        }

        private void CreateButtonNine(Activity reply)
        {
            List<Attachment> att = new List<Attachment>();

            att.Add(new HeroCard()

            {
                Title = "How interested are you in Yoga/Meditation?",
                Subtitle = "(1=not at all, 10 = highly)",
                Buttons = new List<CardAction>()

                {
                    new CardAction(ActionTypes.PostBack, "1", value: "1>9"),
                    new CardAction(ActionTypes.PostBack, "2", value: "2>9"),
                    new CardAction(ActionTypes.PostBack, "3", value: "3>9"),
                    new CardAction(ActionTypes.PostBack, "4", value: "4>9"),
                    new CardAction(ActionTypes.PostBack, "5", value: "5>9"),
                    new CardAction(ActionTypes.PostBack, "6", value: "6>9"),
                    new CardAction(ActionTypes.PostBack, "7", value: "7>9"),
                    new CardAction(ActionTypes.PostBack, "8", value: "8>9"),
                    new CardAction(ActionTypes.PostBack, "9", value: "9>9"),
                    new CardAction(ActionTypes.PostBack, "10", value: "10>9"),
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

        private static async Task InvokeRequestResponseService(Activity reply, string[,] value)
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
                                ColumnNames = new string[] {"gender", "dec_o", "age", "go_out", "sports", "tvsports", "hiking", "gaming", "tv", "shopping", "yoga"},
                                Values = new string[,] { { value[0, 0], value[0, 1], value[0, 2], value[0, 3], value[0, 4], value[0, 5], value[0, 6], value[0, 7], value[0, 8], value[0, 9], value[0, 10] },
                                                                    { value[1, 0], value[1, 1], value[1, 2], value[1, 3], value[1, 4], value[1, 5], value[1, 6], value[1, 7], value[1, 8], value[1, 9], value[1, 10] }, }
                             }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "8SM/8J2nOEAHLQ701i4uaR/XMa7RNIzfddElQ8ACBhVhnT6BU0LWf2NY2hDVUe/NQGsNxLNHhSfPQdsQrUzuhw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/8438100c4a1042d8a0d4fc3e67721cbe/services/adc3ea3d48f849b4b2d3592286bd05f7/execute?api-version=2.0&details=true");

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
                    Scored = res.Results.output1.value.Values[0][11];
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