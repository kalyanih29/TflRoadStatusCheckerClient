# TflRoadStatusCheckerClient
A Commandline client that uses the Tfl REST API to check the status of the major roads for a given road name.

o	**Prerequisites**

As this application is built on .NET5, we need the following tools to Build and Run the program on a Windows machine:
1. Latest version of Visual Studio 2019: https://visualstudio.microsoft.com/downloads/
2. .NET 5 Runtimes: https://dotnet.microsoft.com/download/dotnet/5.0
3. Powershell: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.1


o	**Build**

To build the project, get the master branch clone or download the zip on your local machine.
Once the source code is successfully copied/cloned on your local machine, follow the following steps:
1. Change the API Keys (AppId and ApiKey):
To change the keys, please open the appsettings.json, appsettings.production.json and appsettings.development.json file.
Change the highlighted values shown below with your own keys:


[![keys-change.png](https://i.postimg.cc/fLK3fcj1/keys-change.png)](https://postimg.cc/DJSy2bNc)

2. Right click the solution and build the solution.
This should display in the output window of Visual Studio that the build is successful.


o	**Run the output**

1. Open the Windows Powershell Command tool
2. Browse to you projects ".\bin\[configuration]\net5.0" where it can be release or debug configuration
3. Copy the above path and execute the following commands like below:
 cd C:\Kalyani\Documents\Personal\Profile\RoadStatusCheckerClient\TflRoadStatusCheckerClient\TflRoadStatusCheckerClient\bin\Debug\net5.0
 & .\TflRoadStatusCheckerClient.exe A2
4. The sample commands are as shown below:
[![Output.png](https://i.postimg.cc/7Z3mrcxz/Output.png)](https://postimg.cc/pmdDDs5V)

o	**Run the tests written**

In Visual Studio, under the project: TflRoadStatusCheckerClient.Tests, there is a test class called 'TflRoadStatusCheckerApiServiceTest'
Under the Test Explorer options-> Run All the tests or within the mentioned class for each test right click the [Test] keyword and choose to Run

o	**Assumptions**

1. Windows machine
2. Tfl API is available and the keys provided are correct- Incase, of the wrong keys the program is able to handle this scenario


