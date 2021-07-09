using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.API.Models;
using System.Linq;
using SocialApp.API.Helpers;
using System;

namespace SocialApp.API.Data
{
    public class SocialRepository : ISocialRepository
    {
        private readonly DataContext _context;
        public SocialRepository(DataContext context)
        {

            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
            //throw new System.NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
            //throw new System.NotImplementedException();
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            //FirstOrDefaultAsync -> if the like does not exist then it will return null otherwise it returns Like
           return await _context.Likes.FirstOrDefaultAsync( u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId)//manually add System.Linq import due to Where()
                 .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            //throw new System.NotImplementedException();
            return user;
        }


        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //The line with the error ... remove the async and await from here... and instead return it through method.
           // var users = await _context.Users.Include(p => p.Photos);//.ToListAsync();
            //
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where( u => u.Id != userParams.UserId);
            users = users.Where( u => u.Gender == userParams.Gender);

            if(userParams.Likers){
                var userLikers = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where( u => userLikers.Contains(u.Id));
                //userLikers matches any userId inside table, then we will return only these.. by contains u.Id

            }
            if(userParams.Likees){
                var userLikees = await GetUserLikes(userParams.UserId,userParams.Likers);
                 users = users.Where( u => userLikees.Contains(u.Id));
            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 96){
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u=>u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            if(!string.IsNullOrEmpty(userParams.orderBy)){
                switch(userParams.orderBy){
                    case "created":
                      users = users.OrderByDescending(u => u.Created);
                      break;
                      default:
                      users = users.OrderByDescending(u => u.LastActive);
                      break;

                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id,bool likers){
                //get logged in user that has Likers and Likees
                var user = await _context.Users
                .Include(x => x.Likers)
                .Include( x=> x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

                if(likers){
                    //users that have liked the current user
                    return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
                } else {
                    return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
                }

        }

        public async Task<bool> SaveAll()
        {
            //throw new System.NotImplementedException();
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m=> m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
            .Include(u => u.Sender)
            .ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p=>p.Photos)
            .AsQueryable();

            //filter out messages
            switch(messageParams.MessageContainer){
                    case "Inbox":
                        messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                        break;
                    case "Outbox":
                        messages = messages.Where( u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                        break;
                    default:
                        messages = messages.Where(u => u.RecipientId == messageParams.UserId 
                        && u.RecipientDeleted == false && u.isRead == false);
                        break;

            }

            //add ordering of messages
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {   
            //conversation between 2 users
            var messages = await _context.Messages
            .Include(u => u.Sender)
            .ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p=>p.Photos)
            .Where(m => m.RecipientId == userId && m.RecipientDeleted ==false && m.SenderId == recipientId
            
            
            || m.RecipientId == recipientId &&

            m.SenderDeleted==false&&
            m.SenderId == userId)
            .OrderByDescending(m => m.MessageSent)
            .ToListAsync();

            return messages;
            
            //return messages where RecipientId matches userId
            //SenderId matches RecipientId
            

        }

        
    }
}