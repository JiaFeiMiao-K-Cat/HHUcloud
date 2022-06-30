using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMO_Cloud.Models
{
    public class PlayList
    {
        [Key]
        public long Id { get; set; }

        public string ListTitle { get; set; }
        public long UserId { get; set; }

        [NotMapped]
        public List<long>? SongList { get; set; }
    }
}
