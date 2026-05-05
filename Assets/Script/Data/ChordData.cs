[System.Serializable]
public class Chord
{
    public string name;
    public int damage;
    public ChordFunction element;
}

public enum ChordFunction
{
    T, D, SD
}