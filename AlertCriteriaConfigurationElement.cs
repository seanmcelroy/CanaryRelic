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
        [ConfigurationProperty("pagerDuty", IsRequired = false)]
        public PagerDutyConfigurationElement PagerDuty
        {
            get { return (PagerDutyConfigurationElement)this["pagerDuty"]; }
            set { this["pagerDuty"] = value; }
        }

		/// <summary>
		/// Configuration for the minimum alerting threshold
		/// </summary>
		[ConfigurationProperty("minimumMetricAverage", IsRequired = false)]
		public float? MinimumMetricAverage
		{
			get { return this["minimumMetricAverage"] == null ? default(float?) : (float)this["minimumMetricAverage"]; }
			set { this["minimumMetricAverage"] = value; }
		}

        /// <summary>
        /// Configuration for the alerting threshold
        /// </summary>
		[ConfigurationProperty("maximumMetricAverage", IsRequired = false)]
        public float? MaximumMetricAverage
        {
            get { return this["maximumMetricAverage"] == null ? default(float?) : (float)this["maximumMetricAverage"]; }
            set { this["maximumMetricAverage"] = value; }
        }
        
    }
}
