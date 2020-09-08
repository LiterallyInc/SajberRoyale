using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class Webhook
{

    public static void Send(string msg, string name = "Fabina")
    {
        using (dWebHook dcWeb = new dWebHook())
        {
            dcWeb.displayname = name;
            dcWeb.SendMessage(msg);
        }
    }
    public class dWebHook : IDisposable
    {
        readonly string publichook = "https://discordapp.com/api/webhooks/750810981705449533/Rh7S39h_O5_nlaCa2skBm_LPSd0SyZtcnS4_4TVqlgtreFFDnfGTX0OPm6V3kcoUMiul";
        readonly string privatehook = "https://discordapp.com/api/webhooks/721675223841374228/fzRfJkuLvyrmN0caW3y5vU0_lVI-yeXWZ7Td8eBL2Yjm4n9s5l04mp0mbZ6CDWbxMpAI";
        readonly string avatar = "http://sajber.me/account/Email/webhookpfp.png";
        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        public string displayname { get; set; }
        public string profilepic { get; set; }

        public dWebHook()
        {
            dWebClient = new WebClient();
        }


        public void SendMessage(string msgSend)
        {
            discordValues.Set("username", displayname);
            discordValues.Set("avatar_url", avatar);
            discordValues.Set("content", msgSend);

            try
            {
                dWebClient.UploadValues(privatehook, discordValues);
            }
            catch (WebException e)
            {
                UnityEngine.Debug.LogError($"Webhook: Tried to send message with text {msgSend} failed, error message:\n\n{e}");
            }
            discordValues = new NameValueCollection();
        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }
    }
}