using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleCalculator
{
    public static int CalculateDamage(
        List<Chord> progression,
        List<DegreeProgression> DegreeProgressionList,
        List<FProgression> FProgressionList,
        int FastSelectDamageBonus,

        ref int ProgressionScoreBonus,
        ref string Modifier
    )
    {
        int damage = 0;
        List<Chord> progpart = new List<Chord>();
        int DegreeProgressionCounter = 0;
        int FProgressionCounter = 0;

        foreach (var p in DegreeProgressionList)
        {
            if (BattleCalculator.MatchDegreeProgression(progpart, progression, p.pattern))
            {
                int CalcResult = BattleCalculator.CalculateDegreeProgressionModifier(p, progpart);
                damage += CalcResult;
                Modifier += $"Degree: {p.name} (+{CalcResult})\n";
                DegreeProgressionCounter += p.pattern.Length;
            }
            if(DegreeProgressionCounter != 0) break;
        }
        
        if(DegreeProgressionCounter == 0)
        {
            foreach (var p in FProgressionList)
            {
                if (BattleCalculator.MatchFProgression(progression, p.pattern))
                {
                    damage += p.damage;
                    Modifier += $"F : {p.name} (+{p.damage})\n";
                    FProgressionCounter++;
                }
            }
        }
        
        foreach (var c in progression)
        {
            damage += c.Damage();
        }

        damage += FastSelectDamageBonus * 10;
        if(FastSelectDamageBonus != 0)
        {
            Modifier += $"Fast Select Bonus: +{FastSelectDamageBonus * 10}";
        }

        ProgressionScoreBonus += 2 * DegreeProgressionCounter + FProgressionCounter;
        
        return damage;
    }
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

    public static int CalculateScore(
        FinishState Fstate,
        int maxTurn,
        int currentTurn,
        int FastSelectScoreBonus,
        int ProgressionScoreBonus
    )
    {   
        float score = 0;
        
        if (Fstate == FinishState.CREAR)
        {
            score += (maxTurn - currentTurn + 1) * 2000;
            score += FastSelectScoreBonus * 20;
        }
        score += ProgressionScoreBonus * 50;
        

        return Mathf.RoundToInt(score);
    }
}
