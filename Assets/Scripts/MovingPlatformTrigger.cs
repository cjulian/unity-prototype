using UnityEngine;
using System.Collections;

public class MovingPlatformTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col) {
		if(col.gameObject != null && col.gameObject.rigidbody != null) {
			col.transform.parent = transform;
			Debug.Log("triggerEnter");
		}
	}
	
	void OnTriggerExit(Collider col) {
		if(col.gameObject != null && col.gameObject.rigidbody != null) {
			col.transform.parent = null;
			Debug.Log("triggerExit");
		}
	}	
}
