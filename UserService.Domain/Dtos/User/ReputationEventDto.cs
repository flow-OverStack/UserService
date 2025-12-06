using UserService.Domain.Enums;

namespace UserService.Domain.Dtos.User;

public record ReputationEventDto(long UserId, long EntityId, EntityType EntityType, BaseEventType EventType);