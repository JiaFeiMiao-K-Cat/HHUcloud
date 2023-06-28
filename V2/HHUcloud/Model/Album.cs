using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("ALBUM"), Comment("专辑列表")]
public class Album
{
    [Key, Comment("专辑ID")]
    public long AlbumId { get; set; }

    [Comment("标题")]
    public string Title { get; set; }

    [Comment("发行时间")]
    public DateTime Published { get; set; }

    [Comment("封面链接")]
    public string CoverLink { get; set; }

    [NotMapped]
    public List<long>? ArtistIds { get; set; }

    [NotMapped]
    public List<string>? ArtistNames { get; set; }

    [NotMapped]
    public List<Song>? Songs { get; set; }
}
