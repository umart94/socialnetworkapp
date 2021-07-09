using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SocialApp.API.Data;
using System;// for date time..
/*
ASP.NET MVC - Action Filters
In the previous section, you learned about filters in MVC. In this section, you will learn about another filter type called Action Filters in ASP.NET MVC.

Action filter executes before and after an action method executes. Action filter attributes can be applied to an individual action method or to a controller. When action filter applied to controller then it will be applied to all the action methods in that controller.

OutputCache is a built-in action filter attribute that can be apply to an action method for which we want to cache the output. For example, output of the following action method will be cached for 100 seconds.

Example: ActionFilter
[OutputCache(Duration=100)]
public ActionResult Index()
{
return View();
}
You can create custom action filter for your application. Let's see how to create custom action filters.

Custom Action Filter
You can create custom action filter by two ways. First, by implementing IActionFilter interface and FilterAttribute class. Second, by deriving ActionFilterAttribute abstract class.

IActionFilter interface include following methods to implement:

void OnActionExecuted(ActionExecutedContext filterContext)
void OnActionExecuting(ActionExecutingContext filterContext)
ActionFilterAttribute abstract class includes the following methods to override:

void OnActionExecuted(ActionExecutedContext filterContext)
void OnActionExecuting(ActionExecutingContext filterContext)
void OnResultExecuted(ResultExecutedContext filterContext)
void OnResultExecuting(ResultExecutingContext filterContext)
As you can see that ActionFilterAttribute class has four methods to overload. It includes OnResultExecuted and OnResultExecuting methods, which can be used to execute custom logic before or after result executes. Action filters are generally used to apply cross-cutting concerns such as logging, caching, authorization etc.

Consider the following custom Log filter class for logging.

Example: Custom ActionFilter for Logging
public class LogAttribute : ActionFilterAttribute
{
public override void OnActionExecuted(ActionExecutedContext filterContext)
{
Log("OnActionExecuted", filterContext.RouteData); 
}

public override void OnActionExecuting(ActionExecutingContext filterContext)
{
Log("OnActionExecuting", filterContext.RouteData);      
}

public override void OnResultExecuted(ResultExecutedContext filterContext)
{
Log("OnResultExecuted", filterContext.RouteData);      
}

public override void OnResultExecuting(ResultExecutingContext filterContext)
{
Log("OnResultExecuting ", filterContext.RouteData);      
}

private void Log(string methodName, RouteData routeData)
{
var controllerName = routeData.Values["controller"];
var actionName = routeData.Values["action"];
var message = String.Format("{0}- controller:{1} action:{2}", methodName, 
                                           controllerName, 
                                           actionName);
Debug.WriteLine(message);
}
}
As you can see, Log class derived ActionFilterAttribute class. It logs before and after action method or result executes. You can apply Log attribute to any Controller or action methods where you want to log the action. For example, by applying Log attribute to Controller, it will log each action methods of that controller.

Example: Apply Log ActionFilter to Controller
[Log]
public class StudentController : Controller
{
public ActionResult Index()
{
return View();
}

public ActionResult About()
{
return View();
}

public ActionResult Contact()
{
return View();
}
}
The above example will show following output in the output window of Visual Studio on http://localhost/student request.
*/


namespace SocialApp.API.Helpers
{
    //ASP.NET CORE MVC FILTERS
    //ON ACTION EXECUTION ASYNC METHOD TAKES
    // context and next
    //context is what you want to do when action is being executed
    // next allows us to execute portion of code after the action is executed
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //GetService not working ... so you have to import using Microsoft.Extensions.DependencyInjection;
            var repo = resultContext.HttpContext.RequestServices.GetService<ISocialRepository>();
            //get the user
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();

        }
    }
}