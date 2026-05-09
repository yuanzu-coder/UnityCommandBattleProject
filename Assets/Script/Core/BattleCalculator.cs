using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleCalculator
{
    public static int CalculateDegreeProgressionModifier(
        DegreeProgression p,
        List<Chord> progpart
    )
    {
        int DegreeProgressionModifier = 0;
        foreach (var c in progpart)
        {
            DegreeProgressionModifier += c.Damage();
        }
        DegreeProgressionModifier *= p.Multiplier();

        return DegreeProgressionModifier;
    } 
    
    public static bool MatchDegreeProgression(
        List<Chord> progpart,
        List<Chord> progression,
        Degree[] pattern
    )
    {
        if (progression.Count < pattern.Length) return false;

        for (int j = 0; j <= progression.Count - pattern.Length; j++)
        {
            bool match = true;

            for (int k = 0; k < pattern.Length; k++)
            {
                if (progression[j + k].degree != pattern[k])
                {
                    match = false;
                    progpart.Clear();
                    break;
                }
                progpart.Add(progression[j + k]);
            }

            if (match) return true;
        }

        return false;
    }
    public static bool MatchFProgression(
        List<Chord> progression,
        ChordFunction[] pattern
    )
    {
        if (progression.Count < pattern.Length) return false;

        for (int j = 0; j <= progression.Count - pattern.Length; j++)
        {
            bool match = true;

            for (int k = 0; k < pattern.Length; k++)
            {
                if (progression[j + k].element != pattern[k])
                {
                    match = false;
                    break;
                }
            }

            if (match) return true;
        }

        return false;
    }
}
