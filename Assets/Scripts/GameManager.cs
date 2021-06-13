using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static bool Scroll = true;
    
    public float stepSize = 0.5f;
    public float stepTime = 0.2f;
    public float fallSize = 0.2f;
    public float fallTime = 0.1f;
    public float acceleration = 1.2f;
    public float scrollSpeed = 0.5f;
    public float fireSpeed = 1;
    public float timeToNextInput = 0.5f;
    public float fadeTime = 1;
    public TextMeshProUGUI timeText;
    private DateTime timer;
    public TextMeshProUGUI clicksText;
    private int clicks = 0;
    public Image upperBound;
    public Image lowerBound;
    public Image recordMarker;
    public Image fire;
    public Image blackFade;
    public InputLetter[] letters;
    public GameObject tutorial;

    public Sprite regular;
    public Sprite ai;
    public Sprite correct;
    public Sprite wrong;

    public Image couple;
    public Transform coupleTop;
    public Transform coupleBottom;

    public AudioClip[] slip;
    public AudioClip fireClip;
    public AudioClip[] voiceEffects;
    public AudioClip aiVoice;
    public AudioSource mainAudio;
    public AudioSource effects;
    public float distanceFromFireAlarm = 1;
    public AudioSource alarm;
    
    private ClimbAnimation _climbAnimation;
    private float currentAcceleration = 1;
    private int currentLetter = -1;

    private bool running = false;

    private void Awake()
    {
        _climbAnimation = couple.GetComponent<ClimbAnimation>();
    }

    private void Start()
    {
        StartCoroutine(StartScene(fadeTime));
        clicksText.text = "0";
        timeText.text = "0:00";
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKey && tutorial.activeSelf)
            EndTutorial();
        
        if(!running)
            return;

        var time = DateTime.Now - timer;
        timeText.text = time.ToString(@"m\:ss");
        
        fire.transform.localPosition += Vector3.up * (fireSpeed * Time.deltaTime);
        if(Scroll)
            couple.transform.position += Vector3.down * (scrollSpeed * currentAcceleration * Time.deltaTime);
        for (int i = 0; i < letters.Length; i++)
        {
            if(Input.GetKeyDown(letters[i].code))
                Press(i);
        }
        //
        // if (topLetter + 1 < letters.Length &&
        //     coupleTop.transform.position.y > letters[topLetter + 1].buttonAi.transform.position.y)
        // {
        //     currentAcceleration *= acceleration;
        //     topLetter++;
        // }

        if (coupleBottom.transform.position.y < lowerBound.transform.position.y)
        {
            _climbAnimation.Kill();
            alarm.clip = fireClip;
            alarm.Play();
            StopAllCoroutines();
            StartCoroutine(EndScene(5, 0));
            return;
        }

        if (coupleTop.transform.position.y > upperBound.transform.position.y)
        {
            // var record = PlayerPrefs.GetFloat("record", float.PositiveInfinity);
            // if (time.TotalSeconds < record)
            // {
                PlayerPrefs.SetFloat("record", (float) time.TotalSeconds);
                PlayerPrefs.SetInt("steps", clicks);
            // }

            StopAllCoroutines();
            StartCoroutine(EndScene(fadeTime, 1));
            return;
        }
        
        if (coupleTop.transform.position.y > recordMarker.transform.position.y)
            recordMarker.gameObject.SetActive(false);
        
        // if (height > 0)
        //     currentAcceleration = Mathf.Pow(acceleration, height);
        // else
        //     currentAcceleration = 1;

        var distFire = coupleBottom.transform.position.y - lowerBound.transform.position.y;
        if (distFire > distanceFromFireAlarm)
            alarm.volume = 0;
        else
            alarm.volume = 1 - distFire / distanceFromFireAlarm;
    }

    public void EndTutorial()
    {
        tutorial.SetActive(false);
        RandomizeLetter();
        running = true;
        timer = DateTime.Now;
    }
    
    public void Press(int index)
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonUser.sprite = regular;
        }
        effects.pitch = 0.5f + Random.value;
        if (index == currentLetter)
        {        
            effects.clip = aiVoice;
            Succeed();
        }
        else
        {
            effects.clip = slip[Random.Range(0, slip.Length)];;
            letters[index].buttonUser.sprite = wrong;
            StartCoroutine(Hop(-fallSize * currentAcceleration, fallTime));
        }
        effects.Play();
    }
    
    private void Succeed()
    {
        clicks++;
        clicksText.text = clicks.ToString();
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonAi.sprite = regular;
            letters[i].buttonUser.sprite = regular;
            letters[i].text.color = Color.black;
            letters[i].textUser.color = Color.black;
        }
        letters[currentLetter].text.color = Color.white;
        letters[currentLetter].textUser.color = Color.white;
        letters[currentLetter].buttonUser.sprite = correct;
        letters[currentLetter].buttonAi.sprite = correct;
        _climbAnimation.SetSide(true);
        StartCoroutine(Hop(stepSize * currentAcceleration, stepTime));
        currentLetter = -1;
        Invoke(nameof(RandomizeLetter), timeToNextInput / currentAcceleration);
    }
    
    private void RandomizeLetter()
    {
        for (var i = 0; i < letters.Length; i++)
        {
            letters[i].buttonAi.sprite = regular;
            letters[i].buttonUser.sprite = regular;
            letters[i].text.color = Color.black;
            letters[i].textUser.color = Color.black;
        }
        currentLetter = Random.Range(0, letters.Length);
        letters[currentLetter].text.color = Color.white;
        letters[currentLetter].buttonAi.sprite = ai;
        _climbAnimation.SetSide(false);
        effects.pitch = 0.5f + Random.value;
        effects.clip = voiceEffects[Random.Range(0, voiceEffects.Length)];
        effects.Play();
    }
    
    IEnumerator Hop(float hopHeight, float time) {
        var timer = 0.0f;
         
        while (timer <= 1) {
            var height = Mathf.Sin(Mathf.PI * (timer/2)) * hopHeight;
            couple.transform.position += Vector3.up * (height * Time.deltaTime); 
             
            timer += Time.deltaTime / time;
            yield return null;
        }
    }
    
    IEnumerator StartScene(float time)
    {
        running = false;
        var timer = 0.0f;
        blackFade.gameObject.SetActive(true);
        while (timer <= 1)
        {
            blackFade.color = new Color(0, 0, 0, 1-timer);
            timer += Time.deltaTime / time;
            yield return null;
        }
        blackFade.gameObject.SetActive(false);
    }
    
    IEnumerator EndScene(float time, int scene)
    {
        running = false;
        var timer = 0.0f;
        blackFade.gameObject.SetActive(true);
        while (timer <= 1)
        {
            mainAudio.volume = (1 - timer) * (1 - timer);
            blackFade.color = new Color(0, 0, 0, timer);
            timer += Time.deltaTime / time;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + scene);
    }
    
    [Serializable]
    public struct InputLetter
    {
        public KeyCode code;
        public Image buttonAi;
        public TextMeshProUGUI text;
        public TextMeshProUGUI textUser;
        public Image buttonUser;
    }
}
