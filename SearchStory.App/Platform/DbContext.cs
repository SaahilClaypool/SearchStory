using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SearchStory.App.Platform
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {  }
        
        public Task<IdentityUser?> FindUser(string username) => this.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
    }
}