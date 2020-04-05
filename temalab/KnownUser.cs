using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using temalab.Models;
using Windows.UI.Xaml;

namespace temalab
{
    public class KnownUser
    {
        public string Username { get; set; }
        public string Name { get; set; }

        public List<KnownUser> GetKnownUsers()
        {
            List<KnownUser> result = new List<KnownUser>();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var rememberedUsers = (string)localSettings.Values["rememberedUsers"];
            var knownUsers = JsonSerializer.Deserialize<List<UserModel>>(rememberedUsers);

            foreach (var user in knownUsers)
            {
                result.Add(new KnownUser { Username = user.username, Name = user.firstName + " " + user.lastName });
            }

            return result;
        }
    }
}
