using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] RhythmManager rhythmManager;

    [SerializeField] TextMeshProUGUI countDownText;
    [SerializeField] TextMeshProUGUI beatText;

    public void UpdateCountDownUI(
        int countStartNum
    )
    {
        if (rhythmManager.Count <= countStartNum)
        {
            countDownText.text = $"{rhythmManager.Count}";
        }
    }
    public void UpdateBeatUI()
    {
        beatText.text = $"{rhythmManager.CurrentMeasure}:  {rhythmManager.BeatInMeasure}";
    }
}