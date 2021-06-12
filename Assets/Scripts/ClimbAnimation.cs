using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClimbAnimation : MonoBehaviour {

    public Sprite[] spritesRight;
    public Sprite[] spritesLeft;
    public Sprite jump;
    public bool isLeft;
    public int spritePerFrame = 6;
    public Image image;

    private int index = 0;
    private int frame = 0;

    void Reset() {
        image = GetComponent<Image> ();
    }

    void Update ()
    {
        var sprites = isLeft ? spritesLeft : spritesRight;
        if (index+1 >= sprites.Length) return;
        frame ++;
        if (frame < spritePerFrame) return;
        index ++;
        image.sprite = sprites [index];
        frame = 0;
    }

    public void SetSide(bool left)
    {
        isLeft = left;
        frame = 0;
        index = -1;
        image.sprite = jump;
    }
}