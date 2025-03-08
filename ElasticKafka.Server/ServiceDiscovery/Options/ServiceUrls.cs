using System.ComponentModel.DataAnnotations;

namespace ServiceDiscovery.Options;

public sealed class ServiceUrls
{
    public const string SectionName = "ServiceUrls";

    [Required] public string StorageServices { get; set; } = null!;

    [Required] public string MessagingServices { get; set; } = null!;
}