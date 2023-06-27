using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("PLAYLIST_HAS_SONG"), Comment("歌单的歌曲列表")]
[PrimaryKey(nameof(PlaylistId), nameof(SongId))]
public class PlaylistHasSong
{
    [Comment("歌单ID"), ForeignKey(nameof(Playlist.PlaylistId))]
    public long PlaylistId { get; set; }

    [Comment("歌曲ID"), ForeignKey(nameof(Song.SongId))]
    public long SongId { get; set; }

    [Comment("添加时间")]
    public DateTime Added { get; set; }
}
