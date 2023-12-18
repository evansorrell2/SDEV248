using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] private float speed = 10;
    [SerializeField] private float changeTime = 3.0f;
    float timer;
    int direction = 1;
    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    bool flip = false;
    public bool hasStarted;

    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public Animator skeletonAnimator;
    private PlayerController player;
    

    // Start is called before the first frame update
    void Start() {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        currentHealth = maxHealth;
        skeletonAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update() {
        if(!hasStarted) {
            return;
        }
        // drives how often the enemy turns around in their patrol
        timer -= Time.deltaTime;

        if(timer < 0) {
            direction = -direction;
            timer = changeTime;
            spriteRenderer.flipX = flip;
            flip = !flip;
        }
    }

    void FixedUpdate() {
        if(!hasStarted) {
            return;
        }
        Vector2 position = rigidbody2d.position;
        position.x = position.x + Time.deltaTime * speed * direction;
        skeletonAnimator.SetFloat("Move X", direction);

        rigidbody2d.MovePosition(position);
    }

    public void TakeDamage(int damage) {
        if(!hasStarted) {
            return;
        }
        currentHealth -= damage;
        skeletonAnimator.SetTrigger("Hurt");
        if(currentHealth <= 0) {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(!hasStarted) {
            return;
        }
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            player.ChangeHealth(-8);
        }
    }

    void Die() {
        if(!hasStarted) {
            return;
        }
        Debug.Log(name + " died!");
        skeletonAnimator.SetBool("Death", true);
        gameObject.layer = 9;
        player.UpdateExperience(100);
        this.enabled = false;
    }
}
