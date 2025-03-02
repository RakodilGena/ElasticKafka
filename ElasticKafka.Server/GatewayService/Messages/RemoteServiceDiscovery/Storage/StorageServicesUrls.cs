using System.ComponentModel.DataAnnotations;

namespace GatewayService.Messages.RemoteServiceDiscovery.Storage;

public sealed class StorageServicesUrls
{
    public const string SectionName = "StorageServicesUrls";
    
    [Required]
    public string Value { get; set; } = null!;
}