CanaryRelic
===========

CanaryRelic is a Microsoft .NET Console Application that polls the New Relic API
for one or more custom metrics you can configure and will dispatch an alert to
HipChat or PagerDuty when the 5-minute average of the given metric exceeds the
alerting threshold you specify.

I wrote this tool to fill in a gap in the New Relic alerting API, which does not
allow one to setup alerts for custom metrics you have may integrated for your
application to dispatch to New Relic.  While New Relic can show this information
on Dashboards in that product, it does not allow you to use custom metrics as
the basis for alerts to HipChat and PagerDuty.

And, thanks to Toby at New Relic for pointing out that the metric you are alerting
needn't be a custom metric, rather this will allow you to alert on any metric for
any application, like throughput. To list all metrics for an application, just use
the 'metrics.xml' call with no other parameters:

curl -H “x-api-key:YOUR API KEY” ‘https://api.newrelic.com/api/v1/agents/:agent_id/metrics.xml'

Where 'agent_id' can be any of your app ID #'s.


(The name 'canary' harkens back to the days when canaries were taken into mines
 as a barometer of dangerous gas build-up that could kill miners. :) )
