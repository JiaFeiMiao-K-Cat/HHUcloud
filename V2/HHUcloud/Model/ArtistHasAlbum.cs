using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("ARTIST_HAS_ALBUM"), Comment("歌手的专辑列表")]
[PrimaryKey(nameof(ArtistId), nameof(AlbumId))]
public class ArtistHasAlbum
{
    [Comment("歌手ID"), ForeignKey(nameof(Artist.ArtistId))]
    public long ArtistId { get; set; }

    [Comment("专辑ID"), ForeignKey(nameof(Album.AlbumId))]
    public long AlbumId { get; set; }
}
