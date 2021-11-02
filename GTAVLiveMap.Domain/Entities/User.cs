namespace GTAVLiveMap.Domain.Entities
{
    public class User : Identity<int>
    {
        public string Email { get; set; }
        public string Roles { get; set; } = "User";
    }
}
