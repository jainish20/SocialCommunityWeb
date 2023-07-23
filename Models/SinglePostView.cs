namespace communityWeb.Models
{
    public class SinglePostView
    {
        public postView Post { get; set; }
        public List<PostFeedback> Feedbacks { get; set; }
        public string noOfMembers { get; set; }
        public List<Community> userCommunity { get; set; }
    }
}
