namespace SocialNetwork.API.Models
{
    public class Follow
    {
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
        public User Follower { get; set; }
        public User Followed { get; set; }
    }
}