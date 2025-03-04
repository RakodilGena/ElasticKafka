namespace StorageService.Elastic.Models;

/// <summary>
/// 
/// </summary>
/// <param name="Id">Indexed</param>
/// <param name="Text">NGram Indexed</param>
/// <param name="SentAt">Indexed</param>
/// <param name="SavedAt">NOT Indexed</param>
public sealed record ElasticMessage(
    Guid Id,
    string Text,
    DateTimeOffset SentAt,
    DateTimeOffset SavedAt);