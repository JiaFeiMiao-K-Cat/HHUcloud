using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class PlayRecord
    {
        [Key]
        public int Id { get; set; }

        public long UserId { get; set; }
        public long SongId { get; set; }
        public DateTime LastTime { get; set; }

        public long Count { get; set; }

    }
}
