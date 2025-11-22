using TransactionalOutbox.Contracts.Outbox.Enums;

namespace TransactionalOutbox.Contracts.Outbox.Models;

public record OutboxMessagePayload(Guid UserId, Guid OrderId, OutboxMessageType Type);