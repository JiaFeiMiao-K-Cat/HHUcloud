using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("ARTIST"), Comment("歌手列表")]
public class Artist
{
    [Key, Comment("歌手ID")]
    public long ArtistId { get; set; }

    [Comment("歌手名")]
    public string Name { get; set; }

    [Comment("简介")]
    public string? Description { get; set; }

    [NotMapped]
    public List<long>? AlbumIds { get; set; }

    [NotMapped]
    public List<string>? AlbumTitles { get; set; }
}
