﻿namespace StorageService.Elastic.Models;

/// <summary>
/// 
/// </summary>
/// <param name="Id">Indexed</param>
/// <param name="MessageText">NGram Indexed</param>
/// <param name="SentAt">Indexed</param>
/// <param name="SavedAt">NOT Indexed</param>
public sealed record MessageElastic(
    long Id,
    string MessageText,
    DateTimeOffset SentAt,
    DateTimeOffset SavedAt);