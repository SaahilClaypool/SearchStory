using Microsoft.AspNetCore.Mvc;

namespace SearchStory.App.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // /api/User/GetUser
        [HttpGet("[action]")]
        public UserModel GetUser()
        {
            // Instantiate a UserModel
            var userModel = new UserModel
            {
                UserName = "[]",
                IsAuthenticated = false
            };
            System.Console.WriteLine(User.ToJson());
            // Detect if the user is authenticated
            if (User?.Identity?.IsAuthenticated == true)
            {
                // Set the username of the authenticated user
                userModel.UserName = User.Identity.Name!;
                userModel.IsAuthenticated = User.Identity.IsAuthenticated;
            };
            
            return userModel;
        }
    }
    public class UserModel
    {
        public string UserName { get; set; } = "";
        public bool IsAuthenticated { get; set; }
    }
}