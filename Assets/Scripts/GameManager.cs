using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static bool Scroll = true;
    
    public float stepSize = 0.5f;
    public float fallSize = 0.2f;
    public float scrollSpeed = 0.5f;
    public float fireSpeed = 1;
    public float timeToNextInput = 0.5f;
    public Image upperBound;
    public Image lowerBound;
    public Image fire;
    public InputLetter[] letters;
    
    public Color correct = Color.green;
    public Color wrong = Color.red;
    public Color press = Color.yellow;
    public Color disabled = Color.gray;

    public Image couple;
    public Transform coupleTop;
    public Transform coupleBottom;
    public TextMeshProUGUI setScroll;
    
    private float currentAcceleration = 1;
    private int topLetter = 0;
    private int currentLetter = -1;

    private void Start()
    {
        RandomizeLetter();
        setScroll.text = Scroll ? "Disable Gravity" : "Enable Gravity";
    }

    // Update is called once per frame
    private void Update()
    {
        fire.rectTransform.sizeDelta += Vector2.up * (fireSpeed * Time.deltaTime);
        if(Scroll)
            couple.transform.position += Vector3.down * (scrollSpeed * currentAcceleration * Time.deltaTime);
        if (currentLetter >= 0 && Input.GetKeyDown(letters[currentLetter].code))
        {
            for (int i = 0; i < letters.Length; i++)
            {
                if (i > topLetter)
                {
                    letters[i].buttonAi.color = disabled;
                    letters[i].buttonUser.color = disabled;
                }
                else
                {
                    letters[i].buttonAi.color = Color.white;
                    letters[i].buttonUser.color = Color.white;
                }
            }
            letters[currentLetter].buttonUser.color = correct;
            couple.transform.localScale = Vector3.one;
            couple.transform.position +=  Vector3.up * (stepSize * currentAcceleration);
            currentLetter = -1;
            Invoke(nameof(RandomizeLetter), timeToNextInput / currentAcceleration);
        }

        if (topLetter+1 < letters.Length && coupleTop.transform.position.y > letters[topLetter+1].buttonAi.transform.position.y)
            topLetter++;
        if (coupleBottom.transform.position.y < lowerBound.transform.position.y)
            SceneManager.LoadScene(0);
        if (coupleTop.transform.position.y > upperBound.transform.position.y)
            SceneManager.LoadScene(1);
        
        // if (height > 0)
        //     currentAcceleration = Mathf.Pow(acceleration, height);
        // else
        //     currentAcceleration = 1;
    }
    
    private void RandomizeLetter()
    {
        for (var i = 0; i < letters.Length; i++)
        {
            if (i > topLetter)
            {
                letters[i].buttonAi.color = disabled;
                letters[i].buttonUser.color = disabled;
            }
            else
            {
                letters[i].buttonAi.color = Color.white;
                letters[i].buttonUser.color = Color.white;
            }
        }
        couple.transform.localScale = new Vector3(-1,1,1);
        currentLetter = Random.Range(0, topLetter + 1);
        letters[currentLetter].buttonAi.color = Color.yellow;
    }

    public void ToggleGravity()
    {
        Scroll = !Scroll;
        setScroll.text = Scroll ? "Disable Gravity" : "Enable Gravity";
    }
    
    [Serializable]
    public struct InputLetter
    {
        public KeyCode code;
        public Image buttonAi;
        public Image buttonUser;
    }
}
