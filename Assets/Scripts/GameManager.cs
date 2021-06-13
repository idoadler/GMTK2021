using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Image upperBound;
    public Image lowerBound;
    public Image recordMarker;
    public Image fire;
    public InputLetter[] letters;

    public Sprite regular;
    public Sprite ai;
    public Sprite correct;
    public Sprite wrong;

    public Image couple;
    public Transform coupleTop;
    public Transform coupleBottom;
    public TextMeshProUGUI setScroll;

    public AudioClip slip;
    public AudioClip[] voiceEffects;
    public AudioClip[] stepEffects;
    public AudioSource effects;
    public float distanceFromFireAlarm = 1;
    public AudioSource alarm;
    
    private ClimbAnimation _climbAnimation;
    private float currentAcceleration = 1;
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
            SceneManager.LoadScene(0);
        if (coupleTop.transform.position.y > upperBound.transform.position.y)
            SceneManager.LoadScene(1);
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

    public void Press(int index)
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonUser.sprite = regular;
        }
        effects.pitch = 0.5f + Random.value;
        if (index == currentLetter)
        {        
            effects.clip = voiceEffects[Random.Range(0, voiceEffects.Length)];
            Succeed();
        }
        else
        {
            effects.clip = slip;
            letters[index].buttonUser.sprite = wrong;
            StartCoroutine(Hop(-fallSize * currentAcceleration, fallTime));
        }
        effects.Play();
    }
    
    private void Succeed()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].buttonAi.sprite = regular;
            letters[i].buttonUser.sprite = regular;
            letters[i].text.color = Color.black;
        }
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
        }
        currentLetter = Random.Range(0, letters.Length);
        letters[currentLetter].text.color = Color.white;
        letters[currentLetter].buttonAi.sprite = ai;
        _climbAnimation.SetSide(false);
        effects.pitch = 0.5f + Random.value;
        effects.clip = stepEffects[Random.Range(0, stepEffects.Length)];
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
        public TextMeshProUGUI text;
        public Image buttonUser;
    }
}
