using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace CanaryRelic
{
    public class Program
    {
        public static void Main()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var canaryRelicConfigurationSection = (CanaryConfigurationSection)config.GetSection("canaryRelic");

            var alertingMetrics = canaryRelicConfigurationSection.Alerts.Select(x => new AlertingMetric
            {
                AccountID = x.NewRelic.NewRelicAccountId,
                ApplicationID = x.NewRelic.NewRelicApplicationId,
                FieldName = x.NewRelic.NewRelicMetricFieldName,
                LastPagerDutyAlert = null,
                MaxAverage = x.MaximumMetricAverage,
                MetricName = x.NewRelic.NewRelicMetricName,
                NewRelicAPIKey = x.NewRelic.NewRelicApiKey,
                PagerDutyServiceAPIKey = x.PagerDuty.GenericServiceApiKey,
                PagerDutyMessage = x.PagerDuty.MessageOnAlert
            })
            .ToList();

            while (true)
            {
                try
                {
                    foreach (var alertingMetric in alertingMetrics)
                    {
                        var startDate = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
                        var endDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";

                        var url = string.Format(
                            @"https://api.newrelic.com/api/v1/accounts/{0}/applications/{1}/data.xml?metrics[]={2}&field={3}&begin={4}&end={5}",
                            alertingMetric.AccountID,
                            alertingMetric.ApplicationID,
                            alertingMetric.MetricName,
                            alertingMetric.FieldName,
                            startDate,
                            endDate);

                        var request = new WebClient();
                        request.Headers.Add("x-api-key", alertingMetric.NewRelicAPIKey);
                        var data = request.DownloadString(url);

                        var xml = new XmlDocument();
                        xml.LoadXml(data);
                        var nodes = xml.SelectNodes("metrics/metric/field");
                        if (nodes != null)
                        {
                            var sum = 0F;
                            var count = 0F;
                            foreach (XmlNode node in nodes)
                            {
                                sum += float.Parse(node.InnerText);
                                count++;
                            }
                            var avg = sum / count;

                            Console.WriteLine("{0:yyyy-MM-dd-HH:mm:ss} - {1}: {2} average over {3} minutes", DateTime.Now, alertingMetric.PagerDutyMessage, avg, count);

                            if (avg > alertingMetric.MaxAverage && (alertingMetric.LastPagerDutyAlert == null || (DateTime.Now - alertingMetric.LastPagerDutyAlert.Value).TotalMinutes > 60))
                            {
                                Console.WriteLine("Alerting PagerDuty for {0}!", alertingMetric.PagerDutyMessage);
                                PostPagerDutyAlert(alertingMetric.PagerDutyServiceAPIKey, alertingMetric.PagerDutyMessage, avg);
                                alertingMetric.LastPagerDutyAlert = DateTime.Now;
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(29000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }            
        }

        private static void PostPagerDutyAlert(string pagerDutyServiceKey, string description, float cur)
        {
            Console.WriteLine("[{0} {1}] Posting alert to PagerDuty for {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), description);

            var json = new
            {
                service_key = pagerDutyServiceKey,
                event_type = "trigger",
                description,
                details = new
                {
                    current_average = cur
                }
            };

            var http = (HttpWebRequest)WebRequest.Create(new Uri("https://events.pagerduty.com/generic/2010-04-15/create_event.json"));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            var parsedContent = JsonConvert.SerializeObject(json);
            var encoding = new ASCIIEncoding();
            var bytes = encoding.GetBytes(parsedContent);

            var newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var content = sr.ReadToEnd();
                        Console.WriteLine("Response from PagerDuty: {0}", content);
                    }
                }
            }
        }

    }
}
