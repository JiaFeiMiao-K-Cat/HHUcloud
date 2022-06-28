using System.ComponentModel.DataAnnotations;

namespace EMO_Cloud.Models
{
    public class Converter
    {
        [Key]
        public long Id { get; set; }
        public long Value { get; set; }
    }
}
