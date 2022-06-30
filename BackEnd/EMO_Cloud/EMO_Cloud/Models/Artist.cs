using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMO_Cloud.Models
{
    public class Artist
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        [NotMapped]
        public List<long>? SongList { get; set; }
    }
}