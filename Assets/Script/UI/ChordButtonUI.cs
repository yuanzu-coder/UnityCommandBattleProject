using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ChordButtonUI : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;
    [SerializeField] ChoiceManager choiceManager;

    public GameObject chordButtonPrefab;
    public Transform chordButtonParent;

    public void UpdateChordButtons()
    {
        // 既存ボタン削除
        foreach (Transform child in chordButtonParent)
        {
            Destroy(child.gameObject);
        }

        // 新規生成
        for (int i = 0; i < choiceManager.CurrentChoices.Count; i++)
        {
            int index = i;

            GameObject btn = Instantiate(chordButtonPrefab, chordButtonParent);

            // テキスト設定
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            text.text = choiceManager.CurrentChoices[i].name;

            // ボタンイベント設定
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                battleManager.SelectChord(index);
            });
        }
    }
}
    
