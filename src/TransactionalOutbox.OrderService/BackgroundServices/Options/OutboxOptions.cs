namespace TransactionalOutbox.OrderService.BackgroundServices.Options;

internal class OutboxOptions
{
    public TimeSpan SendPeriod { get; set; }
    public int BatchSize { get; set; }
}