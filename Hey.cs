using System;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        string username = "";
        string password = "";
        string URI = "https://sms.schoolsoft.se/pps/jsp/Login.jsp";
        string myParameters = $"action=login&usertype=1&ssusername={username}&sspassword={password}&button=Logga+in";

        using (WebClient wc = new WebClient())
        {
            int i = 0;
	    bool run = true;
            while (run)
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.UploadString(URI, myParameters);
                Console.WriteLine(i++);
		if(i == 30) run = false;
            }
        }
    }
}