using System.Configuration;

namespace CanaryRelic
{
    public class AlertCriteriaConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// A name for the alert criteria
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

		/// <summary>
		/// Configuration information about NewRelic
		/// </summary>
		[ConfigurationProperty("newRelic", IsRequired = true)]
		public NewRelicConfigurationElement NewRelic
		{
			get { return (NewRelicConfigurationElement)this["newRelic"]; }
			set { this["newRelic"] = value; }
		}

		/// <summary>
		/// Gets or sets the configuration information for HipChat
		/// </summary>
		[ConfigurationProperty("hipChat", IsRequired = false)]
		public HipChatConfigurationElement HipChat
		{
			get { return (HipChatConfigurationElement)this["hipChat"]; }
			set { this["hipChat"] = value; }
		}

        /// <summary>
        /// Configuration information for PagerDuty
        /// </summary>
        [ConfigurationProperty("pagerDuty", IsRequired = true)]
        public PagerDutyConfigurationElement PagerDuty
        {
            get { return (PagerDutyConfigurationElement)this["pagerDuty"]; }
            set { this["pagerDuty"] = value; }
        }
        
        /// <summary>
        /// Configuration for the alerting threshold
        /// </summary>
        [ConfigurationProperty("maximumMetricAverage", IsRequired = true)]
        public float MaximumMetricAverage
        {
            get { return (float)this["maximumMetricAverage"]; }
            set { this["maximumMetricAverage"] = value; }
        }
        
    }
}
