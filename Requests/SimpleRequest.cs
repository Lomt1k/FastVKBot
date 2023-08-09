using Newtonsoft.Json;

namespace FastVKBot.Requests;
internal abstract class SimpleRequest : IRequestBase
{
    private readonly TaskCompletionSource _tcs = new();

    public Task Task => _tcs.Task;

    public abstract string MethodName { get; }
    public abstract string GetRequestForVkScript();
    public abstract void ReadAndSetResult(JsonTextReader reader);

    public void SetResult()
    {
        _tcs.SetResult();
    }

    public void SetCanceled()
    {
        _tcs.SetCanceled();
    }
}
