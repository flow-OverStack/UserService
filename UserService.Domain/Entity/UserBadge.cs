namespace UserService.Domain.Entity;

public class UserBadge
{
    public long UserId { get; set; }

    public long BadgeId { get; set; }

    public DateTime AwardedAt { get; set; }
    public long QuestionAwardedForId { get; set; }
}