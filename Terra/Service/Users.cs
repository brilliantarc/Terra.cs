using System;
using System.Collections.Generic;
using RestSharp;

namespace Terra.Service
{
    /// <summary>
    /// Manage user accounts.  You may also use this service to interact with
    /// users.
    /// <para>
    /// Do not create an instance of this directly.  Instead, call in through 
    /// the Terra.Client.Users.
    /// </para>
    /// </summary>
    /// <seealso cref="Terra.Client.Users"/>
    public class Users
    {
        private Client _client;

        public Users(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Return the list of all the user accounts in the Terra server.
        /// </summary>
        /// <returns>A list of users</returns>
        public List<User> All()
        {
            return _client.Request("users").MakeRequest<List<User>>();
        }

        /// <summary>
        /// Look up a user's account information, including email address.  Details
        /// on a user may be limited based on your own account level.  
        /// Administrators receive more information about a user than general
        /// users may request.
        /// </summary>
        /// <param name="login">The user's account login</param>
        /// <returns>Details about a user</returns>
        public User Get(string login)
        {
            return _client.Request("user").AddParameter("login", login).MakeRequest<User>();
        }

        /// <summary>
        /// Create a new user account.  All fields except the opco are required.
        /// The user's email must be unique in the system.
        /// <para>
        /// If the opco is indicated, the user will be associated with the 
        /// operating company.  This will limit the user's write-access to that
        /// portfolio.
        /// </para>
        /// <para>
        /// If a user does not belong to a portfolio, and is not a "Super" user,
        /// he or she will not have access to write any information.
        /// </para>
        /// </summary>
        /// <param name="login">The user's login; may not contain spaces or non-ASCII characters</param>
        /// <param name="email">The user's email address; forgotten passwords and messages will be forwarded to this email</param>
        /// <param name="password">A password for the user</param>
        /// <param name="passwordConfirmation">Confirm the password</param>
        /// <param name="opco">The three or four letter code of the portfolio to which to associate this user (optional)</param>
        /// <returns>The newly created user account</returns>
        /// <exception cref="Terra.ServerException">If any of the required information is missing or incorrect</exception>
        public User Create(string login, string email, string password, string passwordConfirmation, string opco = null)
        {
            return _client.Request("user", Method.POST).
                AddParameter("login", login).
                AddParameter("password", password).
                AddParameter("confirmation", passwordConfirmation).
                AddParameter("email", email).
                AddParameter("opco", opco).
                MakeRequest<User>();
        }

        /// <summary>
        /// Update a user's email address.  A quick way to update a user's email.
        /// Must be an administrator.
        /// </summary>
        /// <param name="login">The existing user login</param>
        /// <param name="email">The new email address</param>
        /// <returns>The updated user information</returns>
        /// <exception cref="Terra.ServerException">The user's login does not exist, or the email is already in use</exception>
        public User UpdateEmail(string login, string email)
        {
            return _client.Request("user/email", Method.PUT).AddParameter("login", login).AddParameter("email", email).MakeRequest<User>();
        }

        /// <summary>
        /// Update a user's password.  A quick way to update a user's password.
        /// </summary>
        /// <param name="login">The existing user login</param>
        /// <param name="original">The original password</param>
        /// <param name="newPassword">The new password</param>
        /// <param name="newPasswordConfirmation">The new password confirmation</param>
        /// <returns>The updated user information</returns>
        /// <exception cref="Terra.ServerException">The user does not exist, or the password is incorrect</exception>
        public User UpdatePassword(string login, string original, string newPassword, string newPasswordConfirmation)
        {
            return _client.Request("user/password", Method.PUT).
                AddParameter("login", login).
                AddParameter("original", original).
                AddParameter("password", newPassword).
                AddParameter("confirmation", newPasswordConfirmation).
                MakeRequest<User>();
        }

        /// <summary>
        /// Update a user's account, email and password simultaneously, 
        /// typically by an admin.
        /// <para>
        /// A user may not change his or her login.
        /// </para>
        /// </summary>
        /// <param name="user">The updated user information</param>
        /// <returns>The updated user information</returns>
        /// <exception cref="Terra.ServerException">The user does not exist, or some of the updated information is incorrect</exception>
        public User Update(User user)
        {
            return _client.Request("user", Method.PUT).
                AddParameter("login", user.Login).
                AddParameter("email", user.Email).
                AddParameter("password", user.Password).
                AddParameter("confirmation", user.PasswordConfirmation).
                MakeRequest<User>();
        }

        /// <summary>
        /// Add the user to the given role.  A user may only promote a user 
        /// to the same level has they are.  For example, an "Administrator"
        /// may promote a user to "Administrator", but not "Super".
        /// </summary>
        /// <param name="user">The user account</param>
        /// <param name="role">The role, either "Administrator" or "Super"</param>
        /// <exception cref="Terra.ServerException">The user does not exist, or the role is incorrect</exception>
        public void AddRole(User user, string role)
        {
            _client.Request("role/user", Method.PUT).
                AddParameter("role", role).
                AddParameter("login", user.Login).
                MakeRequest();
        }

        /// <summary>
        /// Remove the user from the given role.  A user may only remove other
        /// users from roles they themselves belong to.  For example, an 
        /// "Administrator" may remove a user from the "Administrator" role,
        /// but not from the "Super" role.
        /// </summary>
        /// <param name="user">The user to modify</param>
        /// <param name="role">The role to add the user to, either "Administrator" or "Super"</param>
        /// <exception cref="Terra.ServerException">The user does not exist, or the role is incorrect</exception>
        public void RemoveRole(User user, string role)
        {
            _client.Request("role/user", Method.DELETE).
                AddParameter("role", role).
                AddParameter("login", user.Login).
                MakeRequest();
        }

        /// <summary>
        /// Enable the user's account, i.e. allow him or her to login to Terra.
        /// </summary>
        /// <param name="user">The user to enable</param>
        /// <returns>The updated user account information</returns>
        /// <exception cref="Terra.ServerException">The user does not exist</exception>
        public User Enable(User user)
        {
            return _client.Request("user/enable", Method.PUT).AddParameter("login", user.Login).MakeRequest<User>();
        }

        /// <summary>
        /// Disable the user's account, i.e. preventing him or her from logging
        /// into Terra.
        /// </summary>
        /// <param name="user">The user to disable</param>
        /// <returns>The updated user account information</returns>
        /// <exception cref="Terra.ServerException">The user does not exist</exception>
        public User Disable(User user)
        {
            return _client.Request("user/disable", Method.PUT).AddParameter("login", user.Login).MakeRequest<User>();
        }

        /// <summary>
        /// Send an email message to the given user.
        /// </summary>
        /// <param name="to">The user to send the email to</param>
        /// <param name="message">A brief message to the user</param>
        /// <param name="subject">The subject of the message; optional</param>
        /// <exception cref="Terra.ServerException">The user does not exist</exception>
        public void SendMessage(User to, string message, string subject = null)
        {
            _client.Request("user/message", Method.POST).
                AddParameter("to", to.Login).
                AddParameter("subject", subject).
                AddParameter("message", message).
                MakeRequest();
        }

        /// <summary>
        /// Looks for an email address in Terra and send the user information
        /// on updating his or her password.  The user will be emailed a link
        /// to the Terra client application's "Update Password" screen, just
        /// as if they had clicked on "Forgot Password" in Terra.
        /// </summary>
        /// <param name="email">The user's email address</param>
        public void ForgotPassword(string email)
        {
            _client.Request("user/forgot", Method.PUT).AddParameter("email", email).MakeRequest();
        }

        /// <summary>
        /// Return the history of changes made to the ontology by the given user.
        /// </summary>
        /// <param name="user">The user account</param>
        /// <param name="from">Starting point of results to return; defaults to 0</param>
        /// <param name="max">The maximum number of results to return; defaults to 100</param>
        /// <returns>A list of changes made to the ontology</returns>
        /// <exception cref="Terra.ServerException">The user does not exist</exception>
        public List<String> History(User user, int from = 0, int max = 100)
        {
            return _client.Request("user/history").
                AddParameter("login", user.Login).
                AddParameter("from", from).
                AddParameter("max", max).
                MakeRequest<List<String>>();
        }
    }
}
