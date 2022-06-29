using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class Artist
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public virtual List<Converter> SongList { get; set; }
    }
}