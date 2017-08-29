

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

namespace Single2Double
{
    
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
                else {
                    if (activity.ChannelId == "facebook")
                    {
                        var fbData = JsonConvert.DeserializeObject<FBChannelModel>(activity.ChannelData.ToString());
                        Rootobject ttt=new Rootobject();
                       
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
                                string body2= JsonConvert.SerializeObject(winer).ToString();
                                MakeRequest(body, activity, "單身",Confidence_s);
                                MakeRequest(body2,activity, "不是單身", Confidence_ns);
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
    }
}