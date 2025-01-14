using UserService.Domain.Enum;
using UserService.Domain.Interfaces;

namespace UserService.Domain.Entity;

public class Badge : IEntityId<long>
{
    public BadgeType BadgeType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<User> Users { get; set; }
    public long Id { get; set; }

    //AwardedAt and QuestionAwardedForId is in UserBadge because they are for a specific user

    //question entity is not implemented here
    //It will be implemented via GraphQl via corresponding microservices
}