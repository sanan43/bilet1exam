using bilet1exam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bilet1exam.DAL
{
    public class AppDbContext:IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options ):base(options) 
        {
            
        }
        public DbSet<Post> Posts { get; set; }
    }
}
