using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class Song
    {
        [Key]
        public long Id { get; set; }

        public int Count { get; set; }
        public string Title { get; set; }
        public virtual List<Converter> ArtistId { get; set; }
        public string Duration { get; set; }
        public long AlbumId { get; set; }
        public string ImgLink { get; set; }
    }
}
