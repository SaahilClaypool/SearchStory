using System;

namespace SearchStory.App.Services
{
    public class LoginService
    {
        StateContainer State { get; set; } = null!;
        public bool RequiresPassword => !(PassPhrase is null or "");
        public LoginService(StateContainer container)
        {
            State = container;
            State.Update(() => State.Value.LoggedIn = PassPhrase is null or "");
        }
        private static string? PassPhrase  => Environment.GetEnvironmentVariable("PASSWORD");

        public bool LogIn(string password)
        {
            if (RequiresPassword || password == PassPhrase)
            {
                State.Update(() => State.Value.LoggedIn = true, "LoggedIn");
                return true;
            }
            return false;
        }
        public void LogOut() => State.Update(() => State.Value.LoggedIn = false, "LoggedIn");
    }
    
    class Unauthorized : Exception { } 
}