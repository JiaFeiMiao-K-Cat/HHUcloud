using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HHUcloud.Model;

[Table("SONG"), Comment("歌曲列表")]
public class Song
{
    [Key, Comment("歌曲Id")]
    public long SongId { get; set; }

    [Comment("点击数")]
    public int Count { get; set; }

    [Comment("歌曲名")]
    public string Title { get; set; }

    [Comment("时长")]
    public string Duration { get; set; }

    [Comment("专辑Id")]
    public long AlbumId { get; set; }

    [NotMapped]
    public string? CoverLink { get; set; }

    [NotMapped]
    public List<long>? ArtistIds { get; set; }

    [NotMapped]
    public List<string>? ArtistNames { get; set; }
}