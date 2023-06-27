using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("PLAYRECORD"), Comment("播放记录")]
[PrimaryKey(nameof(UserId), nameof(SongId))]
public class PlayRecord
{
    [Comment("用户ID"), ForeignKey(nameof(User.UserId))]
    public long UserId { get; set; }

    [Comment("歌曲ID"), ForeignKey(nameof(Song.SongId))]
    public long SongId { get; set; }

    [Comment("播放时间")]
    public DateTime Played { get; set; }

    [Comment("播放次数")]
    public int Count { get; set; }
}
