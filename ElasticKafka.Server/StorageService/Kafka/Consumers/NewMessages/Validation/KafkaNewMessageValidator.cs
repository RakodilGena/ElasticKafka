﻿using System.ComponentModel.DataAnnotations;
using StorageService.Kafka.Consumers.NewMessages.Models;

namespace StorageService.Kafka.Consumers.NewMessages.Validation;

internal static class KafkaNewMessageValidator
{
    public static void Validate(this KafkaNewMessage? message)
    {
        if (message is null)
            throw new ValidationException("new message should not be null");

        if (Guid.TryParse(message.Id, out _) is false)
            throw new ValidationException("new message id should be a valid GUID");

        if (string.IsNullOrWhiteSpace(message.Text))
            throw new ValidationException("new message text should not be empty");
    }
}