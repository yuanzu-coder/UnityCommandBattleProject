using UnityEngine;
using TMPro;
using System.Collections.Generic;

public static class DefaultUI
{
    public static void UpdateKeyUI(
        TextMeshProUGUI keyText,
        NoteData key
    )
    {
        keyText.text = ChordManager.GetNoteName(key);
    }

    public static void UpdateEnemyHPUI(
        TextMeshProUGUI enemyHPText,
        int enemyHP
    )
    {
        enemyHPText.text = $"{enemyHP}";
    }

    public static void UpdateTurnUI(
        int maxTurn,
        int currentTurn,
        TextMeshProUGUI turnCountText
    )
    {
        turnCountText.text = currentTurn + "/" + maxTurn;
    }

    public static void UpdatePhaseUI(
        TextMeshProUGUI phaseText,
        GameState Gstate
    )
    {
        phaseText.text = ""; 
        if (Gstate == GameState.Selecting) phaseText.text = "Composing";
        else if (Gstate == GameState.Calculating) phaseText.text = "Playing";
        else if (Gstate == GameState.Executing) phaseText.text = "Preparing";
    }

    public static void UpdateNextChoiceUI(
        Chord nextChoice,
        TextMeshProUGUI nextChoiceText
    )
    {
        nextChoiceText.text = "";
        if (nextChoice != null) nextChoiceText.text = nextChoice.name;
    }

    public static void UpdateSelctedChordCountUI(
        int MaxProgressionNum,
        List<Chord> progression,
        TextMeshProUGUI progressionCountText
    )
    {
        progressionCountText.text = progression.Count + "/" +  MaxProgressionNum;
    }

    public static void UpdateSelectedChordsListUI(
        List<Chord> progression,
        TextMeshProUGUI selectedChordsText
    )
    {
        selectedChordsText.text = "";
        for(int i = 0; i < progression.Count; i++){
            selectedChordsText.text += $"{i + 1}: {progression[i].name}\n";
        }
    }

    public static void UpdateDamageUI(
        int sumDamage,
        TextMeshProUGUI damageText,
        GameState Gstate
    )
    {
        damageText.text = "";
        if (Gstate == GameState.Executing) damageText.text = sumDamage + " damage";
    }

    public static void UpdateModifierUI(
        string modifier,
        TextMeshProUGUI modifierText
    )
    {
        modifierText.text = "";
        if (modifier != "") modifierText.text = "BornusDamage\n" + modifier;
    }

    public static void UpdateResultUI(
        int finalScore,
        TextMeshProUGUI resultText,
        FinishState Fstate
    )
    {
        resultText.text = "";
        switch (Fstate)
        {
            case FinishState.CREAR:
                resultText.text = "SCORE: " + finalScore;
                break;
            case FinishState.TURNOVER:
                resultText.text = "TURNOVER\nSCORE: " + finalScore;
                break;  
        }
    }
    
    public static void UpdateErrorUI(
        List<string> errors,
        TextMeshProUGUI errorText
    )
    {
        errorText.text = "";
        for(int i = 0; i < errors.Count; i++){
            errorText.text += errors[i] + "\n";
        }
    }   
}
