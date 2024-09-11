# Module Management Application

## Version
v3.0.0

## Hardware Specifications
The application can run on any hardware with the following minimum specifications:

- 2 GHz or faster processor
- 2 GB of RAM, or more
- 500 MB of free hard disk space

# NEW!
- The entire application has been overhauled to be used a web application instead of a WPF (Windows Presentation Forms) application.
- Still works the same as the WPF application but can be hosted to the web.

## Installation Instructions - Locally
To install and run the application, follow these steps:
- (SQL Server needs to be installed as this software uses a local database called '(localdb)\\mssqllocaldb' on your system)
- Visual Studio 2022 needs to be installed with support for MVC. Check 1.3 in USER MANUAL for more info.

- Clone the repository to your local machine or download and extract the ZIP file.
- Unzip the file
- Go to "...\prog6212-poe-ST10067405\POEPart3.sln"
- Run the POEPart3.sln file
- Click the 'run without debug' and it should install database schema if sql server installed correctly.
- Enjoy :D

# How To Use
This application is a module management website that allows the user to manage thier semesters, modules, and Module Records for each module.

## Semester Functionality
You will be able to add multiple semesters that can:
- Hold multiple modules.
- Hold number of weeks per semester.
- hold a start date per semester.
- And will calculate end date with the number of weeks automatically.
- Options to edit & delete a semester.
  
## Module Functionality
You will be able to add multiple modules that can: 
- It can create a module with Code, Name, Credits, Class hours per week.
- Automatically calculates self-study hours per week.
- Options to edit & delete a module.
  
## Module Records Functionality
- You will be able to add how many hours you worked for each week
- By entering hours worked, selecting the semester and module, and the day you worked.
- It will be displayed on the home page in a graph.
- Options to edit & delete a record.

# FAQs
1. Q) What is this software/application?
- A) This software/application is a Module Management System that can store and display your modules. It can use the data to calculate your self-study hours and has the functionality to update and delete data.

2. Q) How do I get started with using this software/application?
- A) Follow the instruction under Installation Instructions. Check out USER MANUAL for more.

3. Q) How do I delete a module?
- A) Click the delete button nexto the row that you want deleted.

4. Q) How do I update the software to the latest version??
- A) Download the lastest tag on GitHub and follow the instructions above.

## Code Attributions
This application was created with the help of the following resources:

- Microsoft .NET documentation: https://docs.microsoft.com/en-us/dotnet/
- C# documentation: https://docs.microsoft.com/en-us/dotnet/csharp/
- 'Pro C# 9 with .NET 5: Foundational Principles and Practices in Programming' - Tenth Edition by Andrew Troelsen & Phillip Japikse.

## Part 2 Changes From Feedback
Applied changes from my feedback from part 2:
- update readme to depict better instructions.

## Dev Info
- The application was created by Jaime Marc Futter.
- Student Number: ST10067405
- If any issues may arise, contact me via my email at: 
ST10067405@vcconnect.edu.za
- GitHub Repository Link: [https://github.com/VCWVL/prog6212-poe-ST10067405]

## Frameworks and Plugins Used
- AspNetCoreHero.ToastNotification v1.1.0
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore v6.0.23
- Microsoft.AspNetCore.Identity.EntityFrameworkCore v6.0.23
- Microsoft.AspNetCore.Identity.UI v6.0.23
- Microsoft.EntityFrameworkCore.Sqlite v6.0.23
- Microsoft.EntityFrameworkCore.SqlServer v6.0.23
- Microsoft.EntityFrameworkCore.Tools v6.0.23
- Microsoft.VisualStudio.Web.CodeGeneration.Design v6.0.16
- ModulesLibrary DLL File v2.0.0
- .NET Framework 6.0
- Entity Framework 6 (EF6)
