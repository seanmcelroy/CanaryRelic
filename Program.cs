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

                        // Get all available metrics
                        //{
                        //    var urlAvailableMetrics = string.Format(
                        //       @"https://api.newrelic.com/api/v1/accounts/{0}/applications/{1}/metrics.xml",
                        //       alertingMetric.AccountID,
                        //       alertingMetric.ApplicationID);

                        //    string dataAvailableMetrics;
                        //    using (var request = new WebClient())
                        //    {
                        //        request.Headers.Add("x-api-key", alertingMetric.NewRelicAPIKey);
                        //        dataAvailableMetrics = request.DownloadString(urlAvailableMetrics);
                        //    }

                        //    Console.WriteLine(dataAvailableMetrics);
                        //}

                        var url = string.Format(
                            @"https://api.newrelic.com/api/v1/accounts/{0}/applications/{1}/data.xml?metrics[]={2}&field={3}&begin={4}&end={5}",
                            alertingMetric.AccountID,
                            alertingMetric.ApplicationID,
                            alertingMetric.MetricName,
                            alertingMetric.FieldName,
                            startDate,
                            endDate);

                        string data;
                        using (var request = new WebClient())
                        {
                            request.Headers.Add("x-api-key", alertingMetric.NewRelicAPIKey);
                            data = request.DownloadString(url);
                        }

                        var urlCount = string.Format(
                            @"https://api.newrelic.com/api/v1/accounts/{0}/applications/{1}/data.xml?metrics[]={2}&field={3}&begin={4}&end={5}",
                            alertingMetric.AccountID,
                            alertingMetric.ApplicationID,
                            alertingMetric.MetricName,
                            "call_count",
                            startDate,
                            endDate);

                        string dataCount;
                        using (var requestCount = new WebClient())
                        {
                            requestCount.Headers.Add("x-api-key", alertingMetric.NewRelicAPIKey);
                            dataCount = requestCount.DownloadString(urlCount);
                        }

                        var xml = new XmlDocument();
                        xml.LoadXml(data);
                        var nodes = xml.SelectNodes("metrics/metric/field");

                        var xmlCount = new XmlDocument();
                        xmlCount.LoadXml(dataCount);
                        var nodesCount = xmlCount.SelectNodes("metrics/metric/field");

                        if (nodes != null && nodesCount != null && nodes.Count == nodesCount.Count)
                        {
                            var sumMetric = 0F;
                            var countMetric = 0F;

                            for (var i = 0; i < nodes.Count; i++)
                            {
                                var node = nodes[i];
                                var nodeCount = nodesCount[i];

                                if (float.Parse(nodeCount.InnerText) > 1 || alertingMetric.FieldName == "average_value")
                                {
                                    sumMetric += float.Parse(node.InnerText);
                                    countMetric++;
                                }
                            }

                            if (countMetric < 3)
                                continue;

                            var avgMetric = sumMetric / countMetric;
                            if (float.IsNaN(avgMetric))
                                avgMetric = 0;

                            Console.WriteLine("{0:yyyy-MM-dd-HH:mm:ss} - {1}: {2:N3} average over {3} minutes", DateTime.Now, alertingMetric.PagerDutyMessage, avgMetric, countMetric);

                            if (avgMetric > alertingMetric.MaxAverage && (alertingMetric.LastPagerDutyAlert == null || (DateTime.Now - alertingMetric.LastPagerDutyAlert.Value).TotalMinutes > 60))
                            {
                                Console.WriteLine("Alerting PagerDuty for {0}!", alertingMetric.PagerDutyMessage);
                                PostPagerDutyAlert(alertingMetric.PagerDutyServiceAPIKey, alertingMetric.PagerDutyMessage, avgMetric);
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
