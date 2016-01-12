# InfluxClient
A .NET [InfluxDB](https://influxdb.com/) client that supports asynchronous IO, the [v0.9 API](https://influxdb.com/docs/v0.9/introduction/overview.html), the [line protocol](https://influxdb.com/docs/v0.9/write_protocols/line.html) for efficient logging, and InfluxDB [authentication](https://influxdb.com/docs/v0.9/administration/authentication_and_authorization.html)

[![Build status](https://ci.appveyor.com/api/projects/status/kab7aiacy0vjv1sr?svg=true)](https://ci.appveyor.com/project/danesparza/influxclient)

### Quick Start
Install the [NuGet package](https://www.nuget.org/packages/InfluxClient/) from the package manager console:

```powershell
Install-Package InfluxClient
```

Be sure your project has a reference to `System.Net.Http` (for the [HttpResponseMessage](https://msdn.microsoft.com/en-us/library/system.net.http.httpresponsemessage(v=vs.118).aspx) class)

#### Logging a measurement
In your application, call:

```CSharp
// Create the InfluxManager, passing the InfluxDB endpoint and target database:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE");

// Create a measurement (with a name and at least one field name and value)
Measurement m = new Measurement("unittest").AddField("count", 42);

// Write the measurement (notice that this is awaitable):
var retval = await mgr.Write(m);
```

#### Exceptions
Logging and telemetry is usually a secondary function in an application -- so by default, InfluxClient tries to be as quiet as possible when handling error conditions.  InfluxClient won't throw exceptions for anything unless you indicate it's OK to do so. 

You can control this behavior in the constructor:

```CSharp
// Create the influx client and indicate we want to have exceptions bubble up:
InfluxManager mgr = new InfluxManager(_influxEndpoint, _influxDatabase, true);
// Just add a parameter to your constructor                             ^^^^
```

The client will always try to signal with `Trace` output when something goes wrong -- so you should be able to trap this in your application logging toolkit (or see it in your debugging output) without too much effort -- even if you have exceptions turned off.

#### Reading measurements
Based on your provided [InfuxQL query](https://influxdb.com/docs/v0.9/query_language/data_exploration.html), InfluxDB passes data back in JSON format.  You can either get the raw string back or use the helper methods to get a native object back.  

To get the JSON back:

```CSharp
// Create the InfluxManager, passing the InfluxDB endpoint and target database:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE");

// Pass in your InfluxQL query (notice that this is awaitable)
HttpResponseMessage retval = await mgr.QueryJSON("select * from unittest");

// Get the raw JSON data passed back:
data = await retval.Content.ReadAsStringAsync();
```

To get a [QueryResponse](https://github.com/danesparza/InfluxClient/blob/master/InfluxClient/QueryResponse.cs) object back:

```CSharp
// Create the InfluxManager, passing the InfluxDB endpoint and target database:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE");

// Pass in your InfluxQL query (notice that this is awaitable)
var retval = await mgr.Query("select * from unittest");

// You now have your data back:
string seriesTitle = retval.Results[0].Series[0].Name;
```

#### Using authentication
Using authentication is as simple as passing in your username and password as part of the InfluxManager constructor:

```CSharp
// To authenticate, create the InfluxManager with additional parameters:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE", "user", "password");

// Continue normally ... create a measurement (with a name and at least one field name and value)
Measurement m = new Measurement("unittest").AddField("count", 42);

// Write the measurement (notice that this is awaitable):
var retval = await mgr.Write(m);
```
