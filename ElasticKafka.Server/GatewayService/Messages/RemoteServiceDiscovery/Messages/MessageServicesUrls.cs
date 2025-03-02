using System.ComponentModel.DataAnnotations;

namespace GatewayService.Messages.RemoteServiceDiscovery.Messages;

public sealed class MessageServicesUrls
{
    public const string SectionName = "MessageServicesUrls";
    
    [Required]
    public string Value { get; set; } = null!;
}