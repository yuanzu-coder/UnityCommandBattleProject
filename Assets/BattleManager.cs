using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class BattleManager : MonoBehaviour
{
    public float selectTimeLimit = 10f;
    float selectTimer;

    public float battleTimeLimit = 100f;
    float battleTimer;
    float finalbattleTimer;

    int MaxEnemyHP = 500;
    public int enemyHP;
    public int finalScore;
    public int sumDamge;

    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI selectedSkillsText;
    public TextMeshProUGUI comboNumText;
    public TextMeshProUGUI selectTimerText;
    public TextMeshProUGUI battleTimerText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText;
    
    class Skill
    {
        public string name;
        public int damage;
    }
    Skill attack;
    Skill fire;
    List<Skill> skills = new List<Skill>();

    int MaxComboNum = 5;
    List<Skill> combo = new List<Skill>();

    class SPComboData
    {
        public string name;
        public Skill[] pattern;
        public int damage;
    }
    List<SPComboData> SPcomboList = new List<SPComboData>();

    public enum SkillID
    {
        Attack, Fire
    }

    public enum GameState
    {
        Selecting,
        Executing,
        Result,
        Finished
    }
    GameState state;

    void Awake()
    {
        //Skillの宣言
        attack = new Skill { name = "Attack", damage = 10};
        fire = new Skill { name = "Fire", damage = 20};

        skills = new List<Skill>()
        {
            attack, fire
        };

        SPcomboList = new List<SPComboData>()
        {
            new SPComboData
            {
                name = "AAA",
                pattern = new [] { attack, attack, attack },
                damage = 10 
            },
            new SPComboData
            {
                name = "AFF",
                pattern = new [] { attack, fire, fire },
                damage = 20
            }
        };
    }

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (state == GameState.Selecting)
        {
            selectTimer -= Time.deltaTime;

            if (selectTimer <= 0f)
            {
                AutoConfirm();
            }

            UpdateTimerUI(selectTimerText, selectTimer);
        }
        if (state != GameState.Result && state != GameState.Finished)
        {
            battleTimer -= Time.deltaTime;

            if (battleTimer <= 0f)
            {
                battleTimer = 0;
                FinishGame();
            }

            UpdateTimerUI(battleTimerText, battleTimer);
        }
    }

    public void InitGame()
    {
        enemyHP = MaxEnemyHP;
        finalScore = 0;
        sumDamge = 0;

        damageText.text = "";
        errorText.text = "";
        resultText.text = "";

        battleTimer = battleTimeLimit;

        state = GameState.Selecting;
        selectTimer = selectTimeLimit;
        
        UpdateUI();
    }

    public void SelectSkill(int index)
    {
        if(state != GameState.Selecting) return;
        if(combo.Count >= MaxComboNum) {
            UpdateUI();
            errorText.text = "do not select skills over this" ;
            return;
        }
        combo.Add(skills[index]);
        UpdateUI();
    }

    public void RemoveLastSkill()
    {
        if (state != GameState.Selecting) return;
        if (combo.Count == 0) return;

        combo.RemoveAt(combo.Count - 1);
        UpdateUI();
    }

    public void ClearCombo()
    {
        if (state != GameState.Selecting) return;
        if (combo.Count == 0) return;

        combo.Clear();
        UpdateUI();
    }

    public void ConfirmSelection()
    {
        if(state != GameState.Selecting) return;
        if(combo.Count == 0)
        {
            errorText.text = "Any Skill is not selected!";
            UpdateUI();
            return;
        }

        state = GameState.Executing;
        ExecuteAction();
    }

    void AutoConfirm()
    {
        if (combo.Count == 0)
        {
            errorText.text = "Time over! No skill selected.";
            combo.Clear();
            UpdateUI();

            selectTimer = selectTimeLimit;
            return;
        }

        errorText.text = "Time over! Confirmed skill selection.";
        ConfirmSelection();
    }

    void ExecuteAction()
    {
        if(state != GameState.Executing) return;

        sumDamge = CalculateDamage();
        enemyHP -= sumDamge;
        combo.Clear();

        if(enemyHP <= 0)
        {
            enemyHP = 0;
            
            FinishGame();

            return;
        }

        UpdateUI();
        state = GameState.Selecting;
        selectTimer = selectTimeLimit;
    }

    void FinishGame()
    {
        state = GameState.Result;
        finalScore = CalculateScore();

        UpdateUI();

        state = GameState.Finished;
    }

    bool MatchCombo(Skill[] pattern)
    {
        if (combo.Count < pattern.Length) return false;

        for (int j = 0; j <= combo.Count - pattern.Length; j++)
        {
            bool match = true;

            for (int k = 0; k < pattern.Length; k++)
            {
                if (combo[j + k] != pattern[k])
                {
                    match = false;
                    break;
                }
            }

            if (match) return true;
        }

        return false;
    }
    int CalculateDamage()
    {
        int damage = 0;
        foreach (var c in SPcomboList)
        {
            if (MatchCombo(c.pattern))
            {
                damage += c.damage;
            }
        }

        foreach (var skill in combo)
        {
            damage += skill.damage;
        }

        return damage;
    }

    int CalculateScore()
    {
        float hpRatio = (float)(MaxEnemyHP - enemyHP) / MaxEnemyHP;
        float timeRatio = battleTimer / battleTimeLimit;
        float score = hpRatio * timeRatio;

        return Mathf.RoundToInt(score * 10000);
    }

    void UpdateUI()
    {
        comboNumText.text = combo.Count + "/" +  MaxComboNum;
        if (combo.Count == 0)
        {
            selectedSkillsText.text = "No Skill";
        }
        else
        {
            selectedSkillsText.text = "";
            for(int i = 0; i < combo.Count; i++){
                selectedSkillsText.text += $"{i + 1}: {combo[i].name}\n";
            }
        }

        enemyText.text = "Enemy HP: " + enemyHP;

        if (state == GameState.Selecting)
        {
            if (combo.Count == 0) ;
            else errorText.text = "";
        }
        else
        {
            selectTimerText.text = "";
        }

        if (state == GameState.Executing)
        {
            damageText.text = sumDamge + " damge";
        }
        else
        {
            damageText.text = "";
        }

        if (state == GameState.Result)
        {
            resultText.text = "SCORE: " + finalScore;
        }

    }

    void UpdateTimerUI(TextMeshProUGUI timerText, float timer)
    {
        timerText.text = Mathf.Ceil(timer).ToString();
    }
}