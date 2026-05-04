using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class BattleManager : MonoBehaviour
{
    public float BPM = 120f;
    float beatDuration;     // 1拍
    float measureDuration;  // 1小節
    double phaseStartTime;
    float phaseDuration;

    bool isConfirmed ;
    float confirmRemainTime;

    int maxTurn = 4;
    int currentTurn;

    int MaxEnemyHP = 1000;
    int enemyHP;
    int FastSelectDamageBonus;
    int FastSelectScoreBonus;
    int SPProgressionCounter;
    int FProgressionCounter;
    int ProgressionBonus;
    int finalScore;
    int sumDamage;

    public TextMeshProUGUI turnCountText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI selectedChordsText;
    public TextMeshProUGUI progressionNumText;
    public TextMeshProUGUI nextChoiceText;
    public TextMeshProUGUI phaseTimerText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI ModifierText;

    public UnityEngine.UI.Button confirmButton;
    
    class Chord
    {
        public string name;
        public int damage;
        public ChordFunction element;
    }
    Chord C, Dm, Em, F, G, Am, Bdim;
    List<Chord> chords = new List<Chord>();

    public enum ChordFunction
    {
        T, D, SD
    }

    int MaxProgressionNum = 8;
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

    string Modifier = "";

    public enum GameState
    {
        Selecting, Waiting, Executing, Result, Finished
    }
    GameState Gstate;

    public enum FinishState
    {
        Unfinish, CREAR, TIMEOVER, TURNOVER
    }
    FinishState Fstate;

    void Awake()
    {
        //Chordの宣言
        C = new Chord { name = "C", damage = 10, element = ChordFunction.T };
        Dm= new Chord { name = "Dm", damage = 10, element = ChordFunction.SD };
        Em = new Chord { name = "Em", damage = 10, element = ChordFunction.D };
        F = new Chord { name = "F", damage = 10, element = ChordFunction.SD };
        G = new Chord { name = "G", damage = 10, element = ChordFunction.D };
        Am = new Chord { name = "Am", damage = 10, element = ChordFunction.T };
        Bdim = new Chord { name = "Bm(-5)", damage = 10, element = ChordFunction.SD };


        chords = new List<Chord>()
        {
            C, Dm, Em, F, G, Am, Bdim
        };

        SPprogressionList = new List<SPProgressionData>()
        {
            new SPProgressionData
            {
                name = "Canon Progression (8 Chords)",
                pattern = new [] { C, G, Am, Em, F, C, Dm, G },
                multiplier = 7
            },
            new SPProgressionData
            {
                name = "Minor Canon Progression (8 Chords)",
                pattern = new [] { Am, Em, F, C, Dm, Am, Bdim, Em },
                multiplier = 7
            },
            new SPProgressionData
            {
                name = "Only-one Progression (8 Chords)",
                pattern = new [] { C, F, G, Em, Am, Dm, F, G },
                multiplier = 7
            },
            new SPProgressionData
            {
                name = "Canon Progression",
                pattern = new [] { C, G, Am, Em },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Only-one Progression",
                pattern = new [] { C, F, G, Em },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Ascending Prpgression (from Dm)",
                pattern = new [] { Dm, Em, F, G },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Royal-Road Progression",
                pattern = new [] { F, G, Em, Am },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Pop-Punk Progression",
                pattern = new [] { F, C, G, Am },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Just The Two of Us Progression",
                pattern = new [] { F, Em, Am, G },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Komuro's Progression",
                pattern = new [] { Am, F, G, C },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Minor Canon Progression",
                pattern = new [] { Am, Em, F, C },
                multiplier = 5
            },
            new SPProgressionData
            {
                name = "Two-Five-One Progression",
                pattern = new [] { Dm, G, C },
                multiplier = 4
            },
            new SPProgressionData
            {
                name = "Minor Two-Five-One Progression",
                pattern = new [] { Bdim, Em, Am },
                multiplier = 4
            }

        };

        FProgressionList = new List<FProgressionData>()
        {
            new FProgressionData
            {
                name = "SD-D-T",
                pattern = new []
                { ChordFunction.SD, ChordFunction.D, ChordFunction.T },
                damage = 40
            },
            new FProgressionData
            {
                name = "Deceptive Cadence (to D)",
                pattern = new []
                { ChordFunction.D, ChordFunction.D },
                damage = 20
            },
            new FProgressionData
            {
                name = "Deceptive Cadence (to SD)",
                pattern = new []
                { ChordFunction.D, ChordFunction.SD },
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
        if (Gstate == GameState.Selecting)
        {
            float remain = GetRemainingTime();
            UpdateTimerUI(phaseTimerText, remain);

            if (remain <= 0f)
            {
                AutoConfirm();
            }

            HandleNumberInput();
            ConfirmSpace();
        }
        else if (Gstate == GameState.Waiting)
        {
            float remain = GetRemainingTime();
            UpdateTimerUI(phaseTimerText, remain);

            if (remain <= 0f)
            {
                StartExecutingPhase();
            }
        }
        else if (Gstate == GameState.Executing)
        {
            float remain = GetRemainingTime();
            UpdateTimerUI(phaseTimerText, remain);

            if (remain <= 0f)
            {
                NextTurn();
            }
        }
    }

    public void InitGame()
    {
        isConfirmed = false;
        confirmRemainTime = 0f;
        FastSelectDamageBonus = 0;
        FastSelectScoreBonus = 0;
        currentTurn = 1;
        enemyHP = MaxEnemyHP;
        SPProgressionCounter = 0;
        FProgressionCounter = 0;
        ProgressionBonus = 0;
        finalScore = 0;
        sumDamage = 0;

        nextChoiceText.text = "";
        damageText.text = "";
        ModifierText.text = "";
        errorText.text = "";
        resultText.text = "";

        
        InitMusicTiming();
        Fstate = FinishState.Unfinish;

        StartSelectingPhase();
        GenerateChoices();
        GenerateChordButtons();
        
        UpdateUI();
    }

    public void SelectChord(int index)
    {
        if(Gstate != GameState.Selecting) return;
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
        if (Gstate != GameState.Selecting) return;
        if (progression.Count == 0) return;

        progression.RemoveAt(progression.Count - 1);
        UpdateUI();
    }

    public void ClearProgression()
    {
        if (Gstate != GameState.Selecting) return;
        if (progression.Count == 0) return;

        progression.Clear();
        UpdateUI();
    }

    public void ConfirmSelection()
    {
        if(Gstate != GameState.Selecting) return;
        if(progression.Count == 0)
        {
            errorText.text = "Any Chord is not selected!";
            UpdateUI();
        }

        confirmRemainTime = GetRemainingTime();
        FastSelectDamageBonus = Mathf.RoundToInt(confirmRemainTime);
        FastSelectScoreBonus += Mathf.RoundToInt(confirmRemainTime);

        isConfirmed = true;
        Gstate = GameState.Waiting;
    }

    void InitMusicTiming()
    {
        beatDuration = 60f / BPM;
        measureDuration = beatDuration * 4f;
    }
    void StartPhase(float duration)
    {
        phaseStartTime = AudioSettings.dspTime;
        phaseDuration = duration;
    }
    void StartSelectingPhase()
    {
        Gstate = GameState.Selecting;
        StartPhase(measureDuration * 4);
    }
    void StartExecutingPhase()
    {
        Gstate = GameState.Executing;
        StartPhase(measureDuration * 6);
        ExecuteAction();
    }
    float GetRemainingTime()
    {
        double elapsed = AudioSettings.dspTime - phaseStartTime;
        return Mathf.Max(0f, phaseDuration - (float)elapsed);
    }

    void AutoConfirm()
    {
        if (progression.Count == 0)
        {
            progression.Clear();
        }
        else
        {
            errorText.text = "Time over! Confirmed chord selection.";
        }
        
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
        if(nextChoice != null) currentChoices.Add(nextChoice);

        List<Chord> pool = chords.Except(currentChoices).ToList();
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
        if(Gstate != GameState.Executing) return;

        sumDamage = CalculateDamage();
        enemyHP -= sumDamage;
        progression.Clear();
        UpdateUI();
        
        Debug.Log("Bonus" + ProgressionBonus);

        if(enemyHP <= 0)
        {
            Fstate = FinishState.CREAR;
            enemyHP = 0;
            FinishGame();
            return;
        }
    }

    void FinishGame()
    {
        Gstate = GameState.Result;
        finalScore = CalculateScore();
        UpdateUI();

        Gstate = GameState.Finished;
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
                Debug.Log("SP: " + c.name);
                int CalcResult = CalculateSPProgressionModifier(c);
                damage += CalcResult;
                Modifier += $"SP: {c.name} (+{CalcResult})\n";
                SPProgressionCounter += c.pattern.Length;
            }
            if(SPProgressionCounter != 0) break;
        }
        
        if(SPProgressionCounter == 0)
        {
            foreach (var c in FProgressionList)
            {
                if (MatchFProgression(c.pattern))
                {
                    Debug.Log("E : " + c.name);
                    damage += c.damage;
                    Modifier += $"E : {c.name} (+{c.damage})\n";
                    FProgressionCounter++;
                }
            }
        }
        

        foreach (var chord in progression)
        {
            damage += chord.damage;
        }

        damage += FastSelectDamageBonus * 10;
        if(FastSelectDamageBonus != 0)
        {
            Modifier += $"Fast Select Bonus: +{FastSelectDamageBonus * 10}";
        }
        ProgressionBonus += 2 * SPProgressionCounter + FProgressionCounter;
        
        return damage;
    }
    int CalculateSPProgressionModifier(SPProgressionData c)
    {
        int SPProgressionModifier = c.pattern.Sum(s => s.damage);
        SPProgressionModifier *= c.multiplier;

        return SPProgressionModifier;
    }

    void NextTurn()
    {
        if (currentTurn >= maxTurn)
        {
            Fstate = FinishState.TURNOVER;
            FinishGame();
            return;
        }
        currentTurn++;
        
        Modifier = "";
        SPProgressionCounter = 0;
        FProgressionCounter = 0;

        StartSelectingPhase();
        GenerateChoices();
        GenerateChordButtons();
        UpdateUI();
    }

    int CalculateScore()
    {   
        float score = 0;
        if (Fstate == FinishState.CREAR)
        {
            score += (maxTurn - currentTurn + 1) * 2000;
            score += FastSelectScoreBonus * 20;
        }
        score += ProgressionBonus * 50;
        

        return Mathf.RoundToInt(score);
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

        if (Gstate == GameState.Selecting)
        {
            phaseText.text = "Composing:";
            if (progression.Count != 0) errorText.text = "";
            turnCountText.text = "Turn: " + currentTurn + "/" + maxTurn;
        }
        else
        {
            phaseTimerText.text = "";
        }

        if (Gstate == GameState.Executing)
        {
            phaseText.text = "Playing:";
            damageText.text = sumDamage + " damage";
            if (Modifier != "") ModifierText.text = "BornusDamage\n" + Modifier;
        }

        if (Gstate == GameState.Result)
        {
            switch (Fstate)
            {
                case FinishState.CREAR:
                    resultText.text = "SCORE: " + finalScore;
                    break;
                case FinishState.TIMEOVER:
                    resultText.text = "TIMEOVER\nSCORE: " + finalScore;
                    break;
                case FinishState.TURNOVER:
                    resultText.text = "TURNOVER\nSCORE: " + finalScore;
                    break;  
            }
        }

    }

    void UpdateTimerUI(TextMeshProUGUI timerText, float timer)
    {
        timerText.text = Mathf.Ceil(timer).ToString();
    }
}