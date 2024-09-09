# TaskManagementSystem
.NET backend service that manages a simple task management system with user authentication.

## Prerequirements

* Visual Studio
* .NET Core SDK
* SQL Server

## How To Run

* Open solution in Visual Studio
* Set TaskManagementSystem project as Startup Project and build the project.
* Run the application.

## Endpoints
* POST api/users/register: To register a new user.
* POST api/users/loginWithUserName: To authenticate a user with username and get a JWT.
* POST api/users/loginWithUserEmail: To authenticate a user with email and get a JWT.
* POST api/users/changePassword: To change the user`s password.
* POST api/tasks: To create a new task (authenticated).
* GET api/tasks: To retrieve a list of tasks for the authenticated user, with optional filters (e.g., status, due date, priority).
* GET api/tasks/{id}: To retrieve the details of a specific task by its ID (authenticated).
* PUT api/tasks/{id}: To update an existing task (authenticated).
* DELETE api/tasks/{id}: To delete a specific task by its ID (authenticated).

## Create dbsettings.json
Create file `appsettings.json` into /api with right settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementSystem;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "../Logs/Serilog.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3}] {Username} {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "AllowedHosts": "*",
  "JWT": {
    "Issuer": "http://localhost:port",
    "Audience": "http://localhost:port",
    "SigninKey": "512 bit key"
  }
}
```