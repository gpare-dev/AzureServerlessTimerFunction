using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AzureTimerFunction
{
    public class TopGainersTopLosers
    {
        // If we want the function to run at 4pm GMT-5 each week day, we need to specify 21 (16+5) because it is set in UTC
        [FunctionName("TopGainersTopLosers")]
        public void Run([TimerTrigger("0 0 16 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            var message = new MailMessage();
            message.From = new MailAddress("asfdlkjh45lkfdblkjlkfdjhglkj56lk4j56fgkjh@hotmail.com");
            message.To.Add(new MailAddress("5147101381@vmobile.ca"));
            message.Body = GetMessage();

            var client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp-mail.outlook.com";
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("asfdlkjh45lkfdblkjlkfdjhglkj56lk4j56fgkjh@hotmail.com", "***");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(message);
        }

        private string GetMessage()
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine(DateTime.Today.ToString("ddd d MMM", CultureInfo.CreateSpecificCulture("en-US")));
            message.AppendLine("\nGainers:");
            AddTopsToMessage(message, 3, "https://www.tradingview.com/markets/stocks-usa/market-movers-gainers/");
            message.AppendLine("\nLosers:");
            AddTopsToMessage(message, 3, "https://www.tradingview.com/markets/stocks-usa/market-movers-losers/");

            return message.ToString();
        }

        private void AddTopsToMessage(StringBuilder message, int numberOfElementsToAdd, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            HtmlDocument doc = new HtmlDocument();
            doc.Load(sr);

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//tbody[1]");
            var trs = table.Descendants("tr");
            int count = 0;
            foreach (var tr in trs)
            {
                if (count == numberOfElementsToAdd)
                    break;

                var ticker = tr.Attributes[1].Value;
                var changePercent = tr.ChildNodes[4].InnerHtml;
                message.AppendLine($"{ticker} : {changePercent}");

                count++;
            }
        }
    }
}
