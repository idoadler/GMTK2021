using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClimbAnimation : MonoBehaviour {

    public Sprite[] spritesRight;
    public Sprite[] spritesLeft;
    public Sprite jump;
    public Sprite burnt;
    public bool isLeft;
    public float framesPerSecond = 2.5f;
    public float jumpFps = 5f;
    public Image image;
    private bool killed = false;

    private int index = 0;
    private float timeToSwitch = 0;

    void Reset() {
        image = GetComponent<Image> ();
    }

    void Update ()
    {
        if(killed)
            return;
        var sprites = isLeft ? spritesLeft : spritesRight;
        if (index + 1 >= sprites.Length) return;
        timeToSwitch -= Time.deltaTime;
        if (timeToSwitch > 0) return;
        index ++;
        image.sprite = sprites [index];
        timeToSwitch = 1/framesPerSecond;
        if (index + 1 >= sprites.Length)
            index = -1;
    }

    public void SetSide(bool left)
    {
        if(killed)
            return;
        isLeft = left;        
        timeToSwitch = 1/jumpFps;
        index = -1;
        image.sprite = jump;
    }

    public void Kill()
    {
        image.sprite = burnt;
        killed = true;
    }
}