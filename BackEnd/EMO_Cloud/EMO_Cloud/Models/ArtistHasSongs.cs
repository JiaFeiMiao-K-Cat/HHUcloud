using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class ArtistHasSongs
    {
        [Key]
        public long Id { get; set; }

        public long ArtistId { get; set; }
        public long SongId { get; set; }
    }
}
