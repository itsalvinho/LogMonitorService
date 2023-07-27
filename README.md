# LogMonitorService

This is a .NET 6 project used to read log files.  It serves as an API for users to execute against to retrieve and optionally search logs from a specified file on the server it is hosted on.

**Table of Contents**

[TOCM]

[TOC]

# Deployment

The simplest way to run this project is to use Docker, otherwise you would need dotnet runtimes (.NET 6) installed on your machine, build, publish, and run it using IIS, nginx, or some any sort of server, which can be complicated to figure out. Simplest way is to:

1. Install Docker on your machine
2. Clone the repo or download the ZIP of the files.
3. Open up your console in the root directory of the project and run the following to build the Docker image:

    ```console
    docker build . --tag "log-monitor-service"
    ```
4. To run the docker image, run the following:

    ```console
    docker run --rm -p 5000:80 log-monitor-service
    ```
The service will start up on port `5000` of your localhost. You may hit the endpoint http://localhost/healthcheck to check whether the health of the application.

### Custom Settings

There are three different application settings that you can configure to your environment. In the **/LogMonitorService/appsettings.json** file, you can see the following JSON section:
```javascript
{
	"AppConfig": {
		"PathToLogs": "/var/log",           // Directory where all the log files are
		"DefaultNumberOfLogsToReturn": -1,  // Maximum number of logs to return from the API if number not provided. -1 for unlimited, returning all lines.
		"Encoding": "UTF-8"                 // Encoding of log files that will be read
  },
  ...
}
```
As the comments suggest:

**PathToLogs** is the directory containing the log files. Default value: `/var/log`

**DefaultNumberOfLogsToReturn** is the *default* max number of logs that the API will return if not provided in the query parameters (see API section). Setting the value to -1 means it'll simply return all logs. Default value: `-1`

**Encoding** is the encoding of the log files so that the application knows how to parse them correctly. Default value: `UTF-8`

Each of the above settings can either be updated in the appsettings.json and run the `docker build` command to rebuild or it can be set as environment variables during the `docker run` command like so:

```console
   docker run --rm -p 5000:80 -e AppConfig__PathToLogs="/var/log" -e AppConfig__DefaultNumberOfLogsToReturn=100 -e AppConfig__Encoding="UTF-8" log-monitor-service
```

### Mounting a directory containing your logs

You may have existing log files you want to test this against on the host machine. Easiest way is to mount the directory of your host machine containing your log files to the Docker container as a volume so that it has access to your logs. You may also want to adjust the **AppConfig__PathToLogs** environment variable as well to reflect the mount location. Example:

```console
docker run --rm -p 5000:80 -e AppConfig__PathToLogs="/tmp/mount/logs" -v "//c/users/alvin ho/documents/logs":"/tmp/mount/logs"  log-monitor-service
```
Please note that Docker only accepts lowercase paths.

# APIs

### Testing
Note to test the APIs you can either use curl (provided in each of their respective descriptions below) or use Postman. If you choose to use Postman, in their UI, go to Import > Raw Text and paste in the following:

``` javascript
{
	"info": {
		"_postman_id": "48905689-b690-40dd-991a-1f9ac697f4ea",
		"name": "LogMonitorService APIs",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
	},
	"item": [
		{
			"name": "Health Check",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "http://localhost:5000/healthcheck",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"healthcheck"
					]
				}
			},
			"response": []
		},
		{
			"name": "Request Logs",
			"request": {
				"method": "GET",
				"header": [],
				"url": {
					"raw": "http://localhost:5000/api/v1.0/logs/log.txt?numOfLogsToReturn=10&searchText=Debug",
					"protocol": "http",
					"host": [
						"localhost"
					],
					"port": "5000",
					"path": [
						"api",
						"v1.0",
						"logs",
						"log.txt"
					],
					"query": [
						{
							"key": "numOfLogsToReturn",
							"value": "10"
						},
						{
							"key": "searchText",
							"value": "Debug"
						}
					]
				}
			},
			"response": []
		}
	]
}
```

### GET /healthcheck

Example curl command:

`curl --location "http://localhost:5000/healthcheck"`

This endpoint does not need a request payload.

It returns a 200 status containing a JSON response object with the version and status like so:
``` javascript
{
    "version": "1.0.0.0",
    "status": "Log Monitor Service is healthy"
}
```
### GET /api/v1.0/jobs/{filename}

Example curl command:

`curl --location "http://localhost:5000/api/v1.0/logs/log.txt?numOfLogsToReturn=10&searchText=Debug"`

The **filename** portion of the API path is the name of the log file contained in the log directory on the server. That directory corresponds to the one provided in the appsettings.json (or environment variable) **AppConfig__PathToLogs**.

This endpoint uses the following query parameters:

Parameter | Description
------------- | -------------
SearchText | `Optional` The string of text to search in the log file. Only logs containing this substring will be returned by the API. This is **case-sensitive**.
NumOfLogsToReturn  | `Optional` This maximum number of logs that the API will return from the query. If not provided it will use the appsettings.json (or environment variable) **AppConfig__DefaultNumberOfLogsToReturn** property (which is -1 by default and returns unlimited logs).

The API streams the queried logs into the Response Body of the request returning logs from newest to oldest.
The response headers will be of **Content-Type**: **text/plain** with **Transfer-Encoding**: **chunked**.

# Improvements

Here are some improvements that can be done to improve the project:
- Unit tests! An easy unit test for example can be added to **LogsControllerService** for example to test the validation of the parameters.

- Design with primary / secondary servers in mind. What happens when you need to deploy this service to multiple servers? The primary server should know which secondary server to fetch logs from the file the user requested. This can be done by adding logic to allow for internal communication (via technology like Redis or Kafka to communicate what files are in which node. Infact Kafka is great at streaming files too.). If we do not want to use external technology, internal node requests could also use other means that do not rely on traditional REST APIs for faster communication (eg. gRPC).

- **ILogReaderService** can be implemented in multiple ways. The current implementation **DefaultLogReaderService** reads from the local file system, but this and **ReverseStreamReader** were designed in mind to support streams. Meaning we could add an implementation that opens a stream from anywhere (whether it is a primary server streaming a file from a secondary server or it is simply reading files on S3, Azure Blob Storage or any 3rd party cloud storage provider).

- User interface to easily query logs. The UI could list out possible files so that the user does not have to remember and manually type in the filename. It could also just show a text box for quick search of the selected log file. A button to export the result would also be amazing.

- Better pattern matching. The current implmentation in **DefaultLogReaderService** does simple byte comparison of the provided search string as it iterates the buffer to find the next line feed. While this is fast, it is very limited and case-sensitive. Instead of plain string for the search, we can support a better query structure to filter for logs (using contains, fuzzy matching, parsing JSON logs and filtering keys and values, etc), but that would require a lot of architecture thought and research into algorithms to avoid performance impacts