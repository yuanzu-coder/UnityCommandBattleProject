using UnityEngine;
using System.Collections.Generic;

public static class ProgressionManager
{
    public static List<DegreeProgression> CreateDegreeProgression()
    {
        List<DegreeProgression> Progressions = new List<DegreeProgression>()
        {
            new DegreeProgression
            {
                name = "Canon Progression (8 Chords)",
                pattern = new [] { Degree.Ⅰ, Degree.Ⅴ, Degree.Ⅵ, Degree.Ⅲ, Degree.Ⅳ, Degree.Ⅰ, Degree.Ⅱ, Degree.Ⅴ }
            },
            new DegreeProgression
            {
                name = "Minor Canon Progression (8 Chords)",
                pattern = new [] { Degree.Ⅵ, Degree.Ⅲ, Degree.Ⅳ, Degree.Ⅰ, Degree.Ⅱ, Degree.Ⅵ, Degree.Ⅶ, Degree.Ⅲ }
            },
            new DegreeProgression
            {
                name = "Only-One Progression (8 Chords)",
                pattern = new [] { Degree.Ⅰ, Degree.Ⅳ, Degree.Ⅴ, Degree.Ⅲ, Degree.Ⅵ, Degree.Ⅱ, Degree.Ⅳ, Degree.Ⅴ }
            },
            new DegreeProgression
            {
                name = "Canon Progression",
                pattern = new [] { Degree.Ⅰ, Degree.Ⅴ, Degree.Ⅵ, Degree.Ⅲ }
            },
            new DegreeProgression
            {
                name = "Only-One Progression",
                pattern = new [] { Degree.Ⅰ, Degree.Ⅳ, Degree.Ⅴ, Degree.Ⅲ }
            },
            new DegreeProgression
            {
                name = "Ascending Prpgression (from Dm)",
                pattern = new [] { Degree.Ⅱ, Degree.Ⅲ, Degree.Ⅳ, Degree.Ⅴ }
            },
            new DegreeProgression
            {
                name = "Royal-Road Progression",
                pattern = new [] { Degree.Ⅳ, Degree.Ⅴ, Degree.Ⅲ, Degree.Ⅵ }
            },
            new DegreeProgression
            {
                name = "Pop-Punk Progression",
                pattern = new [] { Degree.Ⅳ, Degree.Ⅰ, Degree.Ⅴ, Degree.Ⅵ }
            },
            new DegreeProgression
            {
                name = "Just The Two of Us Progression",
                pattern = new [] { Degree.Ⅳ, Degree.Ⅲ, Degree.Ⅵ, Degree.Ⅴ }
            },
            new DegreeProgression
            {
                name = "Komuro's Progression",
                pattern = new [] { Degree.Ⅵ, Degree.Ⅳ, Degree.Ⅴ, Degree.Ⅰ }
            },
            new DegreeProgression
            {
                name = "Minor Canon Progression",
                pattern = new [] { Degree.Ⅵ, Degree.Ⅲ, Degree.Ⅳ, Degree.Ⅰ }
            },
            new DegreeProgression
            {
                name = "Two-Five-One Progression",
                pattern = new [] { Degree.Ⅱ, Degree.Ⅴ, Degree.Ⅰ }
            },
            new DegreeProgression
            {
                name = "Minor Two-Five-One Progression",
                pattern = new [] { Degree.Ⅶ, Degree.Ⅲ, Degree.Ⅵ }
            }
        };

        return Progressions;
    }

    public static List<FProgression> CreateFProgression()
    {
        List<FProgression> FProgressionList = new List<FProgression>()
        {
            new FProgression
            {
                name = "Dominant Motion (D-T)",
                pattern = new [] { ChordFunction.D, ChordFunction.T },
                multiplier = 0.5f
            },
            new FProgression
            {
                name = "Deceptive Cadence (D-D)",
                pattern = new [] { ChordFunction.D, ChordFunction.D },
                multiplier = 0.3f
            },
            new FProgression
            {
                name = "Deceptive Cadence (D-SD)",
                pattern = new [] { ChordFunction.D, ChordFunction.SD },
                multiplier = 0.3f
            },
            new FProgression
            {
                name = "Plagal Cadence (SD-T)",
                pattern = new [] { ChordFunction.SD, ChordFunction.T },
                multiplier = 0.4f
            },
            new FProgression
            {
                name = "T-SD",
                pattern = new [] { ChordFunction.T, ChordFunction.SD },
                multiplier = 0.2f
            },
            new FProgression
            {
                name = "SD-D",
                pattern = new [] { ChordFunction.SD, ChordFunction.D },
                multiplier = 0.2f
            }
        };

        return FProgressionList; 
    }
}
