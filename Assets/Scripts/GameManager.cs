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
    public float acceleration = 1.2f;
    public float scrollSpeed = 0.5f;
    public float fireSpeed = 1;
    public float timeToNextInput = 0.5f;
    public Image upperBound;
    public Image lowerBound;
    public Image fire;
    public InputLetter[] letters;

    public Sprite regular;
    public Sprite pressed;
    public Sprite correct;
    public Color wrong = Color.red;

    public Image couple;
    public Transform coupleTop;
    public Transform coupleBottom;
    public TextMeshProUGUI setScroll;

    private ClimbAnimation _climbAnimation;
    private float currentAcceleration = 1;
    private int topLetter = 3;
    private int currentLetter = -1;

    private void Awake()
    {
        _climbAnimation = couple.GetComponent<ClimbAnimation>();
    }

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
        for (int i = 0; i < letters.Length; i++)
        {
            if(Input.GetKeyDown(letters[i].code))
                Press(i);
        }

        if (topLetter + 1 < letters.Length &&
            coupleTop.transform.position.y > letters[topLetter + 1].buttonAi.transform.position.y)
        {
            currentAcceleration *= acceleration;
            topLetter++;
        }

        if (coupleBottom.transform.position.y < lowerBound.transform.position.y)
            SceneManager.LoadScene(0);
        if (coupleTop.transform.position.y > upperBound.transform.position.y)
            SceneManager.LoadScene(1);
        
        // if (height > 0)
        //     currentAcceleration = Mathf.Pow(acceleration, height);
        // else
        //     currentAcceleration = 1;
    }

    public void Press(int index)
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonUser.sprite = regular;
            letters[i].buttonUser.color = Color.white;
        }
        letters[index].buttonUser.sprite = pressed;
        if (index == currentLetter)
        {
            Succeed();
        }
        else
        {
            letters[index].buttonUser.color = wrong;
            couple.transform.position +=  Vector3.down * (fallSize * currentAcceleration);
        }
    }
    
    private void Succeed()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonAi.sprite = regular;
            letters[i].buttonUser.sprite = regular;
        }
        letters[currentLetter].buttonUser.sprite = correct;
        letters[currentLetter].buttonAi.sprite = correct;
        // couple.transform.localScale = Vector3.one;
        _climbAnimation.SetSide(true);
        couple.transform.position +=  Vector3.up * (stepSize * currentAcceleration);
        currentLetter = -1;
        Invoke(nameof(RandomizeLetter), timeToNextInput / currentAcceleration);
    }
    
    private void RandomizeLetter()
    {
        for (var i = 0; i < letters.Length; i++)
        {
            letters[i].buttonAi.sprite = regular;
            letters[i].buttonUser.sprite = regular;
            letters[i].buttonUser.color = Color.white;
        }
        currentLetter = Random.Range(0, topLetter + 1);
        letters[currentLetter].buttonAi.sprite = pressed;
        _climbAnimation.SetSide(false);
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
