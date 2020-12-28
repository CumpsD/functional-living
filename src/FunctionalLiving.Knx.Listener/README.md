# FunctionalLiving.Knx.Listener

## Configuration Options

| Environment Variable                  | Default                               | Description   |
| :---                                  | :---:                                 | :---          |
| Features__EnableTracing               | `false`                               |               |
| Features__ConnectToKnx                | `false`                               |               |
| Features__DebugKnxCemi                | `false`                               |               |
| Features__SendToApi                   | `false`                               |               |
| Features__SendToLog                   | `false`                               |               |
| Features__UseKnxConnectionRouting     | `false`                               |               |
| Features__UseKnxConnectionTunneling   | `false`                               |               |
| Knx__RouterIp                         | `null`                                |               |
| Knx__RouterPort                       | `0`                                   |               |
| Knx__LocalIp                          | `null`        	                    |               |
| Knx__LocalPort                        | `0`                                   |               |
| Api__Endpoint                         | `""`                                  |               |
| Tracing__Host                         | `""`                                  |               |
| Tracing__Port                         | 0                                     |               |
| Tracing__ServiceName                  | `"FunctionalLiving.Knx.Listener"`     |               |
| Serilog__MinimumLevel__Default        | `"Information"`                       |               |
| Serilog__WriteTo__1__Name             | N/A                                   |               |
| Serilog__WriteTo__1__Args__ServerUrl  | N/A                                   |               |
| Serilog__Properties__Application      | `"FunctionalLiving - Knx Listener"`   |               |
