namespace FastVKBot.DataTypes;
public struct UserId
{
    public long Id { get; set; }

    public UserId(long id)
    {
        Id = id;
    }

    public override string ToString()
    {
        return Id.ToString();
    }

    public static implicit operator UserId(long id) => new(id);
}
