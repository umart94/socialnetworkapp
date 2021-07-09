using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.API.Models;

namespace SocialApp.API.Data
{
    //Responsible for querying database
    public class AuthRepository : IAuthRepository
    {
        //Using Repository Pattern
        /*
        REPOSITORY PATTERN ( LEVEL OF ABSTRACTION -> ENTITY FRAMEWORK -> TRANSLATES INTO SOMETHING SQL CAN UNDERSTAND

WE NEED TO ADD A FURTHER LAYER OF ABSTRACTION

REPOSITORY MEDIATES between data source layer and business layer of application

it queries the data source for data
maps the data from the data source to a business entity

persists changes in business entity to a data source

it seperates business logic from interactions with underlying data source or web service


DATABASE = WAREHOUSE
CONTROLLER VIA ENTITY FRAMEWORK GETS DATA FROM DATABASE

CONTROLLER TIGHT TO WAREHOUSE
IF WE MOVE DATA TO WAREHOUSE
CONTROLLER NEEDS TO KNOW ABOUT CHANGES


IF WE ADD ADDITIONAL CONTROLLERS THEY ALSO NEED TO KNOW ABOUT DB WAREHOUSES



----------

CONTROLLER -> REPOSITORY INTERFACE middle layer -> DB/CLOUD/DISK/MEMORY

GETUSERS
GETUSER(INT ID)
LOGIN(USER)


REPOSITORY WILL EXPOSE THESE METHODS

CONTROLLER WILL MAKE USE OF REPOSITORY INTERFACE

REPOSITORY WILL IN TURN CONTACT DB WAREHOUSE


this pattern minimizes duplicate query logic
it decouples application from persistence framework
all db queries in same place
promotes testability
*/
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            this._context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
            //photos are inside collection - Users
            //we need to return them as .Include( p=> p.Photos) 

            //not found null
            //found return username

            if (user == null)
            {
                return null; //401 unauthorized
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;//401 unauthorized
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {


                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        //User entity and user password(not in cleartext) and register our user in db
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //pass by reference, not as values
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //now we will add these to our database (once only)
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();//commit to database
            return user;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            //use System.Security.Cryptography
            //Hash Based Message Authentication Code
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                //provides with randomly generated key, so give it to salt
                //var hmac = new System.Security.Cryptography.HMACSHA512();

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
            //new instance of hmacsha512
            //gives us a randomly generated key
            //use it to unlock our password(salt)
            //compute hash of entered password vs stored hash+salt

            //HMAC -> keyedhashalgorithm -> hashalgorithm -> Idisposable Icryptotransform interface
            //idisposable provides dispose method
            //surround new instance with a using statement.... so that dispose is properly called 


        }
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
            {
                return true;

            }

            return false;
        }
    }
}