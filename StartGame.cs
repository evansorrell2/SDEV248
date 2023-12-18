using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    private PlayerController player;
    private MovementController playerMovement;
    private EnemyController enemies;
    private Button startButton;
    public GameObject titleScreen;

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.Find("Skeleton").GetComponent<EnemyController>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerMovement = GameObject.Find("Player").GetComponent<MovementController>();
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(BeginGame);
        enemies.hasStarted = false;
        player.hasStarted = false;
        playerMovement.hasStarted = false;
    }

    public void BeginGame() {
        enemies.hasStarted = true;
        player.hasStarted = true;
        playerMovement.hasStarted = true;
        titleScreen.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
