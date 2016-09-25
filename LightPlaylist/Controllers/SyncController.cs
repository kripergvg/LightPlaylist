using System;
using System.Linq;
using System.Web.Http;
using LightPlaylist.Models;

namespace LightPlaylist.Controllers
{
    public class SyncController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Songs/
        public IQueryable<Song> GetSongs(DateTime date)
        {
            var songs = db.Songs.Where(s => s.AddedDate > date);

            return songs;
        }

        public IQueryable<Song> GetSongs()
        {
            var songs = db.Songs.AsQueryable();

            return songs;
        }
    }
}
