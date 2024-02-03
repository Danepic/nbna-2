public class StatsEntity
{
    public int aggressive;

    public int technique;

    public int intelligent;

    public int speed;

    public int resistance;

    public int stamina;

    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
