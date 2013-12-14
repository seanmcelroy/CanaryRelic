using System.Configuration;

namespace CanaryRelic
{
    public class NewRelicConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("accountId", IsRequired = true)]
        public int NewRelicAccountId
        {
            get { return (int)this["accountId"]; }
            set { this["accountId"] = value; }
        }
        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string NewRelicApiKey
        {
            get { return (string)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }
        [ConfigurationProperty("applicationId", IsRequired = true)]
        public int NewRelicApplicationId
        {
            get { return (int)this["applicationId"]; }
            set { this["applicationId"] = value; }
        }
        [ConfigurationProperty("metricName", IsRequired = true)]
        public string NewRelicMetricName
        {
            get { return (string)this["metricName"]; }
            set { this["metricName"] = value; }
        }
        [ConfigurationProperty("metricFieldName", IsRequired = true)]
        public string NewRelicMetricFieldName
        {
            get { return (string)this["metricFieldName"]; }
            set { this["metricFieldName"] = value; }
        }
    }
}
