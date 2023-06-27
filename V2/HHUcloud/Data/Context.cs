using HHUcloud.Model;
using Microsoft.EntityFrameworkCore;

namespace HHUcloud.Data;

public class Context : DbContext
{

    public DbSet<User>? User { get; set; }
    public DbSet<Song>? Song { get; set; }
    public DbSet<Artist>? Artist { get; set; }
    public DbSet<ArtistHasSong>? ArtistHasSong { get; set; }
    public DbSet<ArtistHasAlbum>? ArtistHasAlbums { get; set; }
    public DbSet<Album>? Album { get; set; }
    public DbSet<AlbumHasSong>? AlbumHasSong { get; set; }
    public DbSet<Playlist>? Playlist { get; set; }
    public DbSet<PlayRecord>? PlayRecord { get; set; }
    public DbSet<PlaylistHasSong>? PlaylistHasSong { get; set; }
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
