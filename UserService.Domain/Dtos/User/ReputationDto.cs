namespace UserService.Domain.Dtos.User;

public class ReputationDto
{
    public long UserId { get; set; }
    public int CurrentReputation { get; set; }
    public int RemainingDailyLimit { get; set; }
}