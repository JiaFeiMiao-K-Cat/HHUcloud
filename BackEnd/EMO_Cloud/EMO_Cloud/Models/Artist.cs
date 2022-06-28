namespace EMO_Cloud.Models
{
    public class Artist
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
        public List<long> SongList { get; set; }
    }
}