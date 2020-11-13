namespace Ogma3.Data.Models
{
    public class UserFollow
    {
        public OgmaUser FollowingUser { get; set; }
        public long FollowingUserId { get; set; }
        public OgmaUser FollowedUser { get; set; }
        public long FollowedUserId { get; set; }
    }
}