using Newtonsoft.Json;

namespace FastVKBot.Requests;
internal abstract class RequestBase<IRequestResult> : IRequestBase<IRequestResult>
{
    private readonly TaskCompletionSource<IRequestResult> _tcs = new();

    public Task Task => _tcs.Task;
    public abstract string MethodName { get; }

    public abstract string GetRequestForVkScript();

    public abstract void ReadAndSetResult(JsonTextReader reader);

    public void SetResult(IRequestResult result)
    {
        _tcs.SetResult(result);
    }

    public void SetCanceled()
    {
        _tcs.SetCanceled();
    }

    
}
