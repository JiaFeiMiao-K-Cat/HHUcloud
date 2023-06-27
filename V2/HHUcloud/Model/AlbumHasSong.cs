using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("ALBUM_HAS_SONG"), Comment("专辑的歌曲列表")]
[PrimaryKey(nameof(AlbumId), nameof(SongId))]
public class AlbumHasSong
{
    [Comment("专辑ID"), ForeignKey(nameof(Album.AlbumId))]
    public long AlbumId { get; set; }

    [Comment("歌曲ID"), ForeignKey(nameof(Song.SongId))]
    public long SongId { get; set; }

}
