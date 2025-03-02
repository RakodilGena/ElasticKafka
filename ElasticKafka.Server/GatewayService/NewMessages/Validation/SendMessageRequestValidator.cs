using System.ComponentModel.DataAnnotations;
using GatewayService.NewMessages.Models;

namespace GatewayService.NewMessages.Validation;

internal static class SendMessageRequestValidator
{
    public static void Validate(this SendMessageRequest request)
    {
        if (string.IsNullOrEmpty(request.MessageText))
            throw new ValidationException("Empty message text.");
        
        if (request.SentAt is null)
            throw new ValidationException("Empty 'SentAt' timestamp.");
    }
}