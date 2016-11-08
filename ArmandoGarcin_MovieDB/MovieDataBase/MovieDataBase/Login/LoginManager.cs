using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieDataBase.Login
{
    public class LoginManager
    {

        //instance of the manager
        private static LoginManager pInstance;
        
        private LoginData loginData;
        private MovieData.MovieDBEntities1 userDB;

        private LoginManager()
        {
            //If the loginData is null, we have no one logged in
            loginData = null;
            //Create our link to the DB
            userDB = new MovieData.MovieDBEntities1();
        }

        private static LoginManager GetInstance()
        {
            if(pInstance == null)
            {
                pInstance = new LoginManager();
            }
            return pInstance;
        }

        public static bool Login(string _username, string _email, string _password)
        {
            //Find the user in the DB
            MovieData.User tUser = GetInstance().userDB.Users.Find(_email);

            if(tUser != null)
            {
                if(tUser.Password.Equals(_password))
                {
                    GetInstance().loginData = new LoginData(tUser.Username, tUser.Email);
                    //return success
                    return true;
                }
            }

            //In case everything went wrong
            return false;
        }

        public static bool Logout()
        {
            if(IsLoggedIn())
            {
                //Sign the user out
                GetInstance().loginData = null;
                //return success
                return true;
            }
            //In case we tried to log out when we weren't even logged in
            return false;
        }

        public static bool CreateUser(string _username, string _email, string _password)
        {
            if(_email != "")
            {
                //Our new user
                MovieData.User tUser = new MovieData.User(_email, _username, _password);

                //Add to DB
                GetInstance().userDB.Users.Add(tUser);

                //commit changes
                GetInstance().userDB.SaveChanges();

                Login(tUser.Username, tUser.Email, tUser.Password);

                return true;
            }

            //In case everything goes to hell
            return false;
        }

        public static LoginData GetCurrentUser()
        {
            return GetInstance().loginData;
        }

        public static bool IsLoggedIn()
        {
            return (GetInstance().loginData != null);
        }
    }
}