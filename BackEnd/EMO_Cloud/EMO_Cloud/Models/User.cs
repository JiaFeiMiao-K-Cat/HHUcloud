using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMO_Cloud.Models
{
    public enum Role
    {
        User = 1,
        Administrator = 2,
        Root = 4
    }
    [Table("USERS")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        public int Age { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string Password { get; set; }
        public string SecurityKey { get; set; }
        public int ProfilePhoto { get; set; }
        public virtual List<Converter> PlayLists { get; set; }
        public virtual List<PlayRecord> HistoryPlay { get; set; }
    }
}
