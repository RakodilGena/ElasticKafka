namespace StorageService.Messages.Models.Responses;

public sealed class DeleteMessageResponseDto
{
    public bool Success { get; init; }
    public string? Reason { get; init; }
    
    private DeleteMessageResponseDto()
    {
        
    }

    public static DeleteMessageResponseDto OnSuccess()
    {
        return new DeleteMessageResponseDto
        {
            Success = true,
            Reason = null
        };
    }

    public static DeleteMessageResponseDto OnFailure(string reason)
    {
        return new DeleteMessageResponseDto
        {
            Success = false,
            Reason = reason
        };
    }
}