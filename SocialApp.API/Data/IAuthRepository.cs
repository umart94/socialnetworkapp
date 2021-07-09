using System.Threading.Tasks;
using SocialApp.API.Models;

namespace SocialApp.API.Data
{
    public interface IAuthRepository
    {
        //interfaces start with a capital I in name
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);

    }
}

/*
USER SHOULD BE ABLE TO LOG IN
USER SHOULD BE ABLE TO REGISTER
USER SHOULD BE ABLE TO SEE A LIST OF USERS
USER SHOULD BE ABLE TO MESSAGE ANOTHER USER



RISKY STEP - STORE PASSWORDS IN DATABASE

Create user model
use repository pattern
authentication controller
data transfer objects
token authentication

authentication middleware

after this we cannot use simple http request
the get request must and should give us 401 unauthorized

Bearer


STEP-1
STORE PASSWORDS

HASHING A PASSWORD
ONE WAY PROCESS

PASSWORD
.....
SHA512 ONE WAY HASH
.....
wrong ... two same passwords or n time same password will give same n time hash INSECURE
pre-computed rainbow tables


Add Salt with hash
*/
