# FunctionalLiving.Api

## Configuration Options

| Environment Variable                  | Default                       | Description   |
| :---                                  | :---:                         | :---          |
| Features__EnableSignalR               | `false`                       |               |
| Features__EnableTracing               | `false`                       |               |
| Features__SendToInflux                | `false`                       |               |
| Features__SendToKnxSender             | `false`                       |               |
| Features__SendToLog                   | `false`                       |               |
| Features__SendToSignalR               | `false`                       |               |
| BaseUrl                               | `"http://localhost:9000"`     |               |
| BaseName                              | `"functional-living"`         |               |
| Cors__AllowedHosts__0                 | `"http://localhost:9000"`     |               |
| KnxSender__Endpoint                   | `""`                          |               |
| InfluxDb__Endpoint                    | `""`                          |               |
| InfluxDb__Token                       | `""`                          |               |
| InfluxDb__Bucket                      | `""`                          |               |
| InfluxDb__Organisation                | `""`                          |               |
| InfluxDb__LogLevel                    | `"None"`                      |               |
| Tracing__Host                         | `""`                          |               |
| Tracing__Port                         | 0                             |               |
| Tracing__ServiceName                  | `"FunctionalLiving.Api"`      |               |
| Serilog__MinimumLevel__Default        | `"Information"`               |               |
| Serilog__WriteTo__1__Name             | N/A                           |               |
| Serilog__WriteTo__1__Args__ServerUrl  | N/A                           |               |
| Serilog__Properties__Application      | `"FunctionalLiving - API"`    |               |
