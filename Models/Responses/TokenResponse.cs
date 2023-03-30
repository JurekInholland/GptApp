namespace Models.Responses;

public class TokenResponse
{
    public int CharacterCount { get; set; }
    public int TokenCount { get; set; }
    public int[] Tokens { get; set; } = null!;
}
