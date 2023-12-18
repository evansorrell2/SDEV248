using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance {get; private set;}

    public Image mask;
    float originalSize;
    bool hasStarted;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        hasStarted = false; //checking if the player has begun playing
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value) // value is the percentage that the bar will be set to
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
