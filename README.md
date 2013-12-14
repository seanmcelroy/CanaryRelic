CanaryRelic
===========

CanaryRelic is a Microsoft .NET Console Application that polls the New Relic API
for one or more custom metrics you can configure and will dispatch an alert to
PagerDuty when the 5-minute average of the given metric exceeds the alerting
threshold you specify.

I wrote this tool to fill in a gap in the New Relic alerting API, which does not
allow one to setup alerts for custom metrics you have may integrated for your
application to dispatch to New Relic.  While New Relic can show this information
on Dashboards in that product, it does not allow you to use custom metrics as
the basis for alerts to PagerDuty.

(The name 'canary' harkens back to the days when canaries were taken into mines
 as a barometer of dangerous gas build-up that could kill miners. :) )
 
Released into the public domain by Sean McElroy ( me @ seanmcelroy [dot) .com )
