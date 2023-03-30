namespace Services;

public interface ISignalRService
{
    public Task<string> StreamToClient(string connectionId, string callbackName, Stream stream);
}
