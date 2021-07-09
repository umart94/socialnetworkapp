using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialApp.API.DTOS;
using SocialApp.API.Data;
using SocialApp.API.Models;
using AutoMapper;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //Comment this to get the Exception in Postman

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        //api/auth
        //api/values
        //api/firstPartOfNameofControllerClass(beforecontroller)

        public AuthController(IAuthRepository repo, IConfiguration _config, IMapper mapper)
        {
            _mapper = mapper;
            this._config = _config;
            this._repo = repo;
        }

        //HttpPost
        [HttpPost("register")]

        //Note instead of string username, string password
        // we use the class object
        //UserForRegisterDTO

        //[FromBody] infer
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {

            //we will recieve our username and password as a JSON object
            //validate in lowercase

            //username = username.ToLower();
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();//ERROR HERE - cant convert to lowercaseusername.. IFF WE REMOVE APICONTROLLER
                                                                                //if username is null, then Null reference Exception

            //null is not === empty string

            //validate request, for null or empty or invalid usernames by making use Of ModelState and [FromBody] in function parameters. 
            /****Change as per errors / data annotations ******/
            //NOT NEEDED IF USING APICONTROLLER , [FromBody] in parameter of function also not needed

            /* if(!ModelState.IsValid){
                 return BadRequest(ModelState);
             }*/
            /****Change as per errors / data annotations ******/


            if (await _repo.UserExists(userForRegisterDTO.Username))
                return BadRequest(); //HTTP 400 BAD REQUEST


            /*var userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };*/

            var userToCreate = _mapper.Map<User>(userForRegisterDTO);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            var userToReturn = _mapper.Map<UserForDetailedDTO>(createdUser);
            
            //return createdAtRoute
            //return StatusCode(201);//The request has been fulfilled and has resulted in one or more new resources being created.

            return CreatedAtRoute("GetUser",new {controller="Users", id = createdUser.Id},userToReturn);
            
            //parameter, object routevalues
            //3rd parameter is what we send back
            //var userToReturn .. return this
        
           

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {

            // now we set up global exception handler
            var userFromRepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();//do not give the hint that the username exists or password exists
            }

            var claims = new[]
            {

            new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()), //userId
            new Claim(ClaimTypes.Name,userFromRepo.Username) //username
        };

            //using ms identitymodeltokens
            //import to byte array
            //security key uses this
            //encoding utf8 
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //in appsettings.json have a 12 or >12 char key
            //this should not leak , as then attacker can generate n number of tokens

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //encrypt key with hashing algorithm

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDTO>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });







        }
    }
}
//JSON WEB TOKEN IS A SELF-CONTAINED PIECE OF DATA
//INDUSTRY STANDARD FOR TOKENS RFC 7519
//CAN CONTAIN
/*
1. CREDENTIALS
2. CLAIMS
3. OTHER INFORMATION


server does not need to connect to db
without calling datastore, we can validate data

//structure


//HEADER
{
    "alg" : "HS512",
    "typ" : "JWT"
}


//PAYLOAD
{
    "nameid" : "8",
    "unique_name" : "umar",
    "nbf" :1511110407,
    "exp" : 151196807,
    "iat" :1511110407
}

decoded / decrypted by anyone
nbf = not before (earliest stage of token usage)
iat = issued at (timestamp of when token was issued)\
expiry date = token issue expiry

third part is SECRET

HMACSHA256(
    base64UrlEncode(header) + "." + base64UrlEncode(payload)+,secret 
base64 encoded
)


secret stored on server
client sends token on server
server validates token

secret mismatch ... error ... Invalid
HEADER->PAYLOAD->SIGNATURE

only signature can be decoded by server

1. user sends hashed username password
2. server validates and sends back JWT
3. client sends JWT for further request ( every single seperate request needs a valid token )
4. server validates jwt and sends back response
5. jwt stored locally on client

*/
