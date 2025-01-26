namespace CrmPlatformAPI.SingalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> Users = new Dictionary<string, List<string>>();

        public Task Connected(string username, string connectionId)
        {
            lock (Users)
            {
                if (Users.ContainsKey(username))
                {
                    Users[username].Add(connectionId);
                }
                else
                {
                    Users.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;
        }

        public Task Disconnected(string username, string connectionId)
        {
            lock (Users)
            {
                if (!Users.ContainsKey(username)) return Task.CompletedTask;

                Users[username].Remove(connectionId);
                if (Users[username].Count == 0)
                {
                    Users.Remove(username);
                }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (Users)
            {
                onlineUsers = Users.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public Task<string[]> GetConnectionsUser(string username)
        {
            string[] connections;
            lock (Users)
            {
                connections = Users.GetValueOrDefault(username)?.ToArray() ?? new string[0];
            }
            return Task.FromResult(connections);
        }
    }
}
