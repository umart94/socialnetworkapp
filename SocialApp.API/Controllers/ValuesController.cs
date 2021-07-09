using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApp.API.Data;

namespace SocialApp.API.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]

    //Apicontroller attribute
    //1.enforces rule, attribute routing rather than conventional routing
    //2. validating requests.
    //3. controller based usage (it inherits from base MVC Controller)
    //4.controller on its own has viewsupport
    //5.controllerbase does NOT HAVE viewsupport
    //MVC is just model controller, and angular will provide our views.
    public class ValuesController : ControllerBase
    {
        //when browsing app http://localhost:5000/api/[controller] is used (kestral server)
        //when browsing app http://localhost:5000/api/values is used (kestral server) (first name/part of controller)
        //angular framework ->controller -> perform action -> REST API ( hit ActionResult Method)
        //see comment above each function for each specific route.
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            this._context = context;

        }


        // [Authorize]
        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            //IEnumerable is a collection of things <IEnumerable<string>> Get
            //remove it and use IActionResult, (http responses , 200 OK response)

            //throw new Exception("Le Bhai");
            // return new string[] { "value1", "value2" };

            //when this call ToList is made
            //the thread will be blocked, code is synchronous, will retrieve full list then continue
            // we want to make our code asynchronous, so our app will also handle other requests simultaneously

            var values = await _context.Values.ToListAsync();//await this operation until completes, use async version of tolist method
                                                             //USE ASYNC CODE AS MUCH AS POSSIBLE

            return Ok(values);
        }

        /*  
        SYNCHRONOUS DUMMY FUNCTION
         public IActionResult GetValues()
        {
        var values = _context.Values.ToList();
           return Ok(values);
        }*/

        /*  
      ASYNCHRONOUS DUMMY FUNCTION
       public async Task<IActionResult> GetValues()
      {
      var values = await _context.Values.ToListAsync();
         return Ok(values);
      }*/



        // GET api/values/5
        //..|| root parameter HttpGet(id)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            //it gives http 204 Ok no content , as there was no default value given

            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
