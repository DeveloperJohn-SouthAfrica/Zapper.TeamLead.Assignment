namespace AcmeIntegreatedAssessement.Api.Contracts;

public sealed record MessageHeader
{
    public DateTimeOffset MessageDateTime { get; init; }
    public Guid MessageID { get; init; }
    public string? Channel { get; init; }
}

public sealed record MessageEnvelope<T>
{
    public MessageHeader? MessageHeader { get; init; }
    public T? Payload { get; init; }
}

public sealed record MessageAcknowledgement(bool Accepted);

public static class ApiMessageEnvelope
{
    private static readonly TimeSpan SouthAfricanOffset = TimeSpan.FromHours(2);

    public static bool IsValidHeader(MessageHeader? header) =>
        header is not null &&
        header.MessageID != Guid.Empty &&
        header.MessageDateTime.Offset == SouthAfricanOffset &&
        IsValidChannel(header.Channel);

    public static bool IsValidChannel(string? channel) => channel is "App" or "Web" or "POS";

    public static MessageEnvelope<T> Create<T>(T payload, string channel) =>
        new()
        {
            MessageHeader = new MessageHeader
            {
                MessageDateTime = DateTimeOffset.UtcNow.ToOffset(SouthAfricanOffset),
                MessageID = Guid.NewGuid(),
                Channel = channel
            },
            Payload = payload
        };
}
