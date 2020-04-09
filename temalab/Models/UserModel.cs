namespace temalab.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { private get; set; }
        public bool isNew { private get; set; }
        public string email { get; set; }
        public double balance { private get; set; }

        public string GetToken() => token;
        public bool GetIsNew() => isNew;
        public double GetBalance() => balance;
    }
}
