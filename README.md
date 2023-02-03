# serilog-elasticsearch-kibana-net6
Logging into Elasticsearch using Serilog and viewing logs in Kibana | .NET Core

## Environment
| Software | Version |
|----------|:-------:|
| Docker | 20.10.22 |
| Elasticsearch | 8.6.1 |
| Kibana | 8.6.1 |

## Running Elasticsearch and Kibana with Docker
### Starting Containers ###
(Inside the **docker/** directory)

```sh
// Start
$ docker compose -f elasticsearch-kibana.yml up -d
```
```sh
// Stop
$ docker compose -f elasticsearch-kibana.yml stop
```
```sh
// Finish
$ docker compose -f elasticsearch-kibana.yml down
```

### Setting Minimal Security
(Inside the containers)
#### 1. Elasticsearch (es01):
```sh
// Reset the users passwords and write them down
$ ./bin/elasticsearch-reset-password -b -u elastic
$ ./bin/elasticsearch-reset-password -b -u kibana_system
``````

#### 2. Kibana (kib01):
```sh
// Create kibana-keystore
$ ./bin/kibana-keystore create

// Add the kibana_system user password to the keystore
$ ./bin/kibana-keystore add elasticsearch.password
``````
    
ps: Use here the password generated for `kibana_system` user in the previous step.

To see more informations: [Reference](https://www.elastic.co/guide/en/elasticsearch/reference/current/security-minimal-setup.html)

## Serilog Settings
Your appsettings.json normaly looks like that:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
You will overrride this information with:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System":  "Warning" 
      } 
    } 
  }
}
```
