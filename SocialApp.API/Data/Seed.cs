using System.Collections.Generic;
using Newtonsoft.Json;
using SocialApp.API.Models;

namespace SocialApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
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
        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");

            //serialize JSON
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();
                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }
    }
}