using System.Collections.Generic;
using UnityEngine;

public static class BattleCalculator
{
    public static int CalculateDamage(
        List<Chord> progression,
        List<DegreeProgression> DegreeProgressionList,
        List<FProgression> FProgressionList,
        int FastSelectDamageBonus,

        ref int ProgressionScoreBonus,
        ref List<string> modifier
    )
    {
        int damage = 0;
        List<Chord> progpart = new List<Chord>();
        int DegreeProgressionCounter = 0;
        int FProgressionCounter = 0;
        float Fmultiplier = 1;
        
        //基礎ダメージ
        foreach (var c in progression)
        {
            damage += c.Damage();
        }

        //コード機能補正
        foreach (var p in FProgressionList)
        {
            int n = MatchFProgression(progression, p.pattern);
            if (n > 0)
            {
                float multiplier = p.multiplier * n;
                Fmultiplier += multiplier;
                modifier.Add($"F : {p.name} (+{multiplier * 100}%)");
                FProgressionCounter++;
            }
        }

        damage = Mathf.FloorToInt(damage * Fmultiplier);

        //コード進行ボーナス
        foreach (var p in DegreeProgressionList)
        {
            if (MatchDegreeProgression(progpart, progression, p.pattern))
            {
                damage += p.AddDamage();
                modifier.Add($"Degree: {p.name} (+{p.AddDamage()})");
                DegreeProgressionCounter += p.pattern.Length;
            }
            if(DegreeProgressionCounter != 0) break;
        }
        
        //残り時間ボーナス
        if (progression.Count != 0)
        {
            damage += FastSelectDamageBonus * 10;
            if(FastSelectDamageBonus != 0)
            {
                modifier.Add($"Fast Select Bonus: +{FastSelectDamageBonus * 10}");
            }
        }
        
        ProgressionScoreBonus += DegreeProgressionCounter + FProgressionCounter * 3;
        
        return damage;
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

    public static int MatchFProgression(
        List<Chord> progression,
        ChordFunction[] pattern
    )
    {
        int n = 0;

        if (progression.Count < pattern.Length) return n;

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

            if (match) n++;
        }

        return n;
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
