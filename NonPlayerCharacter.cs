using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    private PlayerController player;
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    public GameObject dialogBox2;
    public GameObject dialogBox3;
    public GameObject dialogBox4;
    bool hasPurchased;
    float timerDisplay;
    int timesTalked;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        dialogBox.SetActive(false);
        dialogBox2.SetActive(false);
        dialogBox3.SetActive(false);
        dialogBox4.SetActive(false);
        timerDisplay = -1.0f;
        timesTalked = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
                dialogBox2.SetActive(false);
                dialogBox3.SetActive(false);
                dialogBox4.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        if((player.experience < 100) && (timesTalked <= 3)) {
            timerDisplay = displayTime;
            dialogBox.SetActive(true);
            timesTalked += 1;
        } else if ((player.experience < 100) && (timesTalked > 3)) {
            timerDisplay = displayTime;
            dialogBox2.SetActive(true);
            timesTalked += 1;
        } else if ((player.experience >= 100) && (hasPurchased == false)) {
            timerDisplay = displayTime;
            hasPurchased = true;
            player.m_AirControl = true;
            dialogBox3.SetActive(true);
            timesTalked += 1;
        } else if (hasPurchased) {
            timerDisplay = displayTime;
            dialogBox4.SetActive(true);
        }
        
    }
}
