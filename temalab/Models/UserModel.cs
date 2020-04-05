namespace temalab.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
        public bool isNew { get; set; } = true;
        public string email { get; set; }
    }
}
