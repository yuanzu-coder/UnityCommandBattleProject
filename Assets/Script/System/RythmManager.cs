using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public float BPM = 120f;
    public int Meter = 4;
    
    public float BeatDuration { get; private set;} 
    public float MeasureDuration { get; private set;}
    public int BeatInMeasure {get; private set;}
    public int CurrentMeasure {get; private set;}
    public int Count {get; private set;}

    public double CurrentDSPTime
        => AudioSettings.dspTime;

    void Awake()
    {
        SetBPM(BPM);
    }

    public void SetBPM(float bpm)
    {
        BPM = bpm;

        BeatDuration = 60f/ BPM;
        MeasureDuration = BeatDuration * Meter;
    }

    public void SetTimeSignature(int meter)
    {
        Meter = meter;

        BeatDuration = 60f/ BPM;
        MeasureDuration = BeatDuration * Meter;
    }

    public void SetGameStartTime(
        ref double phaseStartTime
    )
    {
        phaseStartTime = CurrentDSPTime + 0.1;
    }

    public float GetPhaseDuration(
        GameState Gstate
    )
    {
        int measureNum;
        if(Gstate == GameState.Preparing) measureNum = 2;
        else if(Gstate == GameState.Selecting) measureNum = 4;
        else if(Gstate == GameState.Calculating) measureNum = 4;
        else measureNum = 0;

        return MeasureDuration * measureNum;
    }

    public void NextPhase(
        ref double phaseStartTime,
        ref float phaseDuration,
        GameState Gstate
    )
    {
        phaseStartTime += phaseDuration;
        phaseDuration = GetPhaseDuration(Gstate);
    }

    public float GetRemainingTime(
        double phaseStartTime,
        float phaseDuration
    )
    {
        double elapsed = CurrentDSPTime - phaseStartTime;
        float remain = phaseDuration - (float)elapsed;

        return Mathf.Max(0f, remain);
    }

    public void CalcCountDown(
        double phaseStartTime,
        float phaseDuration,
        GameState Gstate
    )
    {
        double elapsed = CurrentDSPTime - phaseStartTime;

        int beat = Mathf.FloorToInt((float)((phaseDuration - elapsed) / BeatDuration));
        Count = beat / 2 + 1;
    }

    public void CalcBeat(
        double phaseStartTime
    )
    {
        double elapsed = CurrentDSPTime - phaseStartTime;

        int beat = Mathf.FloorToInt((float)(elapsed / BeatDuration));
        CurrentMeasure = beat / 4 + 1;
        BeatInMeasure = beat % 4 + 1;
    }
}
