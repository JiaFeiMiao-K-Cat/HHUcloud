using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class PlayList
    {
        [Key]
        public long Id { get; set; }

        public int ListTitle { get; set; }
        public long UserId { get; set; }
        public List<Converter> SongList { get; set; }
    }
}
