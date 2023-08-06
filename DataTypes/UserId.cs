namespace FastVKBot.DataTypes;
public struct UserId
{
    public long Id { get; set; }

    public override string ToString()
    {
        return Id.ToString();
    }
}
