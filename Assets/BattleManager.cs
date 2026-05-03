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

    int MaxEnemyHP = 1000;
    public int enemyHP;
    public int finalScore;
    public int sumDamage;

    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI selectedSkillsText;
    public TextMeshProUGUI comboNumText;
    public TextMeshProUGUI nextChoiceText;
    public TextMeshProUGUI selectTimerText;
    public TextMeshProUGUI battleTimerText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText;

    public UnityEngine.UI.Button confirmButton;
    
    class Skill
    {
        public string name;
        public int damage;
        public ElementType element;
    }
    Skill attack, fire, ice, wind, 
          volcano, blizzard, hurricane,
          explosion, freeze, tornado;
    List<Skill> skills = new List<Skill>();

    public enum ElementType
    {
        normal, fire, ice, wind
    }

    int MaxComboNum = 5;
    List<Skill> combo = new List<Skill>();

    List<Skill> currentChoices = new List<Skill>();
    Skill nextChoice;
    public int choiceCount = 5;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;

    class SPComboData
    {
        public string name;
        public Skill[] pattern;
        public int multiplier;
    }
    List<SPComboData> SPcomboList = new List<SPComboData>();

    class EComboData
    {
        public string name;
        public ElementType[] pattern;
        public int damage; 
    }
    List<EComboData> EComboList = new List<EComboData>();

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
        attack = new Skill { name = "Attack", damage = 10, element = ElementType.normal };
        fire = new Skill { name = "Fire", damage = 20, element = ElementType.fire };
        ice = new Skill { name = "Ice", damage = 20, element = ElementType.ice };
        wind = new Skill { name = "Wind", damage = 20, element = ElementType.wind };
        volcano = new Skill { name = "Volcano", damage = 40, element = ElementType.fire };
        blizzard = new Skill { name = "Blizzard", damage = 40, element = ElementType.ice };
        hurricane = new Skill { name = "Hurricane", damage = 40, element = ElementType.wind };
        explosion = new Skill { name = "Explosion", damage = 70, element = ElementType.fire };
        freeze = new Skill { name = "Freeze", damage = 70, element = ElementType.ice };
        tornado = new Skill { name = "Tornado", damage = 70, element = ElementType.wind };


        skills = new List<Skill>()
        {
            attack, fire, ice, wind, 
            volcano, blizzard, hurricane,
            explosion, freeze, tornado
        };

        SPcomboList = new List<SPComboData>()
        {
            new SPComboData
            {
                name = "HotWind",
                pattern = new [] { fire, wind },
                multiplier = 2
            },
            new SPComboData
            {
                name = "God of Element",
                pattern = new [] { explosion, freeze, hurricane },
                multiplier = 3
            }
        };

        EComboList = new List<EComboData>()
        {
            new EComboData
            {
                name = "Full Fire",
                pattern = new []
                { ElementType.fire, ElementType.fire, ElementType.fire, ElementType.fire, ElementType.fire },
                damage = 300
            },
            new EComboData
            {
                name = "Full Ice",
                pattern = new []
                { ElementType.ice, ElementType.ice, ElementType.ice, ElementType.ice, ElementType.ice },
                damage = 300
            },
            new EComboData
            {
                name = "Full Wind",
                pattern = new []
                { ElementType.wind,  ElementType.wind, ElementType.wind, ElementType.wind, ElementType.wind },
                damage = 300
            },
            new EComboData
            {
                name = "Double Attack",
                pattern = new []
                { ElementType.normal, ElementType.normal },
                damage = 100
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

            HandleNumberInput();
            ConfirmSpace();
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
        sumDamage = 0;

        nextChoiceText.text = "";
        damageText.text = "";
        errorText.text = "";
        resultText.text = "";

        battleTimer = battleTimeLimit;

        state = GameState.Selecting;
        selectTimer = selectTimeLimit;

        GenerateChoices();
        GenerateSkillButtons();
        
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
        combo.Add(currentChoices[index]);
        currentChoices.RemoveAt(index);
        RefillChoices();
        GenerateSkillButtons();

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

    void GenerateChoices()
    {
        currentChoices.Clear();

        List<Skill> pool = new List<Skill>(skills);

        for (int i = 0; i < choiceCount; i++)
        {
            if (pool.Count == 0) break;

            int rand = Random.Range(0, pool.Count);
            currentChoices.Add(pool[rand]);
            pool.RemoveAt(rand);
        }

        if (pool.Count > 0)
        {
            nextChoice = pool[Random.Range(0, pool.Count)];
        }
        else
        {
            nextChoice = null;
        }
    }

    void GenerateSkillButtons()
    {
        // 既存ボタン削除
        foreach (Transform child in skillButtonParent)
        {
            Destroy(child.gameObject);
        }

        // 新規生成
        for (int i = 0; i < currentChoices.Count; i++)
        {
            int index = i;

            GameObject btn = Instantiate(skillButtonPrefab, skillButtonParent);

            // テキスト設定
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            text.text = currentChoices[i].name;

            // ボタンイベント設定
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                SelectSkill(index);
            });
        }

        nextChoiceText.text = nextChoice.name;
    }

    void RefillChoices()
    {
        List<Skill> pool = skills.Except(currentChoices).ToList();

        int rand = Random.Range(0, pool.Count);
        currentChoices.Add(pool[rand]);
        pool.RemoveAt(rand);

        if (pool.Count > 0)
        {
            nextChoice = pool[Random.Range(0, pool.Count)];
        }
        else
        {
            nextChoice = null;
        }
    }

    void HandleNumberInput()
    {
        for (int i = 0; i < currentChoices.Count; i++)
        {
            // Alpha1〜Alpha5（上の数字キー）
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) ||
            Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SelectSkill(i);
            }
        }
    }

    void ConfirmSpace()
    {
        if (combo.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ConfirmSelection();
                }
            }
    }

    void ExecuteAction()
    {
        if(state != GameState.Executing) return;

        sumDamage = CalculateDamage();
        enemyHP -= sumDamage;
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

        GenerateChoices();
        GenerateSkillButtons();
    }

    void FinishGame()
    {
        state = GameState.Result;
        finalScore = CalculateScore();

        UpdateUI();

        state = GameState.Finished;
    }

    bool MatchSPCombo(Skill[] pattern)
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
    bool MatchECombo(ElementType[] pattern)
    {
        if (combo.Count < pattern.Length) return false;

        for (int j = 0; j <= combo.Count - pattern.Length; j++)
        {
            bool match = true;

            for (int k = 0; k < pattern.Length; k++)
            {
                if (combo[j + k].element != pattern[k])
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
            if (MatchSPCombo(c.pattern))
            {
                damage += CalculateSPComboModifier(c);
            }
        }

        foreach (var c in EComboList)
        {
            if (MatchECombo(c.pattern))
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
    int CalculateSPComboModifier(SPComboData c)
    {
        int SPComboModifier = c.pattern.Sum(s => s.damage);
        SPComboModifier *= c.multiplier;

        return SPComboModifier;
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

        confirmButton.interactable = combo.Count > 0;

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
            if (combo.Count != 0) errorText.text = "";
        }
        else
        {
            selectTimerText.text = "";
        }

        if (state == GameState.Executing)
        {
            damageText.text = sumDamage + " damage";
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