namespace CanaryRelic
{
	using System;
	
	public class AlertingMetric
    {
        public string NewRelicAPIKey { get; set; }
        public int AccountID { get; set; }
        public int ApplicationID { get; set; }
        public string MetricName { get; set; }
        public string FieldName { get; set; }
        public float MaxAverage { get; set; }
		public string PagerDutyServiceAPIKey { get; set; }
		public string PagerDutyMessage { get; set; }
		public DateTime? LastPagerDutyAlert { get; set; }

		public string HipChatApiKey { get; set; }
	    public string HipChatRoomName { get; set; }
	}
}
