using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
	
	public float maxDeltaX;
	public float minDeltaX;
	public float velX;

	public float maxDeltaY;
	public float minDeltaY;
	public float velY;
	
	float startX;
	float startY;
	
	// Record platform starting positions
	void Start () {
		startX = transform.position.x;
		startY = transform.position.y;
	}
	
	// Move the platform based on it's velocity
	void Update () {
		float deltaX = transform.position.x - startX;
		float deltaY = transform.position.y - startY;
		
		// If the platform reaches maxDeltaX, reverse its velocity.
		// Also, Don't let deltaX get larger than maxDeltaX.
		if(velX != 0 && (deltaX >= maxDeltaX || deltaX <= minDeltaX)) {
			
			if(deltaX >= maxDeltaX) {
				transform.position = new Vector3(startX + maxDeltaX, transform.position.y, transform.position.z);

			} else if(deltaX <= maxDeltaX) {
				transform.position = new Vector3(startX + minDeltaX, transform.position.y, transform.position.z);
			}
			
			velX *= -1;	
		}
		
		// If the platform reaches maxDeltaY, reverse its velocity.
		// Also, Don't let deltaY get larger than maxDeltaY.
		if(velY != 0 & (deltaY >= maxDeltaY || deltaY <= minDeltaY)) {

			if(deltaY >= maxDeltaY) {
				transform.position = new Vector3(transform.position.x, startY + maxDeltaY, transform.position.z);

			}else if(deltaY <= maxDeltaY) {
				transform.position = new Vector3(transform.position.x, startY + minDeltaY, transform.position.z);
			}
			
			velY *= -1;	
		}		
		
		transform.Translate(new Vector3(velX,velY,0) * Time.deltaTime);
	}
}
