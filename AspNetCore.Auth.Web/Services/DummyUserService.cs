﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Auth.Web.Services
{
    public class DummyUserService : IUserService
    {
        private IDictionary<string, (string PasswordHash, User User)> _users = new Dictionary<string, (string PasswordHash, User User)>();
        public DummyUserService(IDictionary<string, string> users)
        {
            foreach (var user in users)
            {
                _users.Add(user.Key.ToLower(), (BCrypt.Net.BCrypt.HashPassword(user.Value), new User(user.Key)));
            }
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="passsword">passsword</param>
        /// <returns>bool</returns>
        public Task<bool> AddUser(string username, string passsword)
        {
            if (_users.ContainsKey(username.ToLower()))
            {
                return Task.FromResult(false);
            }
            _users.Add(username.ToLower(), (BCrypt.Net.BCrypt.HashPassword(passsword), new User(username)));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate user credentials 
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="user">user</param>
        /// <returns>bool</returns>
        public Task<bool> ValidateCredentials(string username, string password, out User user)
        {
            user = null;
            var key = username.ToLower();
            if (_users.ContainsKey(key))
            {
                var hash = _users[key].PasswordHash;
                if (BCrypt.Net.BCrypt.Verify(password, hash))
                {
                    user = _users[key].User;
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}
