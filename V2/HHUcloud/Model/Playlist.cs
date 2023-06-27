using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HHUcloud.Model;

[Table("PLAYLIST"), Comment("歌单列表")]
public class Playlist
{
    [Key, Comment("歌单ID")]
    public long PlaylistId { get; set; }

    [Comment("标题")]
    public string Title { get; set; }

    [Comment("创建时间")]
    public DateTime Created { get; set; }

    [Comment("用户ID"), ForeignKey(nameof(User.UserId))]
    public long UserId { get; set; }
}
