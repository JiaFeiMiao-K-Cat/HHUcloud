using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMO_Cloud.Models
{
    public class PlayListHasSongs
    {
        [Key]
        public long Id { get; set; }

        public long PlaylistId { get; set; }
        public long SongId { get; set; }
    }
}
