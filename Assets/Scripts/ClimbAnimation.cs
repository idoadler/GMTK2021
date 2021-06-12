using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClimbAnimation : MonoBehaviour {

    public Sprite[] spritesRight;
    public Sprite[] spritesLeft;
    public Sprite jump;
    public bool isLeft;
    public float timePerFrame = 0.05f;
    public Image image;

    private int index = 0;
    private float timeToSwitch = 0;

    void Reset() {
        image = GetComponent<Image> ();
    }

    void Update ()
    {
        var sprites = isLeft ? spritesLeft : spritesRight;
        if (index + 1 >= sprites.Length) return;
        timeToSwitch -= Time.deltaTime;
        if (timeToSwitch > 0) return;
        index ++;
        image.sprite = sprites [index];
        timeToSwitch = timePerFrame;
        if (index + 1 >= sprites.Length)
            index = -1;
    }

    public void SetSide(bool left)
    {
        isLeft = left;        
        timeToSwitch = timePerFrame;
        index = -1;
        image.sprite = jump;
    }
}