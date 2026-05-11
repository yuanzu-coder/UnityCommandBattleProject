using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class ChoiceManager : MonoBehaviour
{
    private List<Chord> currentChoices = new List<Chord>();
    public IReadOnlyList<Chord> CurrentChoices
        => currentChoices;
    public Chord nextChoice {get; private set;}
    int choiceCount = 5;

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

    public void RemoveChoice(
        int index
    )
    {
        currentChoices.RemoveAt(index);
    }
}
