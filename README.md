# InfluxClient
A .NET InfluxDB client that supports the [v0.9 API](https://influxdb.com/docs/v0.9/introduction/overview.html), the [line protocol](https://influxdb.com/docs/v0.9/write_protocols/line.html) for efficient logging, and InfluxDB [authentication](https://influxdb.com/docs/v0.9/administration/authentication_and_authorization.html)

[![Build status](https://ci.appveyor.com/api/projects/status/kab7aiacy0vjv1sr?svg=true)](https://ci.appveyor.com/project/danesparza/influxclient)

### Quick Start
In your application, call:

```CSharp
// Create the InfluxManager, passing the InfluxDB endpoint and target database:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE");

// Create a measurement (with at least one value)
Measurement m = new Measurement("unittest").AddField("count", 42);

// Write the measurement (notice that this is awaitable):
var retval = await mgr.Write(m);
```

#### Using authentication
Using authentication is as simple as passing in your username and password as part of the InfluxManager constructor:

```CSharp
// To authenticate, create the InfluxManager with additional parameters:
InfluxManager mgr = new InfluxManager(influxEndpoint, influxDatabase, influxUser, influxPassword);
```
