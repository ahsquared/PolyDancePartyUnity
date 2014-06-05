using UnityEngine;
using System.Collections;

public class CameraDolly : MonoBehaviour {
	public Transform target;
	public float radius = 20f;
	public float dollyTime = 20f;
	public float stallTime = 5f;
	public int numberOfNodes = 4;
	private float angle;
	private Vector3[] cameraPathXZ;
	private Vector3[] cameraPathZY;
	Helpers helpers;

	// Use this for initialization
	void Start () {
		cameraPathXZ = new Vector3 [numberOfNodes + 1];
		cameraPathZY = new Vector3 [numberOfNodes + 1];
		helpers = GameObject.Find("Helpers").GetComponent<Helpers>();
		angle = (2 * Mathf.PI) / numberOfNodes;
		for (int i = 0; i < numberOfNodes; i++) {
			cameraPathXZ[i] = helpers.locationOnXZCircle(i, radius, angle);
			cameraPathZY[i] = helpers.locationOnZYCircle(i, radius, angle);
			//Debug.Log ("Path node: " + cameraPath[i] + ", counter: " + i);
		}
		cameraPathXZ[numberOfNodes] = helpers.locationOnXZCircle(0, radius, angle);
		cameraPathZY[numberOfNodes] = helpers.locationOnXZCircle(0, radius, angle);

		//oldRotation = target.rotation;

		dollyOnXZ ();
	}

	void dollyOnXZ() {
		iTween.MoveTo (gameObject, iTween.Hash ("path", cameraPathXZ, "time", dollyTime,
		                                        "easetype", iTween.EaseType.easeInOutQuad,
		                                        "looptype", iTween.LoopType.loop,
		                                        "delay", stallTime,
		                                        //"looktarget", target,
		                                        //"axis", "y",
		                                        "oncomplete", "dollyOnZY"));
	}
	void dollyOnZY() {
		iTween.MoveTo (gameObject, iTween.Hash ("path", cameraPathZY, "time", dollyTime,
		                                        "easetype", iTween.EaseType.easeInOutQuad,
		                                        "looptype", iTween.LoopType.loop,
		                                        "delay", stallTime,
		                                        //"looktarget", target,
		                                        //"axis", "x",
		                                        "oncomplete", "dollyOnXZ"));
	}

	void Update() {
		transform.LookAt(target, transform.up);
	}
}
