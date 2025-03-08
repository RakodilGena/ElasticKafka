using System.ComponentModel.DataAnnotations;

namespace GatewayService.Messages.RemoteServiceDiscovery;

public sealed class ServiceDiscoveryUrls
{
    public const string SectionName = "ServiceDiscoveryUrls";
    
    [Required]
    public string? Value { get; set; }
}