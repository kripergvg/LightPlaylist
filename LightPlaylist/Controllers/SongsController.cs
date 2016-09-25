using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LightPlaylist.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model.Attachments;

namespace LightPlaylist.Controllers
{
    public class SongsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string _password = "ab2481632AA";

        // GET: api/Songs
        public IQueryable<Song> GetSongs()
        {
            return db.Songs;
        }

        // GET: api/Songs/5
        [ResponseType(typeof(Song))]
        public IHttpActionResult GetSong(int id)
        {
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }


        // POST: api/Songs
        [ResponseType(typeof(Song))]
        public IHttpActionResult PostSong(NewSongModel song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSong = new Song
            {
                AddedDate = DateTime.UtcNow,
                Key = song.Key
            };
            db.Songs.Add(newSong);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = newSong.SongId }, song);
        }

        // DELETE: api/Songs/5
        [ResponseType(typeof(Song))]
        [HttpDelete]
        public IHttpActionResult DeleteSong(string vkId)
        {
            Song song = db.Songs.FirstOrDefault(s => s.Key == vkId);
            if (song == null)
            {
                return NotFound();
            }

            db.Songs.Remove(song);
            db.SaveChanges();

            return Ok(song);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SongExists(int id)
        {
            return db.Songs.Count(e => e.SongId == id) > 0;
        }
    }
}