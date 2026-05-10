using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DefaultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI turnCountText;
    [SerializeField] TextMeshProUGUI phaseText;
    
    [SerializeField] TextMeshProUGUI nextChoiceText;
    [SerializeField] TextMeshProUGUI progressionCountText;
    [SerializeField] TextMeshProUGUI selectedChordsText;
    
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI modifierText;
    [SerializeField] TextMeshProUGUI enemyHPText;

    [SerializeField] TextMeshProUGUI resultText;

    [SerializeField] TextMeshProUGUI errorText;

    public void UpdateAllUI(
        NoteData key,
        int maxTurn,
        int currentTurn,
        GameState Gstate,
        Chord nextChoice,
        int MaxProgressionLength,
        List<Chord> progression,
        int sumDamage,
        List<string> modifier,
        int enemyHP,
        int finalScore,
        FinishState Fstate,
        ref List<string> errors
    )
    {
        UpdateKeyUI(key);
        UpdateTurnUI(maxTurn, currentTurn);
        UpdatePhaseUI(Gstate);
        UpdateNextChoiceUI(nextChoice);
        UpdateSelctedChordCountUI(MaxProgressionLength, progression);
        UpdateSelectedChordsListUI(progression);
        UpdateDamageUI(sumDamage, Gstate);
        UpdateModifierUI(modifier, Gstate);
        UpdateEnemyHPUI(enemyHP);
        UpdateResultUI(finalScore, Fstate, Gstate);
        UpdateErrorUI(ref errors);
    }


    public void UpdateSelecting(
        Chord nextChoice,
        int MaxProgressionLength,
        List<Chord> progression
    )
    {
        UpdateNextChoiceUI(nextChoice);
        UpdateSelctedChordCountUI(MaxProgressionLength, progression);
        UpdateSelectedChordsListUI(progression);
    }

    public void UpdateExecuting(
        int sumDamage,
        GameState Gstate,
        List<string> modifier,
        int enemyHP
    )
    {
        UpdateDamageUI(sumDamage, Gstate);
        UpdateModifierUI(modifier, Gstate);
        UpdateEnemyHPUI(enemyHP);
    }


    public void UpdateKeyUI(
        NoteData key
    )
    {
        keyText.text = ChordManager.GetNoteName(key);
    }

    public void UpdateTurnUI(
        int maxTurn,
        int currentTurn
    )
    {
        turnCountText.text = currentTurn + "/" + maxTurn;
    }

    public void UpdatePhaseUI(
        GameState Gstate
    )
    {
        phaseText.text = ""; 
        if (Gstate == GameState.Selecting || Gstate == GameState.Waiting) phaseText.text = "Composing";
        else if (Gstate == GameState.Calculating) phaseText.text = "Playing";
        else if (Gstate == GameState.Preparing) phaseText.text = "Preparing";
    }

    public void UpdateNextChoiceUI(
        Chord nextChoice
    )
    {
        nextChoiceText.text = "";
        if (nextChoice != null) nextChoiceText.text = nextChoice.name;
    }
    public void UpdateSelctedChordCountUI(
        int MaxProgressionLength,
        List<Chord> progression
    )
    {
        progressionCountText.text = progression.Count + "/" +  MaxProgressionLength;
    }
    public void UpdateSelectedChordsListUI(
        List<Chord> progression
    )
    {
        selectedChordsText.text = "";
        for(int i = 0; i < progression.Count; i++){
            selectedChordsText.text += $"{i + 1}: {progression[i].name}\n";
        }
    }

    public void UpdateDamageUI(
        int sumDamage,
        GameState Gstate
    )
    {
        damageText.text = "";
        if (Gstate == GameState.Executing) damageText.text = sumDamage + " damage";
    }
    public void UpdateModifierUI(
        List<string> modifier,
        GameState Gstate
    )
    {
        modifierText.text = "";
        if (Gstate == GameState.Executing)
        {
            if (modifier.Count != 0) modifierText.text = "BornusDamage\n";
            for(int i = 0; i < modifier.Count; i++){
                modifierText.text += modifier[i] + "\n";
            }
        }
    }

    public void UpdateEnemyHPUI(
        int enemyHP
    )
    {
        enemyHPText.text = $"{enemyHP}";
    }

    public void UpdateResultUI(
        int finalScore,
        FinishState Fstate,
        GameState Gstate
    )
    {
        resultText.text = "";
        if (Gstate == GameState.Result)
        {
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
    }
    
    public void UpdateErrorUI(
        ref List<string> errors
    )
    {
        errorText.text = "";
        for(int i = 0; i < errors.Count; i++){
            errorText.text += errors[i] + "\n";
        }
        errors.Clear();
    }
}
