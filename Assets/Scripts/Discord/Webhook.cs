using Photon.Pun;
using SajberRoyale.Game;
using System;
using System.Collections.Specialized;
using System.Net;

internal class Webhook
{
    public static void Log(string command)
    {
        if (!Game.Instance.IsTournament) return;
        using (dWebHook dcWeb = new dWebHook())
        {
            dcWeb.displayname = "Kami";
            dcWeb.profilepic = "https://i.imgur.com/jDbymde.png";
            dcWeb.SendMessage($"<:fish:807746886148685834> **Developer command used in tournament.**\n\n**Player:** {PhotonNetwork.NickName}\n**Room:** {PhotonNetwork.CurrentRoom.Name}\n**Command:** {command}\n");
        }
    }

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
        private readonly string publichook = "https://discordapp.com/api/webhooks/750810981705449533/Rh7S39h_O5_nlaCa2skBm_LPSd0SyZtcnS4_4TVqlgtreFFDnfGTX0OPm6V3kcoUMiul";
        private readonly string privatehook = "https://discordapp.com/api/webhooks/721675223841374228/fzRfJkuLvyrmN0caW3y5vU0_lVI-yeXWZ7Td8eBL2Yjm4n9s5l04mp0mbZ6CDWbxMpAI";
        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        public string displayname { get; set; }
        public string profilepic { get; set; } = "http://sajber.me/account/Email/webhookpfp.png";

        public dWebHook()
        {
            dWebClient = new WebClient();
        }

        public void SendMessage(string msgSend)
        {
            discordValues.Set("username", displayname);
            discordValues.Set("avatar_url", profilepic);
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