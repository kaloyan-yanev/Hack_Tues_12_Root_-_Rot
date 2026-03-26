namespace RootAndRot.Server.Models
{
    public class RefreshTokenDTO
    {
        string name {  get; set; }
        DateTime ExpiresAt { get; set; }
        bool Consumed { get; set; } 
        public RefreshTokenDTO(string Name, DateTime ExpiresAt, bool Consumed)
        {
            this.name = Name;
            this.ExpiresAt = ExpiresAt;
            this.Consumed = Consumed;
        }
    }
}
