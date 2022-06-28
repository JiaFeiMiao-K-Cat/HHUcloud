namespace EMO_Cloud.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public int Age { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string PassWord { get; set; }
        public string SecurityKey { get; set; }
        public int ProfilePhoto { get; set; }
        public List<long> songSheet { get; set; }
        public List<long> history_Play { get; set; }
    }
}
