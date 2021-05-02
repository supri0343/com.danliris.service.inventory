# com.danliris.service.inventory
[![Build Status](https://travis-ci.org/danliris/com.danliris.service.inventory.svg?branch=dev)](https://travis-ci.org/danliris/com.danliris.service.inventory) [![codecov](https://codecov.io/gh/danliris/com.danliris.service.inventory/branch/dev/graph/badge.svg)](https://codecov.io/gh/danliris/com.danliris.service.inventory)

DanLiris Application is a enterprise project that aims to manage the business processes of a textile factory, PT. DanLiris.
This application is a microservices application consisting of services based on .NET Core and Aurelia Js which part of  NodeJS Frontend Framework. This application show how to implement microservice architecture principles. com-danliris-service-finance-accounting repository is part of service that will serve  inventory business activity.

## Prerequisites
* Windows, Mac or Linux
* [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/vs/whatsnew/)
* [IIS Web Server](https://www.iis.net/) 
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [.NET Core SDK](https://www.microsoft.com/net/download/core#/current) (v2.0.9,  SDK 2.1.202, ASP.NET Core Runtime 2.0.9 )


## Getting Started

- Fork the repository and then clone the repository using command  `git clone https://github/YOUR-USERNAME/com.danliris.service.inventory.git`  checkout the `dev` branch.


### Command Line

- Install the latest version of the .NET Core SDK from this page <https://www.microsoft.com/net/download/core>
- Next, navigate to root project or wherever your folder is on the command line in administrator mode.
- Create empty database.
- Setting connection to database using Connection Strings in appsettings.json. Your appsettings.json look like this:

```
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=YourDbServer;Database=your_parent_database;Trusted_Connection=True;MultipleActiveResultSets=true",
  },
  "ClientId": "your ClientId",
  "Secret": "Your Secret",
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```
and  Your appsettings.Developtment.json look like this :
```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```
- Make sure port application has no conflict, setting port application in launchSettings.json
```
com.danliris.service.inventory
 ┣ Com.Danliris.Service.Inventory.WebApi
    ┗ Properties
       ┗ launchSettings.json
```

file launchSettings.json look like this :
```
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:54774/",
      "sslPort": 0
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Com.Danliris.Service.Inventory.WebApi": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```
- Call `dotnet run`.
- Then open the `http://localhost:54774` URL in your browser.

### Visual Studio

- Download Visual Studio 2019 (any edition) from https://www.visualstudio.com/downloads/ .
- Open `Com.Danliris.Service.Inventory.sln` and wait for Visual Studio to restore all Nuget packages.
- Create empty database.
- Setting connection to database using Connection Strings in appsettings.json. Your appsettings.json look like this:

```
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=YourDbServer;Database=your_parent_database;Trusted_Connection=True;MultipleActiveResultSets=true",
  },
  "ClientId": "your ClientId",
  "Secret": "Your Secret",
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```
and  Your appsettings.Developtment.json look like this :
```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```
- Make sure port application has no conflict, setting port application in launchSettings.json.
```
com.danliris.service.inventory
 ┣ Com.Danliris.Service.Inventory.WebApi
    ┗ Properties
       ┗ launchSettings.json
```
file launchSettings.json look like this :
```
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:54774/",
      "sslPort": 0
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Com.Danliris.Service.Inventory.WebApi": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

- Ensure `Com.Danliris.Service.Inventory.WebApi` is the startup project and run it and the browser will launched in new tab http://localhost:54774/swagger/index.html


### Run Unit Tests in Visual Studio 
1. You can run all test suite, specific test suite or specific test case on test explorer.
2. Choose Tab Menu **Test** to select differnt menu test.
3. Select **Run All Test** or press (Ctrl + R, A ) to run all test suite.
4. Select **Test Explorer** or press (Ctrl + E, T ) to determine  test suite to run specifically.
5. Select **Analyze Code Coverage For All Test** to generate code coverage. 


## Knows More Details
### Root directory and description

```
com.danliris.service.inventory
 ┣ Com.Danliris.Service.Inventory.Lib
 ┣ Com.Danliris.Service.Inventory.Test
 ┣ Com.Danliris.Service.Inventory.WebApi
 ┣ TestResults
 ┣ .codecov.yml
 ┣ .gitignore
 ┣ .travis.yml
 ┣ Com.Danliris.Service.Inventory.sln
 ┗ README.md
 ```

**1. Com.Danliris.Service.Inventory.Lib**

This folder consists of various libraries, domain Models, View Models, and Business Logic.The Model and View Models represents the data structure. Business Logic has responsibility  to organize, prepare, manipulate, and organize data. The tasks are include entering data into databases, updating data, deleting data, and so on. The model carries out its work based on instructions from the controller.


AutoMapperProfiles:

- Colecction class to setup mapping data 

BusinessLogic

- Colecction of classes to prepare, manipulate, and organize data, including CRUD (Create, Read, Update, Delete ) on database.

Models:

- The Model is a collection of objects that Representation of data structure which hold the application data and it may contain the associated business logic.

ViewModels

- The View Model refers to the objects which hold the data that needs to be shown to the user.The View Model is related to the presentation layer of our application. They are defined based on how the data is presented to the user rather than how they are stored.

ModelConfigs

- Collection of classes to setup entity model  that will be used in EF framework to generate schema database.

Migrations

- Collection of classes that generated by EF framework  to setup database and the tables.


PdfTemplates

- Collection of classes to generate report in pdf format.


Helpers 

- Collection of helper classes that frequently used in various cases. 


Utilities

- Collection of classes that frequently used as utility in various class. 

Services

- Collection of classes and interfaces to validation and authentication user.


The folder tree in this folder is:

```
com.danliris.service.inventory
 ┣ Com.Danliris.Service.Inventory.Lib
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Configs
 ┃ ┃ ┣ FpRegradingResultDocsConfig
 ┃ ┃ ┣ FpReturnFromBuyerConfigs
 ┃ ┃ ┣ FPReturnInvToPurchasingConfig
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ InventoriesConfig
 ┃ ┃ ┣ MaterialDistributionNoteConfig
 ┃ ┃ ┣ MaterialRequestNoteConfig
 ┃ ┃ ┗ StockTransferNoteConfig
 ┃ ┣ Enums
 ┃ ┣ Facades
 ┃ ┃ ┣ InventoryFacades
 ┃ ┣ Helpers
 ┃ ┣ Interfaces
 ┃ ┣ Migrations
 ┃ ┣ Models
 ┃ ┃ ┣ FpRegradingResultDocs
 ┃ ┃ ┣ FpReturnFromBuyers
 ┃ ┃ ┣ FPReturnInvToPurchasingModel
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ InventoryModel
 ┃ ┃ ┣ MaterialDistributionNoteModel
 ┃ ┃ ┣ MaterialRequestNoteModel
 ┃ ┃ ┗ StockTransferNoteModel
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ PDFTemplates
 ┃ ┣ Services
 ┃ ┃ ┣ FpRegradingResultDocs
 ┃ ┃ ┣ FpReturnFromBuyers
 ┃ ┃ ┣ FPReturnInvToPurchasingService
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ Inventory
 ┃ ┃ ┣ MaterialDistributionNoteService
 ┃ ┃ ┣ MaterialRequestNoteServices
 ┃ ┃ ┣ StockTransferNoteService
 ┃ ┣ ViewModels
 ┃ ┃ ┣ FpRegradingResultDocs
 ┃ ┃ ┣ FpReturnFromBuyers
 ┃ ┃ ┣ FPReturnInvToPurchasingViewModel
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ InventoryDocumentViewModel
 ┃ ┃ ┣ InventoryViewModel
 ┃ ┃ ┣ MaterialDistributionNoteViewModel
 ┃ ┃ ┣ MaterialRequestNoteViewModel
 ┃ ┃ ┣ StockTransferNoteViewModel
 ┃ ┣ Com.Danliris.Service.Inventory.Lib.csproj
 ┃ ┗ InventoryDbContext.cs

 ```


**2. Com.Danliris.Service.Inventory.WebApi**

This folder consists of controller API. The controller has responsibility to processing data and  HTTP requests and then send it to a web page. All responses from the HTTP requests API are formatted as JSON (JavaScript Object Notation) objects containing information related to the request, and any status.

The folder tree in this folder is:

```
 ┣ Com.Danliris.Service.Inventory.WebApi
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┃ ┃ ┃ ┣ Properties
 ┃ ┣ Controllers
 ┃ ┃ ┗ v1
 ┃ ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┃ ┣ Loaders
 ┃ ┣ Helpers
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Properties
 ┃ ┃ ┗ launchSettings.json
 ┃ ┣ Com.Danliris.Service.Inventory.WebApi.csproj
 ┃ ┣ Com.Danliris.Service.Inventory.WebApi.csproj.user
 ┃ ┣ Program.cs
 ┃ ┗ Startup.cs
 ```

**3. Com.Danliris.Service.Inventory.Test**

This folder is collection of classes to run code testing. The automation type testing used in this app is  a unit testing with using moq and xunit libraries.

DataUtils:

- Colecction class to seed data as data input in unit test 

The folder tree in this folder is:

```
 ┣ Com.Danliris.Service.Inventory.Test
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┃ ┃ ┃ ┣ Properties
 ┃ ┣ Controllers
 ┃ ┃ ┣ FpRegradingResultDocs
 ┃ ┃ ┣ FpReturnFromBuyer
 ┃ ┃ ┣ FPReturnInvToPurchasing
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ InventoryDocument
 ┃ ┃ ┣ InventoryMovementReport
 ┃ ┃ ┣ InventorySummaryReport
 ┃ ┃ ┣ MaterialDistributionNote
 ┃ ┃ ┣ MaterialRequestNote
 ┃ ┃ ┗ StockTransferNote
 ┃ ┣ DataUtils
 ┃ ┃ ┣ FpRegradingResultDataUtil
 ┃ ┃ ┣ FpReturnFromBuyerDataUtil
 ┃ ┃ ┣ FPReturnInvToPurchasingDataUtil
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┗ ReceiptFinishedGood
 ┃ ┃ ┣ IntegrationDataUtil
 ┃ ┃ ┣ InventoryDataUtils
 ┃ ┃ ┣ MaterialDistributionNoteDataUtil
 ┃ ┃ ┣ MaterialRequestNoteDataUtil
 ┃ ┃ ┗ StockTransferNoteDataUtil
 ┃ ┣ Facades
 ┃ ┃ ┣ FPReturnInvToPurchasing
 ┃ ┃ ┗ Inventory
 ┃ ┣ Helpers
 ┃ ┣ Interfaces
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Services
 ┃ ┃ ┣ FpRegradingResult
 ┃ ┃ ┣ FpReturnFromBuyer
 ┃ ┃ ┣ GarmentLeftoverWarehouse
 ┃ ┃ ┃ ┣ ExpenditureAval
 ┃ ┃ ┃ ┣ ExpenditureFabric
 ┃ ┃ ┃ ┣ ExpenditureFinishedGood
 ┃ ┃ ┃ ┣ ReceiptAval
 ┃ ┃ ┃ ┣ ReceiptFabric
 ┃ ┃ ┃ ┣ ReceiptFinishedGood
 ┃ ┃ ┃ ┗ Stock
 ┃ ┃ ┣ MaterialDistributionNote
 ┃ ┃ ┣ MaterialRequestNote
 ┃ ┃ ┗ StockTransferNote
 ┃ ┣ Com.Danliris.Service.Inventory.Test.csproj
 ┃ ┣ ServiceProviderFixture.cs
 ┃ ┗ TestServerFixture.cs
```

**TestResults**

- Collections of files generated by the system for purposes of unit test code coverage.

**InventoryDbContext.cs**

This file contain context class that derives from DbContext in entity framework. DbContext is an important class in Entity Framework API. It is a bridge between domain or entity classes and the database. DbContext and context class  is the primary class that is responsible for interacting with the database.

**File Program.cs**

Important class that contains the entry point to the application. The file has the Main() method used to run the application and it is used to create an instance of WebHostBuilder for creating a host for the application. The Startup class to be used by the application is specified in the Main method.

**File Startup.cs**

This file contains Startup class. The Startup class configures services and the app's request pipeline.Optionally includes a ConfigureServices method to configure the app's services. A service is a reusable component that provides app functionality. Services are registered in ConfigureServices and consumed across the app via dependency injection (DI) or ApplicationServices.This class also Includes a Configure method to create the app's request processing pipeline.

**File docker-compose.test.yml**

File that configure docker compose. Docker compose provides a way to orchestrate multiple containers that work together.

**File Dockerfile**

A Dockerfile is a text document that contains all the commands a user could call on the command line to assemble an image.

**File .travis.yml**

Travis CI (continuous integration) is configured by adding a file named .travis.yml. This file in a YAML format text file, located in root directory of the repository. This file specifies the programming language used, the desired building and testing environment (including dependencies which must be installed before the software can be built and tested), and various other parameters.

**File .codecov.yml**

This file is used to configure code coverage in unit tests.

**Com.Danliris.Service.Inventory.sln**

File .sln is extention for *solution* aka file solution for .Net Core, this file is used to manage all project by code editor.

 ### Validation
Data validation using **IValidatableObject**