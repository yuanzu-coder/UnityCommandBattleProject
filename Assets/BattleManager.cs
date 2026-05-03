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
    public TextMeshProUGUI selectedChordsText;
    public TextMeshProUGUI progressionNumText;
    public TextMeshProUGUI nextChoiceText;
    public TextMeshProUGUI selectTimerText;
    public TextMeshProUGUI battleTimerText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText;

    public UnityEngine.UI.Button confirmButton;
    
    class Chord
    {
        public string name;
        public int damage;
        public ChordFunction element;
    }
    Chord attack, fire, ice, wind, 
          volcano, blizzard, hurricane,
          explosion, freeze, tornado;
    List<Chord> chords = new List<Chord>();

    public enum ChordFunction
    {
        normal, fire, ice, wind
    }

    int MaxProgressionNum = 5;
    List<Chord> progression = new List<Chord>();

    List<Chord> currentChoices = new List<Chord>();
    Chord nextChoice;
    public int choiceCount = 5;
    public GameObject chordButtonPrefab;
    public Transform chordButtonParent;

    class SPProgressionData
    {
        public string name;
        public Chord[] pattern;
        public int multiplier;
    }
    List<SPProgressionData> SPprogressionList = new List<SPProgressionData>();

    class FProgressionData
    {
        public string name;
        public ChordFunction[] pattern;
        public int damage; 
    }
    List<FProgressionData> FProgressionList = new List<FProgressionData>();

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
        //Chordの宣言
        attack = new Chord { name = "Attack", damage = 10, element = ChordFunction.normal };
        fire = new Chord { name = "Fire", damage = 20, element = ChordFunction.fire };
        ice = new Chord { name = "Ice", damage = 20, element = ChordFunction.ice };
        wind = new Chord { name = "Wind", damage = 20, element = ChordFunction.wind };
        volcano = new Chord { name = "Volcano", damage = 40, element = ChordFunction.fire };
        blizzard = new Chord { name = "Blizzard", damage = 40, element = ChordFunction.ice };
        hurricane = new Chord { name = "Hurricane", damage = 40, element = ChordFunction.wind };
        explosion = new Chord { name = "Explosion", damage = 70, element = ChordFunction.fire };
        freeze = new Chord { name = "Freeze", damage = 70, element = ChordFunction.ice };
        tornado = new Chord { name = "Tornado", damage = 70, element = ChordFunction.wind };


        chords = new List<Chord>()
        {
            attack, fire, ice, wind, 
            volcano, blizzard, hurricane,
            explosion, freeze, tornado
        };

        SPprogressionList = new List<SPProgressionData>()
        {
            new SPProgressionData
            {
                name = "HotWind",
                pattern = new [] { fire, wind },
                multiplier = 2
            },
            new SPProgressionData
            {
                name = "God of Element",
                pattern = new [] { explosion, freeze, hurricane },
                multiplier = 3
            }
        };

        FProgressionList = new List<FProgressionData>()
        {
            new FProgressionData
            {
                name = "Full Fire",
                pattern = new []
                { ChordFunction.fire, ChordFunction.fire, ChordFunction.fire, ChordFunction.fire, ChordFunction.fire },
                damage = 300
            },
            new FProgressionData
            {
                name = "Full Ice",
                pattern = new []
                { ChordFunction.ice, ChordFunction.ice, ChordFunction.ice, ChordFunction.ice, ChordFunction.ice },
                damage = 300
            },
            new FProgressionData
            {
                name = "Full Wind",
                pattern = new []
                { ChordFunction.wind,  ChordFunction.wind, ChordFunction.wind, ChordFunction.wind, ChordFunction.wind },
                damage = 300
            },
            new FProgressionData
            {
                name = "Double Attack",
                pattern = new []
                { ChordFunction.normal, ChordFunction.normal },
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
        GenerateChordButtons();
        
        UpdateUI();
    }

    public void SelectChord(int index)
    {
        if(state != GameState.Selecting) return;
        if(progression.Count >= MaxProgressionNum) {
            UpdateUI();
            errorText.text = "do not select chords over this" ;
            return;
        }
        progression.Add(currentChoices[index]);
        currentChoices.RemoveAt(index);
        RefillChoices();
        GenerateChordButtons();

        UpdateUI();
    }

    public void RemoveLastChord()
    {
        if (state != GameState.Selecting) return;
        if (progression.Count == 0) return;

        progression.RemoveAt(progression.Count - 1);
        UpdateUI();
    }

    public void ClearProgression()
    {
        if (state != GameState.Selecting) return;
        if (progression.Count == 0) return;

        progression.Clear();
        UpdateUI();
    }

    public void ConfirmSelection()
    {
        if(state != GameState.Selecting) return;
        if(progression.Count == 0)
        {
            errorText.text = "Any Chord is not selected!";
            UpdateUI();
            return;
        }

        state = GameState.Executing;
        ExecuteAction();
    }

    void AutoConfirm()
    {
        if (progression.Count == 0)
        {
            errorText.text = "Time over! No chord selected.";
            progression.Clear();
            UpdateUI();

            selectTimer = selectTimeLimit;
            return;
        }

        errorText.text = "Time over! Confirmed chord selection.";
        ConfirmSelection();
    }

    void GenerateChoices()
    {
        currentChoices.Clear();

        List<Chord> pool = new List<Chord>(chords);

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

    void GenerateChordButtons()
    {
        // 既存ボタン削除
        foreach (Transform child in chordButtonParent)
        {
            Destroy(child.gameObject);
        }

        // 新規生成
        for (int i = 0; i < currentChoices.Count; i++)
        {
            int index = i;

            GameObject btn = Instantiate(chordButtonPrefab, chordButtonParent);

            // テキスト設定
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            text.text = currentChoices[i].name;

            // ボタンイベント設定
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                SelectChord(index);
            });
        }

        nextChoiceText.text = nextChoice.name;
    }

    void RefillChoices()
    {
        List<Chord> pool = chords.Except(currentChoices).ToList();

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
                SelectChord(i);
            }
        }
    }

    void ConfirmSpace()
    {
        if (progression.Count > 0)
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
        progression.Clear();

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
        GenerateChordButtons();
    }

    void FinishGame()
    {
        state = GameState.Result;
        finalScore = CalculateScore();

        UpdateUI();

        state = GameState.Finished;
    }

    bool MatchSPProgression(Chord[] pattern)
    {
        if (progression.Count < pattern.Length) return false;

        for (int j = 0; j <= progression.Count - pattern.Length; j++)
        {
            bool match = true;

            for (int k = 0; k < pattern.Length; k++)
            {
                if (progression[j + k] != pattern[k])
                {
                    match = false;
                    break;
                }
            }

            if (match) return true;
        }

        return false;
    }
    bool MatchFProgression(ChordFunction[] pattern)
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

    int CalculateDamage()
    {
        int damage = 0;
        foreach (var c in SPprogressionList)
        {
            if (MatchSPProgression(c.pattern))
            {
                damage += CalculateSPProgressionModifier(c);
                Debug.Log("SP Progression: " + c.name);
            }
        }

        foreach (var c in FProgressionList)
        {
            if (MatchFProgression(c.pattern))
            {
                damage += c.damage;
                Debug.Log("E Progression: " + c.name);
            }
        }

        foreach (var chord in progression)
        {
            damage += chord.damage;
        }

        return damage;
    }
    int CalculateSPProgressionModifier(SPProgressionData c)
    {
        int SPProgressionModifier = c.pattern.Sum(s => s.damage);
        SPProgressionModifier *= c.multiplier;

        return SPProgressionModifier;
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
        progressionNumText.text = progression.Count + "/" +  MaxProgressionNum;

        confirmButton.interactable = progression.Count > 0;

        if (progression.Count == 0)
        {
            selectedChordsText.text = "No Chord";
        }
        else
        {
            selectedChordsText.text = "";
            for(int i = 0; i < progression.Count; i++){
                selectedChordsText.text += $"{i + 1}: {progression[i].name}\n";
            }
        }

        enemyText.text = "Enemy HP: " + enemyHP;

        if (state == GameState.Selecting)
        {
            if (progression.Count != 0) errorText.text = "";
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