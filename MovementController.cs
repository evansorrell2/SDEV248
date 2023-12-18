using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    
    public PlayerController controller;
    public Animator animator;

    public float runSpeed = 40.0f;
    float horizontalMove = 0f;
    float verticalMove = 0f;
    bool jump = false;
    bool crouch = false;
    public bool hasStarted;

    Rigidbody2D m_Rigidbody2D;

    private void Awake() {
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if(!hasStarted) {
            return;
        }
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        verticalMove = m_Rigidbody2D.velocity.y;
        animator.SetFloat("AirSpeedY", verticalMove);
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if(Input.GetButtonDown("Jump")) {
            jump = true;
            animator.SetBool("isJumping", true);
            animator.SetBool("Grounded", false);
        }
        if(Input.GetButtonDown("Crouch")) {
            crouch = true;
        } else if(Input.GetButtonUp("Crouch")) {
            crouch = false;
        }
        if(verticalMove < -0.5) {
            animator.SetBool("isJumping", false);
            animator.SetBool("Grounded", false);
        } else if (verticalMove > 0.5) {
            animator.SetBool("isJumping", true);
        } else {
            animator.SetBool("isJumping", false);
            animator.SetBool("Grounded", true);
        }
    }

    void FixedUpdate() {
        if(!hasStarted) {
            return;
        }
        controller.Move(horizontalMove * Time.deltaTime, crouch, jump);
        jump = false;
    }
}
