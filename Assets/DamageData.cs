
public class DamageData
{
    public int OwnerId;
    public int Damage { get; set; }
    public int Key => GetHashCode();

    public DamageData()
    {
  
    }

    public DamageData(int ActorNumber)
    {
        this.OwnerId = ActorNumber;
        this.Damage = 1;
    }

    public DamageData(int ActorNumber, int Damage)
    {
        this.OwnerId = ActorNumber;
        this.Damage = Damage;
    }
}
