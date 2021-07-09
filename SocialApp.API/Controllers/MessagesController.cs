using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialApp.API.Data;
using SocialApp.API.Helpers;
using SocialApp.API.DTOS;
using SocialApp.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SocialApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // action filter, --- last active property
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ISocialRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(ISocialRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]

        public async Task<IActionResult> GetMessage(int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo ==null){
                return NotFound();
            }

            return Ok(messageFromRepo);


        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId,[FromQuery]MessageParams messageParams){
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }
            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            //return pagination / response
            var messages = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrentPage,messagesFromRepo.PageSize,messagesFromRepo.TotalCount,messagesFromRepo.TotalPages);

            return Ok(messages);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDTO messageForCreationDto){

            //we were using a userId here, but it was just getting the default userId of loggedin user
            //we wanted the sender Id here. so
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }

            messageForCreationDto.SenderId = userId;
            //the values for messageForCreation are in memory due to AutoMapper
            //below, in var messageToReturn, we changed MessageForCreationDto to MessageToReturnDTO
            //this gives us all details of the user, AutoMapper maps this 
            // and then we return it to messageToReturn
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null){
                return BadRequest("Could Not Find User");
            }

            var message = _mapper.Map<Message>(messageForCreationDto);



            _repo.Add(message);
            //map to dto to return it
            //using reverse map

                                    //message for creation dto sends less amount of info back
                                    //message for return dto sends all info , with photos of users back
            

            if(await _repo.SaveAll()){
                var messageToReturn = _mapper.Map<MessageToReturnDTO>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating the message failed on save");

        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId,int recipientId){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();//401 ... or 403 ... but we return 401
                //if you are not sending token of current user, it shoudl return 401
                //if you send http://localhost:5000/api/users/10000
                //and user does not exist it should return 401


            }

            var messageFromRepo = await _repo.GetMessageThread(userId,recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messageFromRepo);
            return Ok(messageThread);


        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id,int userId){

                if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    return Unauthorized();

                    var messageFromRepo = await _repo.GetMessage(id);

                    if(messageFromRepo.SenderId == userId){
                        messageFromRepo.SenderDeleted = true;
                    }

                    if(messageFromRepo.RecipientId == userId){
                        messageFromRepo.RecipientDeleted = true;
                    }

                    if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted){
                        _repo.Delete(messageFromRepo);
                    }

                    if(await _repo.SaveAll()){
                        return NoContent();
                    }
                    
                    throw new Exception("Error Deleting The Message ");

        }

        [HttpPost("{id}/read")]

        public async Task<IActionResult> MarkMessageAsRead(int userId, int id){

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();

            }

            var message = await _repo.GetMessage(id);

            if(message.RecipientId != userId){
                return Unauthorized();
            }
            message.isRead = true;
            message.DateRead = DateTime.Now;

            await _repo.SaveAll();

            return NoContent();
        }

    }
}