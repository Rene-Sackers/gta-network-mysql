using GTANetworkServer;
using MySQLLinqExample.resources.mysql.Server.Models;
using System.Linq;

namespace MySQLLinqExample.resources.mysql.Server
{
    class Main : Script
    {
        public Main()
        {
            API.onResourceStart += OnResourceStart;
            API.onPlayerConnected += OnPlayerConnected;
        }

        private void OnResourceStart()
        {
            ContextFactory.SetConnectionParameters("127.0.0.1", "root", "root", "example_database");
            
            using (var database = ContextFactory.Instance)
            {
                var uniqueUsers = database.UserProfiles.Count();
                API.consoleOutput("Unique players: " + uniqueUsers);
            }
        }

        private void OnPlayerConnected(Client player)
        {
            using (var database = ContextFactory.Instance)
            {
                var userProfile = database.UserProfiles.FirstOrDefault(up => up.SocialClubName == player.socialClubName);

                if (userProfile == null)
                {
                    API.consoleOutput("New player: " + player.socialClubName);

                    userProfile = new UserProfile { SocialClubName = player.socialClubName };
                    database.UserProfiles.Add(userProfile);
                }
                else
                {
                    API.consoleOutput($"Returning player: {player.socialClubName}. Last display name: {userProfile.LastDisplayName}, current display name: {player.name}");
                }

                userProfile.LastIp = player.address;
                userProfile.LastDisplayName = player.name;

                database.SaveChanges();
            }
        }
    }
}
