using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
/*
STEP-1 TEMPLATE OF PROJECT

dotnet new webapi -o SocialNetworkApp.API -n SocialNetworkApp.API

write 


D:\app\code . (from cmd , opens code directory in visual studio code)

CTRL+SHIFT+P (SEARCH COMMANDS)



 //Entry Point Of Application
            //CreateWebHostBuilder
            //use Kestrel Webserver and configure it
            CreateWebHostBuilder(args).Build().Run();
        }

        //Startup class configure services method
        //we can add services MVC setcompatibility, HTTP request pipeline developerexceptionpage
        //HSTS is a security enhancement (strict transport security header) 
        //https redirection




STEP-2
//Create Database using Entity Framework Migrations

dotnet ef migrations add InitialCreate  ///create migrations scaffolding

info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 2.1.11-servicing-32099 initialized 'DataContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None
Done. To undo this action, use 'ef migrations remove'

designer -> what to remove from migrations snapshot file

iimportant file is the timestamp file

Up method and down method

Up method = all info entity framework needs to create and change our database
has a createtable method
two columns Id and Name -> based on our model Value file

Id automatically used for primary key attribute ( convention-based)



Down method

if we want to rollback migration
this method will drop table
we can create database from this migration


STEP-3 CREATE DATABASE AND TABLES


//

will create db and tables


hookup controller to database to get our values










STEP-4 CREATE ANGULAR PROJECT
USING ANGULAR CLI


npm install -g @angular/cli

npm install -g @angular/cli@6.0.8 (stable,6 month stable release)


 //vulnerabilities are in 3rd party packages, eg. 13 vuln , 9 low ,  4 high so we won't fix them
//keep updating dependencies

*/
namespace SocialApp.API
{
    public class Program
    {
        //Dot net project does not allow dashes in project name
        //Angular does not allow . in project name




        //Ctrl+Shift+P Restart OmniSharp


        //for this project we will use Model first approach
        //create a model based on values
        //retrieve data from db
        public static void Main(string[] args)
        {
            //Entry Point Of Application
            //CreateWebHostBuilder
            //use Kestrel Webserver and configure it
            CreateWebHostBuilder(args).Build().Run();
        }

        //Startup class configure services method
        //we can add services MVC setcompatibility, HTTP request pipeline developerexceptionpage
        //HSTS is a security enhancement (strict transport security header) 
        //https redirection

       
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
               

    }



    //launchSettings.json    
    //launchBrowser (open in a browser)
    //use single applicationUrl
    //start in development mode

    //appsettings.Development.json
    //used in development environment
    //loglevels  additional information shown in terminal while running/execution/api requests etc.


    //Production mode will not show the default exception page, it will just show the HTTP STATUS CODE e.g 500 failed to load resource
    //check google chrome developer tools for the actual detailed error (network -> headers -> preserve log) or terminal in vs code
    //the error will be traced using fail tag, shows stacktrace.
}


/*
//e2e related to end to end testing
//node_modules is for all dependencies
//package.json angular dependencies downloaded in node_modules folder

//avoid deprecated code

start , build , test are ng commands 

ng serve is used to start an angular app 

//src folder contains all the sources for our angular application


//all components that we write will be in app folder
//some pre-built components
//spec.ts related to testing apps

//every angular app has to have atleast 1 app.module.ts file

ngmodule is loaded... it bootstraps appcomponent

inside appcomponent its decorated with angular core component

typescript class that has angular core features

//inside component
//1 selector
//2 template url
//3 style url


we will be served app.component.html


welcome to {{ title }} this curly brace syntax is called interpolation, title will be replaced with title in appcomponent 


SPA app ... app-root in selector means in index.html we have app root
so contents of approot will be replaced with whats inside app.component.html

(kind of like the php templates)









-------

main.ts
platform browser dynamic method ... web app dynamic.... bootstrap app module ... which loads html on the front page

how does main.ts get into index.html ?

ANSWER = ANGULAR

WEBPACK is a module bundler
it will bundle our html js into our html file when it indexes and builds it into our angular app

the settings and configs are in angular.json
it is quite complex to configure

it has the settings that webpack needs

// it has reference to main src/main.ts and index src/index.html

this is bootstrap process






ERROR in node_modules/rxjs/internal/types.d.ts(81,44): error TS1005: ';' expected.
node_modules/rxjs/internal/types.d.ts(81,74): error TS1005: ';' expected.
node_modules/rxjs/internal/types.d.ts(81,77): error TS1109: Expression expected.
node_modules/rxjs/internal/types.d.ts(82,52): error TS1005: ';' expected.
node_modules/rxjs/internal/types.d.ts(82,88): error TS1005: ';' expected.
node_modules/rxjs/internal/types.d.ts(82,92): error TS1109: Expression expected.


for this error change rxjs from ^6.0.0 to 6.0.0 in package.json

to run

1. terminal ng serve from root dir
2. localhost:4200










*/

/*
create login and navbar
angular template forms
using services in angular
inject services into our components
conditionally display elements on page

don't show all info / show some of the info etc. on login page ,after login page etc.

create register component , parent component and communicate between components

*/
