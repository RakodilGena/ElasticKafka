using System.ComponentModel.DataAnnotations;

namespace MessagingService.Grpc;

internal static class SendMessageRequestRpcValidator
{
    public static void Validate(this SendMessageRequestRpc request)
    {
        if (Guid.TryParse(request.MessageId, out _) is false)
            throw new ValidationException("Invalid message id.");
        
        if (string.IsNullOrEmpty(request.MessageText))
            throw new ValidationException("Empty message text.");
        
        if (request.SentAt is null)
            throw new ValidationException("Empty 'SentAt' timestamp.");
    }
}