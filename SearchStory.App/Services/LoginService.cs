using System;

namespace SearchStory.App.Services
{
    public class LoginService
    {
        private static string? PassPhrase  => Environment.GetEnvironmentVariable("PASSWORD");
        public bool Status { get; private set; } = PassPhrase is null or "";

        public void LogIn(string password)
        {
            if (PassPhrase is null or "")
            {
                Status = true;
            }
            else
            {
                Status = PassPhrase == password;
            }
        }
        public void LogOut() => Status = false;
    }
}