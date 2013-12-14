using System.Configuration;

namespace CanaryRelic
{
    /// <summary>
    /// Configuration information for PagerDuty
    /// </summary>
    public class PagerDutyConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// The API key of the PagerDuty 'generic service' to dispatch alerts to
        /// </summary>
        [ConfigurationProperty("genericServiceAPIKey", IsRequired = true)]
        public string GenericServiceApiKey
        {
            get { return (string)this["genericServiceAPIKey"]; }
            set { this["genericServiceAPIKey"] = value; }
        }
        /// <summary>
        /// The message to send to PagerDuty on alert
        /// </summary>
        [ConfigurationProperty("messageOnAlert", IsRequired = true)]
        public string MessageOnAlert
        {
            get { return (string)this["messageOnAlert"]; }
            set { this["messageOnAlert"] = value; }
        }
    }
}
