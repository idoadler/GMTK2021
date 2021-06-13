using System;
using TMPro;
using UnityEngine;

public class RestoreScore : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI steps;
    public TextMeshProUGUI score;
    
    // Start is called before the first frame update
    void Start()
    {
        var seconds = PlayerPrefs.GetFloat("record", 100000f);
        time.text = TimeSpan.FromSeconds(seconds).ToString(@"m\:ss");
        steps.text = PlayerPrefs.GetInt("steps", 9999).ToString();
        var scoreSec = seconds - 30;
        if (scoreSec <= 0)
            scoreSec = 1;
        score.text = ((int)(100000f / scoreSec)).ToString();
    }
}
