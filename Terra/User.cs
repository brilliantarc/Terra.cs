﻿using System.Collections.Generic;

namespace Terra
{
    /// <summary>
    /// A user account on the Terra server.
    /// </summary>
    public class User
    {
        /// <summary>
        /// A user's account login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// A user's email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The roles for the user.  We currently only support two in Terra:
        /// "Super" and "Administrator".  An "Administrator" may create and
        /// update user information in an operating company, while a "Super"
        /// may do this system-wide.
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// This is never returned by the Terra server, but it can be used to
        /// update user information via the API.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// This is never returned by the Terra server, but it can be used to
        /// update user information via the API.
        /// </summary>
        public string PasswordConfirmation { get; set; }

        /// <summary>
        /// Has this account been disabled?  If so, the user will not be 
        /// allowed to authenticate and use Terra.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// User credentials are generated by the server on login, and only
        /// available to the user himself.  They are used for subsequent calls
        /// to the Terra API, to reduce the password being passed around too
        /// frequently.
        /// 
        /// A future release will likely change the credentials from their
        /// random nature to an encrypted Hmac value, similar to Amazon.
        /// </summary>
        public string UserCredentials { get; set; }

        public override int GetHashCode()
        {
            return ("user:" + Login).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            User u = obj as User;
            if ((object)u == null)
            {
                return false;
            }

            return (u.Login == Login);
        }
    }
}