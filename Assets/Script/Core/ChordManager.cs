using UnityEngine;
using System.Collections.Generic;

public class ChordManager
{
    public List<Chord> CreateTriadDiatonic(NoteData key)
    {
        NoteData 
            note1 = key,
            note2 = (NoteData)(((int)key + 2) % 12),
            note3 = (NoteData)(((int)key + 4) % 12),
            note4 = (NoteData)(((int)key + 5) % 12),
            note5 = (NoteData)(((int)key + 7) % 12),
            note6 = (NoteData)(((int)key + 9) % 12),
            note7 = (NoteData)(((int)key + 11) % 12);
        List<Chord> Chords = new  List<Chord> ()
        {
            new Chord
            {
                element = ChordFunction.T,
                octave = 4,
                root = note1,
                degree = Degree.Ⅰ,
                type = ChordType.Major,
                degreeName = GetDegree(Degree.Ⅰ) + GetChordType(ChordType.Major),
                name = GetNoteName(note1) + GetChordType(ChordType.Major)
            },
            new Chord
            {
                element = ChordFunction.SD,
                octave = 4,
                root = note2,
                degree = Degree.Ⅱ,
                type = ChordType.Minor,
                degreeName = GetDegree(Degree.Ⅱ) + GetChordType(ChordType.Minor),
                name = GetNoteName(note2) + GetChordType(ChordType.Minor)
            },
            new Chord
            {
                element = ChordFunction.D,
                octave = 4,
                root = note3,
                degree = Degree.Ⅲ,
                type = ChordType.Minor,
                degreeName = GetDegree(Degree.Ⅲ) + GetChordType(ChordType.Minor),
                name = GetNoteName(note3) + GetChordType(ChordType.Minor)
            },
            new Chord
            {
                element = ChordFunction.SD,
                octave = 4,
                root = note4,
                degree = Degree.Ⅳ,
                type = ChordType.Major,
                degreeName = GetDegree(Degree.Ⅳ) + GetChordType(ChordType.Major),
                name = GetNoteName(note4) + GetChordType(ChordType.Major)
            },
            new Chord
            {
                element = ChordFunction.D,
                octave = 4,
                root = note5,
                degree = Degree.Ⅴ,
                type = ChordType.Major,
                degreeName = GetDegree(Degree.Ⅴ) + GetChordType(ChordType.Major),
                name = GetNoteName(note5) + GetChordType(ChordType.Major)
            },
            new Chord
            {
                element = ChordFunction.T,
                octave = 4,
                root = note6,
                degree = Degree.Ⅵ,
                type = ChordType.Minor,
                degreeName = GetDegree(Degree.Ⅵ) + GetChordType(ChordType.Minor),
                name = GetNoteName(note6) + GetChordType(ChordType.Minor)
            },
            new Chord
            {
                element = ChordFunction.SD,
                octave = 4,
                root = note7,
                degree = Degree.Ⅶ,
                type = ChordType.Diminish,
                degreeName = GetDegree(Degree.Ⅶ) + GetChordType(ChordType.Diminish),
                name = GetNoteName(note7) + GetChordType(ChordType.Diminish)
            }
        };

        return Chords;
    }
    string GetDegree(Degree degree)
    {
        switch (degree)
        {
            case Degree.Ⅰ: return "Ⅰ";
            case Degree.Ⅰs: return "Ⅰ♯";
            case Degree.Ⅱ: return "Ⅱ";
            case Degree.Ⅱs: return "Ⅱ♯";
            case Degree.Ⅲ: return "Ⅲ";
            case Degree.Ⅳ: return "Ⅳ";
            case Degree.Ⅳs: return "Ⅳ♯";
            case Degree.Ⅴ: return "Ⅴ";
            case Degree.Ⅴs: return "Ⅴ♯";
            case Degree.Ⅵ: return "Ⅵ";
            case Degree.Ⅵs: return "Ⅵ♯";
            case Degree.Ⅶ: return "Ⅶ";
            
        }
        return "";
    }
    string GetChordType(ChordType type)
    {
        switch (type)
        {
            case ChordType.Major: return "";
            case ChordType.Minor: return "m";
            case ChordType.Diminish: return "dim";
            case ChordType.Augment: return "arg";
            case ChordType.Dominant7: return "7";
            case ChordType.Major7: return "M7";
            case ChordType.Minor7: return "m7";
            case ChordType.Minor7b5: return "m7(♭5)";
        }
        return "";
    }
    string GetNoteName(NoteData note)
    {
        switch (note)
        {
            case NoteData.C: return "C";
            case NoteData.Cs: return "C♯";
            case NoteData.D: return "D";
            case NoteData.Ds: return "D♯";
            case NoteData.E: return "E";
            case NoteData.F: return "F";
            case NoteData.Fs: return "F♯";
            case NoteData.G: return "G";
            case NoteData.Gs: return "G♯";
            case NoteData.A: return "A";
            case NoteData.As: return "A♯";
            case NoteData.B: return "B";
        }
        return "";
    }
}
