using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class BattleManager : MonoBehaviour
{
    public NoteData key;
    public AudioSource[] audioSources;
    int audioIndex = 0;
    public AudioClip clickSound;
    double nextTickTime;
    bool isMetronomeRunning;

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
    int ProgressionScoreBonus;
    int finalScore;
    int sumDamage;

    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI turnCountText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI beatText;
    public TextMeshProUGUI phaseTimerText;
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI selectedChordsText;
    public TextMeshProUGUI progressionNumText;
    public TextMeshProUGUI nextChoiceText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI ModifierText;

    public UnityEngine.UI.Button confirmButton;
    
    Chord C, Dm, Em, F, G, Am, Bdim;
    List<Chord> chords = new List<Chord>();


    int MaxProgressionNum = 8;
    List<Chord> progression = new List<Chord>();
    List<DegreeProgression> DegreeProgressionList = new List<DegreeProgression>();
    

    List<Chord> currentChoices = new List<Chord>();
    Chord nextChoice;
    public int choiceCount = 5;
    public GameObject chordButtonPrefab;
    public Transform chordButtonParent;

    List<FProgression> FProgressionList = new List<FProgression>();

    string Modifier = "";

    GameState Gstate;
    FinishState Fstate;

    void Awake()
    {
        key = NoteData.C;

        chords = ChordManager.CreateTriadDiatonic(key);
        DegreeProgressionList = ProgressionManager.CreateDegreeProgression();


        FProgressionList = new List<FProgression>()
        {
            new FProgression
            {
                name = "SD-D-T",
                pattern = new []
                { ChordFunction.SD, ChordFunction.D, ChordFunction.T },
                damage = 40
            },
            new FProgression
            {
                name = "Deceptive Cadence (to D)",
                pattern = new []
                { ChordFunction.D, ChordFunction.D },
                damage = 20
            },
            new FProgression
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
        Gstate = GameState.Start;
        InitGame();
    }

    void Update()
    {
        if (isMetronomeRunning)
        {
            double dspTime = AudioSettings.dspTime;

            while (nextTickTime < dspTime + 0.5)
            {
                AudioSource src = audioSources[audioIndex];

                src.clip = clickSound;
                src.PlayScheduled(nextTickTime);

                audioIndex = (audioIndex + 1) % audioSources.Length;

                nextTickTime += beatDuration;
            }
        }

        if (AudioSettings.dspTime < phaseStartTime)
        {
            return;
        }

        if (Gstate == GameState.Start)
        {
            float remain = GetRemainingTime();
            UpdateCountDownUI();

            if(remain <= 0f)
            {
                countDownText.text = "";
                StartSelectingPhase();
            }
            
        }
        else if (Gstate == GameState.Selecting)
        {
            float remain = GetRemainingTime();
            UpdateBeatUI();

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
            UpdateBeatUI();

            if (remain <= 0f)
            {
                StartCalculatingPhase();
            }
        }
        else if (Gstate == GameState.Calculating)
        {
            float remain = GetRemainingTime();
            UpdateBeatUI();

            if (remain <= 0f)
            {
                StartExecutingPhase();
            }
        }
        else if (Gstate == GameState.Executing)
        {
            float remain = GetRemainingTime();
            UpdateCountDownUI();

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
        ProgressionScoreBonus = 0;
        finalScore = 0;
        sumDamage = 0;

        countDownText.text = "";
        beatText.text = "";
        nextChoiceText.text = "";
        damageText.text = "";
        ModifierText.text = "";
        errorText.text = "";
        resultText.text = "";

        
        InitMusicTiming();
        Fstate = FinishState.Unfinish;
        
        StartPhase(measureDuration * 4);
        GenerateChoices();
        GenerateChordButtons();

        StartMetronome(phaseStartTime);
        UpdateUI();
    }

    public void SelectChord(int index)
    {
        if(Gstate != GameState.Selecting) return;
        if(progression.Count >= MaxProgressionNum) {
            errorText.text = "do not select chords over this";
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
        }

        confirmRemainTime = GetRemainingTime();
        FastSelectDamageBonus = Mathf.RoundToInt(confirmRemainTime);
        FastSelectScoreBonus += Mathf.RoundToInt(confirmRemainTime);

        isConfirmed = true;
        Gstate = GameState.Waiting;
    }

    void StartMetronome(double startTime)
    {
        nextTickTime = startTime;
        isMetronomeRunning = true;
    }

    void InitMusicTiming()
    {
        beatDuration = 60f / BPM;
        measureDuration = beatDuration * 4f;
    }
    void StartPhase(float duration)
    {
        phaseStartTime = AudioSettings.dspTime + 0.1;
        phaseDuration = duration;
    }
    void NextPhase(float duration)
    {
        phaseStartTime += phaseDuration;
        phaseDuration = duration;
    }
    void StartSelectingPhase()
    {
        Gstate = GameState.Selecting;
        NextPhase(measureDuration * 4);
        UpdateUI();
    }
    void StartCalculatingPhase()
    {
        Gstate = GameState.Calculating;
        NextPhase(measureDuration * 4);
        sumDamage = BattleCalculator.CalculateDamage(
        progression,
        DegreeProgressionList,
        FProgressionList,
        FastSelectDamageBonus,
        ref ProgressionScoreBonus,
        ref Modifier
    );
        UpdateUI();
    }
    void StartExecutingPhase()
    {
        Gstate = GameState.Executing;
        NextPhase(measureDuration * 2);
        ExecuteAction();
        GenerateChoices();
        GenerateChordButtons();
        UpdateUI();
    }
    float GetRemainingTime()
    {
        double elapsed = AudioSettings.dspTime - phaseStartTime;
        float remain = phaseDuration - (float)elapsed;

        return Mathf.Max(0f, remain);
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

        enemyHP -= sumDamage;
        Debug.Log("DAMAGE: " + sumDamage);
        progression.Clear();
        

        if(enemyHP <= 0)
        {
            Fstate = FinishState.CREAR;
            enemyHP = 0;
            FinishGame();
            return;
        }
        else if (currentTurn >= maxTurn)
        {
            Fstate = FinishState.TURNOVER;
            FinishGame();
            return;
        }
    }

    void FinishGame()
    {
        UpdateUI();
        Gstate = GameState.Result;
        finalScore = BattleCalculator.CalculateScore(
            Fstate,
            maxTurn,
            currentTurn,
            FastSelectScoreBonus,
            ProgressionScoreBonus
        );

        UpdateUI();
        Gstate = GameState.Finished;
    }

    void NextTurn()
    {
        currentTurn++;
        
        Modifier = "";
        /*DegreeProgressionCounter = 0;
        FProgressionCounter = 0;*/
        progression.Clear();

        UpdateUI();
        StartSelectingPhase();
    }

    /*int CalculateScore()
    {   
        float score = 0;
        if (Fstate == FinishState.CREAR)
        {
            score += (maxTurn - currentTurn + 1) * 2000;
            score += FastSelectScoreBonus * 20;
        }
        score += ProgressionScoreBonus * 50;
        

        return Mathf.RoundToInt(score);
    }*/

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
            countDownText.text = "";
        }
        else
        {
            phaseTimerText.text = "";
        }

        if (Gstate == GameState.Calculating)
        {
            phaseText.text = "Playing:";
        }

        if (Gstate == GameState.Executing)
        {
            beatText.text = "";
            phaseText.text = "Preparing:";
            damageText.text = sumDamage + " damage";
            if (Modifier != "") ModifierText.text = "BornusDamage\n" + Modifier;
        }

        if (Gstate == GameState.Result)
        {
            beatText.text = "";
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
        timerText.text = Mathf.Floor(timer).ToString();
    }
    void UpdateCountDownUI()
    {
        double elapsed = AudioSettings.dspTime - phaseStartTime;

        int beat = Mathf.FloorToInt((float)((phaseDuration - elapsed) / beatDuration));
        int count = beat / 2 + 1;
        if (count <= 3)
        {
            countDownText.text = $"{count}";
        }
    }
    void UpdateBeatUI()
    {
        double elapsed = AudioSettings.dspTime - phaseStartTime;

        int beat = Mathf.FloorToInt((float)(elapsed / beatDuration));
        int currentMeasure = beat / 4 + 1;
        int beatInMeasure = beat % 4 + 1;

        beatText.text = $"{currentMeasure}:  {beatInMeasure}";
    }
}