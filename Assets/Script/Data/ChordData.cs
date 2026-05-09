[System.Serializable]

public class Chord
{
    public string degreeName;
    public string name;
    public int Damage()
    {
        if(GetTone().Length == 3) return 10;
        else if(GetTone().Length == 4) return 5;
        else return 0;
    }
    public ChordFunction element;
    public NoteData bass;
    public NoteData root;
    public int octave;
    public Degree degree;
    public ChordType type;
    public int[] GetTone()
    {
        return ChordToneData.GetIntervals(type);
    }
    

    public int[] GetMidiNotes()
    {
        int[] tone = GetTone();

        int[] result = new int[tone.Length];

        int rootMidi = (octave + 1) * 12 + (int)root;

        for (int i = 0; i < tone.Length; i++)
        {
            result[i] = rootMidi + tone[i];
        }

        return result;
    }
}


public enum ChordFunction
{
    T, D, SD
}