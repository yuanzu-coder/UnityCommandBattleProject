using UnityEngine;

public class MetronomeManager : MonoBehaviour
{
    [SerializeField] RhythmManager rhythmManager;

    [SerializeField] AudioSource[] audioSources;
    [SerializeField] AudioClip clickSound;

    double nextTickTime;
    int audioIndex;

    bool isRunning;

    public void StartMetronome(double startTime)
    {
        nextTickTime = startTime;
        isRunning = true;
    }

    public void StopMetronome()
    {
        isRunning = false;
    }

    public void PlayMetronome()
    {
        if (isRunning)
        {
            double dspTime = rhythmManager.CurrentDSPTime;

            while (nextTickTime < dspTime + 0.1)
            {
                AudioSource src = audioSources[audioIndex];

                src.clip = clickSound;
                src.PlayScheduled(nextTickTime);

                audioIndex = (audioIndex + 1) % audioSources.Length;

                nextTickTime += rhythmManager.BeatDuration;
            }
        }
    }
}
