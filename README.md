# InfluxClient
A .NET InfluxDB client

[![Build status](https://ci.appveyor.com/api/projects/status/kab7aiacy0vjv1sr?svg=true)](https://ci.appveyor.com/project/danesparza/influxclient)

### Quick Start
In your application, call:

```CSharp
// Instanciate the InfluxManager, passing your endpoint and target database:
InfluxManager mgr = new InfluxManager("http://YOURSERVER:8086/", "YOUR_DATABASE");

// Create a measurement (with at least one value)
Measurement m = new Measurement()
{
  Name = "some_metric",
  IntegerFields = new List<IntegerField>()
  {
      new IntegerField() { Name="count", Value=44 }
  }
};

// Write the measurement (notice that this is awaitable):
var retval = await mgr.Write(m);
```
