using Google.Protobuf.WellKnownTypes;
using MessagingService;
using StorageService.Elastic.Models;
using StorageService.Messages.Models;

namespace StorageService.Messages.Mapping;

internal static class MessageMapper
{
    public static MessageRpc ToRpc(this MessageDto message)
    {
        return new MessageRpc
        {
            Id = message.Id.ToString(),
            Text = message.Text,
            SentAt = message.SentAt.ToTimestamp()
        };
    }

    public static MessageDto ToDto(this ElasticMessage message)
    {
        return new MessageDto(message.Id,
            message.Text,
            message.SentAt);
    }
}