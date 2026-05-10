using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ChordButtonUI : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;

    public List<Chord> currentChoices = new List<Chord>();
    public Chord nextChoice {get; private set;}
    int choiceCount = 5;

    public GameObject chordButtonPrefab;
    public Transform chordButtonParent;

    public void GenerateChoices(
        List<Chord> chords
    )
    {
        currentChoices.Clear();

        List<Chord> pool = new List<Chord> (chords);


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

    public void UpdateChordButtons()
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
                battleManager.SelectChord(index);
            });
        }
    }

    public void RemoveChoice(
        int index
    )
    {
        currentChoices.RemoveAt(index);
    }

    public void RefillChoices(
        List<Chord> chords
    )
    {   
        if(nextChoice != null) currentChoices.Add(nextChoice);

        List<Chord> pool = new List<Chord> (chords.Except(currentChoices).ToList());
        if (pool.Count > 0)
        {
            nextChoice = pool[Random.Range(0, pool.Count)];
        }
        else
        {
            nextChoice = null;
        }
    }
}
