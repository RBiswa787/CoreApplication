using System;
using MongoDB.Bson;
using MongoDB.Driver;
using CoreApplication.Model;
using System.Threading.Tasks;
using CoreApplication.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using CoreApplication.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using Microsoft.AspNetCore.Routing;
using CoreApplication.Helpers;
using System.Security.Claims;
using System.Text;


namespace CoreApplication.Data
{
    using BCrypt.Net;
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context = null;
        private IOptions<Configuration.DatabaseSettings> _settings;
        public UserRepository(IOptions<Configuration.DatabaseSettings> settings)
        {
            _context = new DbContext(settings);
            _settings = settings;
        }

 

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _context.Users.Find(x => x.Username == username)
               .FirstOrDefaultAsync();

            if (user == null)
                return null;

            if (!BCrypt.Verify(password, user.Password))
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
           
            var key = Encoding.ASCII.GetBytes(_settings.Value.Secret);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, user.Role)
        };
            var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = signingCredentials
            };

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Role, user.Role)
            //    }),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials = new SigningCredentials(
            //        new SymmetricSecurityKey(key),
            //        SecurityAlgorithms.HmacSha256)
            //};
            var token = tokenHandler.CreateToken(accessTokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = null;

            return user;
        }

        public async Task<User> Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Find(x => x.Username == user.Username).Any())
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            //string passwordHash = SecurePasswordHash.Hash(password);
            string passwordHash = BCrypt.HashPassword(password);
            user.Password = passwordHash;

            if(user.Role != "warehouse_manager")
                user.Verified = false;
            else
                user.Verified = true;

            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {

            try
            {
                return await _context.Users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<IEnumerable<string>> GetUnverifiedUsers()
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Verified, false);
                var projection = Builders<User>.Projection.Include(u => u.Username);
                var cursor = await _context.Users
                                            .Find(filter)
                                            .Project(projection)
                                            .ToListAsync();
                return cursor.Select(bson => bson.GetValue("Username").AsString);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> OnboardUser(string username)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var update = Builders<User>.Update.Set(u => u.Verified, true);
            var result = await _context.Users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
