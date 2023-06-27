using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("ARTIST_HAS_SONG"), Comment("歌手的歌曲列表")]
[PrimaryKey(nameof(ArtistId), nameof(SongId))]
public class ArtistHasSong
{
    [Comment("歌手ID"), ForeignKey(nameof(Artist.ArtistId))]
    public long ArtistId { get; set; }

    [Comment("歌曲ID"), ForeignKey(nameof(Song.SongId))]
    public long SongId { get; set; }
}
