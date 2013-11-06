using UnityEngine;
using System.Collections;

public class SideviewCharacterController : MonoBehaviour {
	
	// Public members
	public float jumpSpeed = 25.0f;
	public float runSpeed = 10.0f;
	public float dashSpeed = 35.0f;
	public float dashDuration = 0.25f;
	public float dashCoolDown = 0.5f;
	public GUIText stats;
	
	
	// Private members
	int numMaxAirJumps = 1;
	int numAirJumps = 0;
	int numMaxAirDash = 1;
	int numAirDash = 0;
	
	Vector3 groundedNormal;
	
	bool isGrounded = false;
	bool canRun = false;
	bool canJump = false;	
	bool canDash = false;	
	bool isAscending = false;	
	bool isDashing = false;
	bool isDashCooling = false;
	
	
	// Use this for initialization
	void Start () {	
	}
	
	
	// Update is called once per frame
	void Update () {
		UpdatePlayerState();
		GetPlayerInput();
		UpdateStats();
	}
	
	
	// Sets the various states that the hero can be in, eg. canJump, canDash, etc.
	void UpdatePlayerState() {
		canRun = isGrounded;
		canJump = isGrounded || numMaxAirJumps - numAirJumps > 0;		
		canDash = !isDashing && !isDashCooling && (isGrounded || numAirDash < numMaxAirDash);
	}
	
	
	// Gets the player's controller input and moves the hero accordingly
	void GetPlayerInput() {
		float hVel = GetHorizontalVelocity();
		float vVel = GetVerticalVelocity();
		rigidbody.velocity = new Vector3(hVel, vVel, 0);
	}

	
	// Calculate the horizontal velocity of the hero based on player input
	float GetHorizontalVelocity() {
		float hInputRaw = Input.GetAxisRaw("Horizontal"); // either -1, 0 or 1
		float hInput = Input.GetAxis("Horizontal"); // some value between -1 and 1
		float hVel = 0.0f;
	
		// Is player pressing left/right?
		if(hInputRaw != 0.0f) {
			
			// Did player hit the dash button?
			if(Input.GetButtonDown("Fire1")) {
				
				// Player can always dash on the ground, but
				// can only dash numMaxAirDash times in the air
				if(canDash) {
					isDashing = true;
					isDashCooling = true;

					if(!isGrounded) {
						numAirDash++;
					}
					
					StartCoroutine("DashTimer");				
				}
			}			
			
			hVel = isDashing ? hInputRaw * dashSpeed : hInput * runSpeed;

		} else {
			// Dashing ends if player does not continue pressing left/right
			isDashing = false;
		}
		
		return hVel;
	}
	
	
	// Calculate the vertical velocity of the hero based on player input
	float GetVerticalVelocity() {
		
		float vVel;
		
		// The player will not descend while air isDashing
		if(isDashing && !isGrounded && !isAscending) { 
			vVel = 0.0f;
		} else {
			vVel = rigidbody.velocity.y;
		}

		if(vVel <= 0) {
			isAscending = false;
		}
		
		// If the player hits jump and the hero is in a state such that he can jump,
		// then make the hero jump.  Includes double-jump check.
		if(Input.GetButtonDown("Jump") && (isGrounded || numAirJumps < numMaxAirJumps)) {
			
			vVel = jumpSpeed;
			isAscending = true;
			
			if(!isGrounded) {
				numAirJumps++;
			}			
		}
		
		// If the player lets go of jump button mid jump, immediately start descending.
		// This allows for variable jump height.
		if(isAscending && Input.GetButtonUp("Jump")) {
			vVel = 0;
			isAscending = false;
		}
		
		return vVel;
	}
	
	
	// Controls durations of dash move as well as cooldown
	IEnumerator DashTimer() {
		yield return new WaitForSeconds(dashDuration);
		isDashing = false;
		yield return new WaitForSeconds(dashCoolDown);
		isDashCooling = false;
	}
	
	
	// Check for isGrounded state
	void OnCollisionEnter(Collision collision) {
		CheckGroundedState(collision);
	}

	void OnCollisionStay(Collision collision) {
		CheckGroundedState(collision);
	}
	
	void CheckGroundedState(Collision collision) {

		foreach (ContactPoint contact in collision.contacts) {

			if(Mathf.Abs(contact.normal.x) < 0.8f && contact.normal.y > 0.7f) {
			// if(contact.normal == Vector3.up) {
				isGrounded = true;
				numAirJumps = 0;
				numAirDash = 0;
			}
			
			groundedNormal = contact.normal;
        }	
	}
	
	
	// Release isGrounded state
	void OnCollisionExit(Collision col) {
		isGrounded = false;
		groundedNormal = Vector3.zero;
	}	

	
	// For Debugging purposes
	void UpdateStats() {
		if(stats != null) {
			stats.text =
				" isGrounded: " + isGrounded.ToString() + 
				"\n isGrounded normal: " + groundedNormal.ToString() +
				"\n air jumps remaining: " + (numMaxAirJumps - numAirJumps) +
				"\n air dash remaining: " + (numMaxAirDash - numAirDash);
			
		}
	}
	
	// For use with the GroundedTrigger (was incorrect approach)
//	void UpdateGroundedState(bool g) {
//		isGrounded = g;
//
//		if(isGrounded) {
//			numAirJumps = 0;
//			numAirDash = 0;
//		}
//	}	
}
