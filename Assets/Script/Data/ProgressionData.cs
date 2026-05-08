[System.Serializable]
public class SPProgressionData
{
    public string name;
    public Chord[] pattern;
    public int multiplier;
}
public class FProgressionData
{
    public string name;
    public ChordFunction[] pattern;
    public int damage; 
}
public class DegreeProgression
{
    public string name;
    public Degree[] pattern;
    public int Multiplier()
    {
        switch (pattern.Length)
        {
            case 8: return 7;
            case 4: return 5;
            case 3: return 4;
            default: return 2;
        }
    }
}