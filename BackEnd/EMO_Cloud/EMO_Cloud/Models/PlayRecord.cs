using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class PlayRecord
    {
        [Key]
        public int Id { get; set; }
        public long Song { get; set; }
        public DateTime Time { get; set; }

    }
}
