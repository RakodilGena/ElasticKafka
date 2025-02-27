using System.ComponentModel.DataAnnotations;

namespace GatewayService.NewMessages.Setup;

public sealed class MessageServicesUrls
{
    public const string SectionName = "MessageServicesUrls";
    
    [Required]
    public string Value { get; set; } = null!;
}