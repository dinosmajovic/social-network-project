namespace SocialNetwork.API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public int EventOwnerId { get; set; }
    }
}