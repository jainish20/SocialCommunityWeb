namespace communityWeb.Models
{
    public class SearchPostView
    {
        public List<Post> Posts { get; set; }
        public List<Community> Communities { get; set; }
        public List<PostFeedback> Feedbacks { get; set; }
        public List<CommunityMember> Members { get; set; }
        public List<Community> userCommunity { get; set; }
        public List<FriendRequest> FriendRequests { get; set; }
        public List<FriendRequest> FriendRequestsFromFriend { get; set; }
        public List<Community> AllCommunities { get; set; }
        public List<Community> Community { get; set; }
        public List<User> People { get; set; }
        public List<Vote> Votes { get; set; }
        public List<DownVote> DownVotes { get; set; }
       


    }
}
