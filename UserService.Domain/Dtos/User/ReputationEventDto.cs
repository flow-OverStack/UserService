using UserService.Domain.Enums;

namespace UserService.Domain.Dtos.User;

public record ReputationEventDto(
    long AuthorId,
    long InitiatorId,
    long EntityId,
    EntityType EntityType,
    BaseEventType EventType);