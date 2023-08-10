namespace FastVKBot.Requests;
internal abstract class MultipleRequest<T> : SimpleRequest
{
    public abstract bool TryAddToRequest(T value);
}
