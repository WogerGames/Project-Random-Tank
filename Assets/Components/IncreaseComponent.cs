
struct IncreaseComponent
{
    public byte Value;

    public override string ToString()
    {
        return Value == 0 ? "" : Value == 1 ? "I" : Value == 2 ? "II" : Value == 3 ? "III" : "Уебался блять";
    }
}

public static class IncreaseExtinsions
{
    internal static int GetChance(this IncreaseComponent increase)
    {
        if(increase.Value < 1)
        {
            return 50;
        }
        else if (increase.Value < 2)
        {
            return 30;
        }
        else if (increase.Value < 3)
        {
            return 10;
        }

        return 0;
    }
}
