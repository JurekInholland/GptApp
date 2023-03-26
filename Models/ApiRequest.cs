namespace Models;

public class ApiRequest
{
    public String Model { get; set; } = "";
    public ApiMessage[] Messages { get; set; } = null!;
    public float Temperature { get; set; }
    public bool Stream { get; set; } = false;
}
