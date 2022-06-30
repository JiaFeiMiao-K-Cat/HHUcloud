using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMO_Cloud.Models;

namespace EMO_Cloud.Data
{
    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<User>? User { get; set; }
        public DbSet<Song>? Song { get; set; }
        public DbSet<Artist>? Artist { get; set; }
        public DbSet<Album>? Album { get; set; }
        public DbSet<PlayList>? PlayList { get; set; }
        public DbSet<PlayRecord>? PlayRecord { get; set; }
        public DbSet<ArtistHasSongs>? ArtistHasSongs { get; set; }
        public DbSet<AlbumHasSongs>? AlbumHasSongs { get; set; }
        public DbSet<PlayListHasSongs>? PlayListHasSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(b => b.Email);
            modelBuilder.Entity<Song>().ToTable("SONG");
            modelBuilder.Entity<Artist>().ToTable("ARTIST");
            modelBuilder.Entity<PlayList>().ToTable("PLAYLIST");
            modelBuilder.Entity<PlayRecord>().ToTable("PLAYRECORD");
            modelBuilder.Entity<AlbumHasSongs>().ToTable("ALBUMHASSONGS");
            modelBuilder.Entity<ArtistHasSongs>().ToTable("ARTISTHASSONGS");
            modelBuilder.Entity<PlayListHasSongs>().ToTable("PLAYLISTHASSONGS");
        }
    }
}
