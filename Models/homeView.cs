namespace communityWeb.Models
{
    public class homeView
    {
        public List<postView> Posts { get; set; }
        public List<Community> Communities { get; set; }
        public List<Community> Community { get; set; }
        public List<FriendRequest> FriendRequests { get; set; }
        public List<FriendRequest> FriendRequestsFromFriend { get; set; }
        public List<Award> Awards { get; set; }

    }
}
