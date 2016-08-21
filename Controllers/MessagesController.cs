using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace RoguBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private char[] vowels = new[] { 'а', 'у', 'е', 'ы', 'о', 'э', 'я', 'и', 'ю' };

        private char[] garbage = new[] { '!', ',', ' ', '+', ')', '=','?' };

        private Dictionary<char, char> dict = new Dictionary<char, char>();
 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var cleanText = activity.Text.TrimEnd(garbage);
                int length = (activity.Text ?? string.Empty).Length;
                var shortAnswer = "";
                for (int i = cleanText.Length - 1; i >= (int)(0.3 * length); i--)
                {
                    shortAnswer += cleanText[i];
                }
                shortAnswer = shortAnswer.Split(' ').First();
                var sa = shortAnswer.ToCharArray();
                Array.Reverse(sa);
                var resp = new string(sa);
                FillDictionary();
                var builder = new StringBuilder(resp);
                for (int i = 0; i <= builder.Length; i++)
                {
                    if (dict.ContainsKey(builder[i]))
                    {
                        builder[i] = dict[builder[i]];
                        break;
                    }
                }
                var answer = "Ху" + builder.ToString();
                
                Activity reply = activity.CreateReply(answer);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        private void FillDictionary()
        {
            dict.Add('а', 'я');
            dict.Add('у', 'ю');
            dict.Add('е', 'е');
            dict.Add('ы', 'ы');
            dict.Add('о', 'ё');
            dict.Add('я', 'я');
            dict.Add('и', 'и');
            dict.Add('ю', 'ю');
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