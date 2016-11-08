using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDataBase.Login
{
    public class LoginData
    {
        //Username 
        private string username;
        //The email of the user
        private string email;
        //We will NOT store the password, if we need it, we'll ask for it

        public LoginData(string _username, string _email)
        {
            username = _username;
            email = _email;
        }

        public string GetUsername()
        {
            return username;
        }

        public string GetEmail()
        {
            return email;
        }

    }
}