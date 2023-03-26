namespace Services;

public interface ISignalRService
{
    public Task StreamToClient(string connectionId, string callbackName, Stream stream);
}
