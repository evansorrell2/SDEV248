using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour {
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] public bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer spriteRenderer;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    [SerializeField] private int maxJumps = 1; // The player can jump a number of times equal to this value plus 1
    [SerializeField] private int numJumps = 0;
    public Animator animator;
    public Transform attackPoint;
    Vector2 lookDirection = new Vector2(1,0);
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] int attackDamage = 50;
    public LayerMask enemyLayers;
    [SerializeField] int maxHealth = 50;
	public int health {get{return currentHealth;}}
	int currentHealth;

	[SerializeField] float timeInvincible = 1.5f;
	bool isInvincible;
	float invincibleTimer;


	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	public bool hasStarted;
	public bool gameOver;
	public GameObject gameOverScreen;
    private MovementController playerMovement;
    private EnemyController enemies;
    public GameObject titleScreen;
	public int experience;
	public TextMeshProUGUI experienceValue;

	void Start() 
	{
		currentHealth = maxHealth;
		gameOverScreen.gameObject.SetActive(false);
		UpdateExperience(0);
	}

	private void Awake() {
        enemies = GameObject.Find("Skeleton").GetComponent<EnemyController>();
        playerMovement = GameObject.Find("Player").GetComponent<MovementController>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

    void Update() {
        if(!hasStarted) {
            return;
        }
		if (isInvincible) {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if(Input.GetKeyDown(KeyCode.F)) {
            Attack();
        }
		if (currentHealth <= 0) { // display game over and prevent player from moving.
			GameOver();
			animator.SetTrigger("Death");
			hasStarted = true;
			playerMovement.hasStarted = true;
        	gameObject.layer = 9;
		}
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Open Menu");
            enemies.hasStarted = false;
            hasStarted = false;
            playerMovement.hasStarted = false;
            titleScreen.gameObject.SetActive(true);
        }
		if(Input.GetKeyDown(KeyCode.X)) {
			RaycastHit2D hit = Physics2D.Raycast(m_Rigidbody2D.position + Vector2.down * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
			if (hit.collider != null) {
				if (hit.collider != null)
				{
					NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
					if (character != null)
					{
						character.DisplayDialog();
					}  
				}
				if(experience >= 100) {
					//purchase air control upgrade
				} else {
					//say you need 100 experience
				}
			}
            
        }
		
    }

	public void RestartGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void UpdateExperience(int amount) {
		experience += amount;
		experienceValue.text = "" + experience;
	}

	public void GameOver() {gameOverScreen.gameObject.SetActive(true);}

	private void FixedUpdate() {
        if(!hasStarted) {
            return;
        }
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
        animator.SetBool("Grounded", false);

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
                    animator.SetBool("Grounded", true);
                    numJumps = 0;
			}
		}
	}


	public void Move(float move, bool crouch, bool jump) {
        if(!hasStarted) {
            return;
        }
		// If crouching, check to see if the character can stand up
		if (!crouch) {
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) {
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl) {

			// If crouching
			if (crouch) {
				if (!m_wasCrouching) {
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else {
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching) {
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight) {
				// ... flip the player.
				m_FacingRight = !m_FacingRight;
                spriteRenderer.flipX = false;
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight) {
				// ... flip the player.
				m_FacingRight = !m_FacingRight;
                spriteRenderer.flipX = true;
			}
		}
		// If the player should jump...
		if ((m_Grounded && jump) || ((numJumps < maxJumps) && jump)) {
			// Add a vertical force to the player.
			m_Grounded = false;
            animator.SetBool("Grounded", false);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            numJumps += 1;
		}
	}

    void Attack() {
        if(!hasStarted) {
            return;
        }
        animator.SetTrigger("Attack1"); //play attack animation
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers); //detect enemies in range
        //apply damage to those in range
        foreach(Collider2D enemy in hitEnemies) {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
            Debug.Log("We hit" + enemy.name);
        }
    }

    void OnDrawGizmosSelected() {
        if(!hasStarted) {
            return;
        }
        if(attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

	public void ChangeHealth(int amount) {
        if(!hasStarted) {
            return;
        }
		if (amount < 0) {
			animator.SetTrigger("Hurt");
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
		if(amount > 0) {
			// heal, not implemented
		}
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIController.instance.SetValue(currentHealth / (float)maxHealth);
		Debug.Log("The player took " + amount + " damage!");
	}
}
	
