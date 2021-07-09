using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SocialApp.API.Data;
using System.Threading.Tasks;
using SocialApp.API.DTOS;
using AutoMapper;
using System.Collections.Generic;
using System.Collections;

using System.Security.Claims;//dont use identity models claims class
using SocialApp.API.Helpers;
using SocialApp.API.Models;

namespace SocialApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // action filter, --- last active property
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISocialRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(ISocialRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers( [FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            //Response
            Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(usersToReturn);//pass information back to client in the header with pagination
        }

        [HttpGet("{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(userToReturn);
        }
        /*
                        The HTTP 401 Unauthorized client error status response code indicates that the request has not been applied because it lacks valid authentication credentials for the target resource.

        This status is sent with a WWW-Authenticate header that contains information on how to authorize correctly.

        This status is similar to 403, but in this case, authentication is possible.
        */

        /*
                       The HTTP 204 No Content success status response code indicates that the request has succeeded, but that the client doesn't need to go away from its current page. A 204 response is cacheable by default. An ETag header is included in such a response.

       The common use case is to return 204 as a result of a PUT request, updating a resource, without changing the current content of the page displayed to the user. If the resource is created, 201 Created is returned instead. If the page should be changed to the newly updated page, the 200 should be used instead.
       */
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO userForUpdateDTO)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }
            var userFromRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdateDTO, userFromRepo);

            if (await _repo.SaveAll())
            {
                return NoContent();//204 No Content  
                                   //it should return 204 only when the user exists and the token is valid and you update the user info

            }
            throw new System.Exception("$Updating user {id} failed on saving, Please Try Again");
        }

        [HttpPost("{id}/like/{recipientId}")]

        public async Task<IActionResult> LikeUser(int id,int recipientId){
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }
                //like object or null
                //if there is a like object -> return BadRequest
                //if its null , create a new Like , add them to repo , pass them in like entity , save them in repo
            var like = await _repo.GetLike(id,recipientId);
            if(like != null){
                return BadRequest("You already like this user");
            }

            if(await _repo.GetUser(recipientId) == null){
                return NotFound();
            }

            like = new Like {
                    LikerId = id,
                    LikeeId = recipientId
            };

            //this is a synchronous method
            //its saving the like in memory

            _repo.Add<Like>(like); 
            if(await _repo.SaveAll()){
                return Ok();
            }
            return BadRequest("Failed to Like User");

        }






    }
}