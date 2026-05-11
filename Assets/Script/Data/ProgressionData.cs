[System.Serializable]
public class FProgression
{
    public string name;
    public ChordFunction[] pattern;
    public float multiplier;
}
public class DegreeProgression
{
    public string name;
    public Degree[] pattern;
    public int AddDamage()
    {
        switch (pattern.Length)
        {
            case 8: return 300;
            case 4: return 100;
            case 3: return 50;
            default: return 0;
        }
    }
}