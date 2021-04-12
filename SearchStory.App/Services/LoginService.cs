using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SearchStory.App.Platform;

namespace SearchStory.App.Services
{
    public class LoginService
    {
        private readonly SignInManager<IdentityUser> signInManager;

        StateContainer State { get; set; } = null!;
        public AppDbContext Db { get; }
        readonly UserManager<IdentityUser> userManager = null!; 

        public async Task<bool> RequiresPassword() => await Db.Users.AnyAsync();
        public LoginService(StateContainer container, SignInManager<IdentityUser> signInManager, AppDbContext db, UserManager<IdentityUser> userManager)
        {
            State = container;
            this.signInManager = signInManager;
            Db = db;
            this.userManager = userManager;
        }

        public async Task<bool> LogIn(string username, string password)
        {
            var user = await Db.FindUser(username);
            if (user is null) return false;

            var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                State.Update(() => 
                { 
                    State.Value.LoggedIn = true;
                    State.Value.User = user;
                }, "LoggedIn");
                return  true;
            }
            return false;
        }
        public void LogOut() => State.Update(() => State.Value.LoggedIn = false, "LoggedIn");
        
        
        private record SeedUser(string Username, string Password);
        public void SeedUsers()
        {
            string filename = "./user_seed.jsonl";
            if (!File.Exists(filename)) return;
            
            var users = File.ReadAllLines(filename).Select(JsonExt.FromJson<SeedUser>);
            
            foreach(var user in users)
            {
                if (user is null) continue;
                if(!Db.Users.Where(u => u.UserName == user.Username).Any())
                {
                    userManager.CreateAsync(new() { UserName = user.Username }, user.Password).Wait();
                }
                else
                {
                    System.Console.WriteLine($"User {user.Username} already exists");
                }
            }
            
            if (!Db.Users.Any())
            {
                // we can make a single admin user... only for local
                userManager.CreateAsync(new() { UserName = "" }, "").Wait();
            }
        }
    }

    class Unauthorized : Exception { }
}