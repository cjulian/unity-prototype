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
	bool grounded = false;
	bool ascending = false;	
	bool dashing = false;
	bool dashCoolingDown = false;
	
	
	// Use this for initialization
	void Start () {	
	}
	
	
	// Update is called once per frame
	void Update () {
		GetPlayerInput();
		UpdateStats();
	}

	
	void GetPlayerInput() {
		float hVel = GetHorizontalVelocity();
		float vVel = GetVerticalVelocity();
		rigidbody.velocity = new Vector3(hVel, vVel, 0);
	}

	
	// Calculate the horizontal velocity of the hero based on player input
	float GetHorizontalVelocity() {
		float hInputRaw = Input.GetAxisRaw("Horizontal");
		float hInput = Input.GetAxis("Horizontal");
		float hVel = 0.0f;
	
		// Is player pressing left/right?
		if(hInputRaw != 0.0f) {
			
			// Did player hit the dash button?
			if(Input.GetButtonDown("Fire1")) {
				
				// Player can always dash on the ground, but
				// can only dash numMaxAirDash times in the air
				if(!dashing && !dashCoolingDown && (grounded || numAirDash < numMaxAirDash)) {
					dashing = true;
					dashCoolingDown = true;

					if(!grounded) {
						numAirDash++;
					}
					
					StartCoroutine("DashTimer");				
				}
			}			
			
			hVel = dashing ? hInputRaw * dashSpeed : hInput * runSpeed;

		} else {
			// Dashing ends if player does not continue pressing left/right
			dashing = false;
		}
		
		return hVel;
	}
	
	
	// Calculate the vertical velocity of the hero based on player input
	float GetVerticalVelocity() {
		
		float vVel;
		
		// The player will not descend while air dashing
		if(dashing && !grounded && !ascending) { 
			vVel = 0.0f;
		} else {
			vVel = rigidbody.velocity.y;
		}

		if(vVel <= 0) {
			ascending = false;
		}
		
		// If the player hits jump and the hero is in a state such that he can jump,
		// then make the hero jump.  Includes double-jump check.
		if(Input.GetButtonDown("Jump") && (grounded || numAirJumps < numMaxAirJumps)) {
			
			vVel = jumpSpeed;
			ascending = true;
			
			if(!grounded) {
				numAirJumps++;
			}			
		}
		
		// If the player lets go of jump button mid jump, immediately start descending.
		// This allows for variable jump height.
		if(ascending && Input.GetButtonUp("Jump")) {
			vVel = 0;
			ascending = false;
		}
		
		return vVel;
	}
	
	
	// Controls durations of dash move as well as cooldown
	IEnumerator DashTimer() {
		yield return new WaitForSeconds(dashDuration);
		dashing = false;
		yield return new WaitForSeconds(dashCoolDown);
		dashCoolingDown = false;
	}
	
	
	// Check for grounded state
	void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            if(contact.normal == Vector3.up) {            
				grounded = true;
				numAirJumps = 0;
				numAirDash = 0;
			}
        }
	}
	
	// Release grounded state
	void OnCollisionExit(Collision col) {
		grounded = false;
	}	

	
	// For Debugging purposes
	void UpdateStats() {
		if(stats != null) {
			stats.text =
				"grounded: " + grounded.ToString() + "\n" +
				"air jumps remaining: " + (numMaxAirJumps - numAirJumps);
		}
	}
	
	// For use with the GroundedTrigger (was incorrect approach)
//	void UpdateGroundedState(bool g) {
//		grounded = g;
//
//		if(grounded) {
//			numAirJumps = 0;
//			numAirDash = 0;
//		}
//	}	
}
