﻿using GatewayService.Messages.Services.Impl;

namespace GatewayService.Messages.Services;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ICreateMessageService, CreateMessageService>()
            .AddScoped<IGetMessagesService, GetMessagesService>()
            .AddScoped<ISearchMessagesService, SearchMessagesService>()
            .AddScoped<IDeleteMessageService, DeleteMessageService>();
    }
}