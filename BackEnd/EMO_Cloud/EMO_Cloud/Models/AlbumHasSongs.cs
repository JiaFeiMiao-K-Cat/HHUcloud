using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class AlbumHasSongs
    {
        [Key]
        public long Id { get; set; }

        public long AlbumId { get; set; }
        public long SongId { get; set; }
    }
}
