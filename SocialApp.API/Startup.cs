using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SocialApp.API.Data;
using Microsoft.AspNetCore.Http;
using SocialApp.API.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using Microsoft.AspNetCore.Authentication;

namespace SocialApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

           // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           

            //service add
            //have to add manually
            //nuget sqlite -> install -> restore project -> check csproj file -> fix using statement
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));


            //1.) appsettings connection string db file name
            //2.)Cors

            //AddCors 
            services.AddCors();
            //Add Cloud providers here below CORS
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            //we need to save every photoid that is returned ( public id of photo)

            services.AddAutoMapper();
            //ordering is important

            services.AddTransient<Seed>();

            //3. MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).
            AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //1.singleton create a single instance of repository, uses this same object, creates issues during concurrent requests

            //2.transient useful for lightweight stateless services, each time request comes, an instance of repository is created

            //3.AddScoped // means that the service is created once per request in the scope , i.e in the current scope itself.

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ISocialRepository, SocialRepository>();

            //Authentication Middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            services.AddScoped<LogUserActivity>();


        

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {

               services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(60);
        
    });

    services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 5001;
    });
       
            //service add
            //have to add manually
            //nuget sqlite -> install -> restore project -> check csproj file -> fix using statement
            //services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
           services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));

            //1.) appsettings connection string db file name
            //2.)Cors

            //AddCors 
            services.AddCors();
            //Add Cloud providers here below CORS
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            //we need to save every photoid that is returned ( public id of photo)

            services.AddAutoMapper();
            //ordering is important

            services.AddTransient<Seed>();

            //3. MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).
            AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //1.singleton create a single instance of repository, uses this same object, creates issues during concurrent requests

            //2.transient useful for lightweight stateless services, each time request comes, an instance of repository is created

            //3.AddScoped // means that the service is created once per request in the scope , i.e in the current scope itself.

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ISocialRepository, SocialRepository>();

            //Authentication Middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            services.AddScoped<LogUserActivity>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                //on exception, app uses global exception handler, and outputs this exception page
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);//Access Control Headers enabled
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });

                });//exception handler middleware
                //From https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.1&tabs=visual-studio
                 // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
       
               // app.UseHsts();
            }




            /*
            app.UseHsts();

This enables HSTS, which is a HTTP/2 feature to avoid man-in-the-middle attacks. It tells the browser to cache the certificate for the specific host-headers for a specific time range. If the certificate changes before the time range ends, something is wrong with the page. (more about HSTS)

The next new middleware redirects all requests without HTTPS to use the HTTPS version:

app.UseHttpsRedirection();

If you call http://localhost:5000, you get redirected to https://localhost:5001. This makes sense if you want to enforce HTTPS.

So from the ASP.NET Core perspective all is done to run the web using HTTPS. Unfortunately the Certificate is missing. For the production mode you need to buy a valid trusted certificate and to install it in the windows certificate store. For the Development mode, you are able to create a development certificate using Visual Studio 2017 or the .NET CLI. VS 2017 is creating a certificate for you automatically.

Using the .NET CLI tool "dev-certs" you are able to manage your development certificates, like exporting them, cleaning all development certificates, trusting the current one and so on. Just time the following command to get more detailed information:
            */
           //app.UseHttpsRedirection();

            //Drop database then add values to database again uncomment line below
            //seeder.SeedUsers();//THIS LINE 
            //app.UseCors(x => x.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials());//since were not allowing AllowCredentials we are not in a sec risk
            
            app.UseHsts();
         
            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication(); //MIDDLEWARE
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseRouting();

            //app.UseAuthorization();

            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Fallback", action = "Index"}
                );
            });
        }
    }
}
/*
GENERATE COMPONENT IN SRC FOLDER by name of value , right click mouse

it automatically creates the declarations for us inside the app.module.ts file


"@angular/http": "^6.0.3",  // this is deprecated will be removed in angular 7

http client lives inside angular common .. we will use that

//make http get request
//get values from server


values: any; //js variable


* while auto importing..... use tab or fix missing whitespace


 url: string , options? get method

return an observable of body as an object... it is a stream of data coming back from server

response will be in values property



component load

constructor is early


ngonInit happens after constructor ... write commands there

before running angular app
goto dotnet web api and run dotnet watch run
then go to angular dir and run ng serve

Access to XMLHttpRequest at 'http://localhost:5000/api/values' 
from origin 'http://localhost:4200' has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource.





CORS is a security measure, it allows api to restrict which clients ( angular apps ) are allowed to access our api
this is done through headers.

we are going to different domains

localhost:5000 from localhost:4200
from 1 domain to different domain


values is a class property
this. something to access it


this.values

class method to access

we are inside context of the value class

go to value.comp.html

in p tag type ngFor

<p *ngFor="let item of list">
  value works!
</p>


structural directives change dom, they have an asterisk before them



step fin for structure of app
npm install bootstrap font-awesome


npm WARN ajv-keywords@3.4.1 requires a peer of ajv@^6.9.1 but none is installed. You must install peer dependencies yourself.
npm WARN bootstrap@4.3.1 requires a peer of jquery@1.9.1 - 3 but none is installed. You must install peer dependencies yourself.
npm WARN bootstrap@4.3.1 requires a peer of popper.js@^1.14.7 but none is installed. You must install peer dependencies yourself.
npm WARN optional SKIPPING OPTIONAL DEPENDENCY: fsevents@1.2.9 (node_modules\fsevents):
npm WARN notsup SKIPPING OPTIONAL DEPENDENCY: Unsupported platform for fsevents@1.2.9: wanted {"os":"darwin","arch":"any"} (current: {"os":"win32","arch":"x64"})

+ bootstrap@4.3.1
+ font-awesome@4.7.0
added 2 packages from 7 contributors and audited 34647 packages in 14.345s
found 19 vulnerabilities (12 low, 7 high)
  run `npm audit fix` to fix them, or `npm audit` for details

we will use alternative of jquery


BOOTSTRAP
FONTAWESOME
CSS
OUR OWN

CASCADING

Alertifyjs
Angular JWT
NGX Bootstrap
Bootswatch

*/
