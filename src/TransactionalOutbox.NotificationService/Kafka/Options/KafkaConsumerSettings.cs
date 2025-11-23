namespace TransactionalOutbox.NotificationService.Kafka.Options;

internal class KafkaConsumerSettings
{
    public string Topic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}