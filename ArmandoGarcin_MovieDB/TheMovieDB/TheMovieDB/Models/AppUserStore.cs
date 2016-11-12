using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheMovieDB.MovieData;

namespace TheMovieDB.Models
{
    public class AppUserStore : UserStore<AppUser>
    {

        public AppUserStore(IdentityDBContext _context) : base(_context)
        {

        }

    }
}