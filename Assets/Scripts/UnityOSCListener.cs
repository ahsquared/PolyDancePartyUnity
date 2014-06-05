

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityOSCListener : MonoBehaviour  {
	//private GameObject madeCube;
	public Quaternion rotation;
	public GameObject[] shapes = new GameObject[6];
	private int numShapes = 6;
	public float radius = 4f;
	public float angle = Mathf.PI / 6; //30f;
	public float minAccelForLaunch = 6.5f;
	private bool init = false;
	public int counter = 0;
	float scaleFactor = 0.2f;
	float minShapeDimension = 0.5f;
	float maxShapeDimension = 2.5f;
	Helpers helpers;

	void Start() {
		helpers = GameObject.Find("Helpers").GetComponent<Helpers>();
		numShapes = shapes.Length;
		Debug.Log (shapes[1]);
	}
	public void OSCMessageReceived(OSC.NET.OSCMessage message){	
		string address = message.Address;
		//ArrayList args = message.Values;
		Bounds bounds = new Bounds (Vector3.zero, new Vector3 (1, 2, 1));

		// https://github.com/ellensundh#sthash.n1WHcF6t.dpuf

		//oscListener.oscHandler.Send(Osc.StringToOscMessage("Your event name")); 
	
		//- See more at: http://www.sundh.com/blog/2013/01/send-osc-messages-from-unity/#sthash.cOVjJbPt.dpuf

		if (address.Contains ("rot") && init) {
			//Debug.Log (address);
			string[] rotVals = address.Substring (4).Split ('|');
			if (rotVals[0].IndexOf("null") == -1) {
				float alpha = float.Parse (rotVals [0]);
				float beta = float.Parse (rotVals [1]);
				float gamma = float.Parse (rotVals [2]);
				string rotId = rotVals[3];
				rotation = Quaternion.Euler (beta, gamma, alpha);
				//rotation = new Quaternion (x, y, z, w);
				GameObject shapeToRotate = GameObject.Find (rotId);
				//hand.transform.Translate (x/factor, y/factor, z/factor, null);
				if (shapeToRotate) {
					shapeToRotate.transform.rotation = rotation;
				}
			}
		}
		if (address.Contains ("acc") && init) {
			string[] accVals = address.Substring (4).Split ('|');
			//Debug.Log(accVals[0].IndexOf("null"));
			if (accVals[0].IndexOf("null") == -1) {
				float valX = float.Parse( accVals [0] );
				float valY = float.Parse( accVals [1] );
				float valZ = float.Parse( accVals [2] );
				float x = NormalizeAccel( valX );
				float y = NormalizeAccel( valY );
				float z = NormalizeAccel( valZ );
				Vector3 v = new Vector3(valX * scaleFactor, valY * scaleFactor, valZ * scaleFactor);
				//Vector3 normV = new Vector3(x, y, z);
				string accId = accVals[3];
				GameObject shapeToScale = GameObject.Find (accId);
				if (shapeToScale) {
					shapeToScale.GetComponent<move>().scaleShape(v.sqrMagnitude);
					if ( ( x + y + z ) > minAccelForLaunch) {
						shapeToScale.GetComponent<move>().Fling(v);
					}
					shapeToScale.GetComponent<MidiControl>().sendCC(v.sqrMagnitude);
					// not sure about the halo look
					//cubeToScale.transform.GetComponentInChildren<Light>().light.range = 3 * (x + y + z);
				}
			}
		}
		if (address.Contains ("create")) {
			Debug.Log (address.Substring (8));
			string id = address.Split ('|')[1];
			GameObject newShape;
			Vector3 initPos = helpers.locationOnXYCircle(counter, radius, angle);
			int shapeCount = counter % numShapes;
			Debug.Log (shapeCount);
			newShape = (GameObject) Instantiate(shapes[shapeCount], initPos, Quaternion.identity);
//			if (counter % 3 == 0) {
//				newShape = (GameObject)Instantiate(shape1, initPos, Quaternion.identity);
//			} else if (counter % 2 == 0) {
//				newShape = (GameObject)Instantiate(shape2, initPos, Quaternion.identity);
//			} else  {
//				newShape = (GameObject)Instantiate(shape3, initPos, Quaternion.identity);
//			}
			newShape.name = id;
			newShape.GetComponent<MidiControl>().controlNumber = counter + 1;
			newShape.GetComponent<move>().initPos = initPos;
			bounds.Encapsulate(newShape.transform.position);
			init = true;
			counter++;
		}
	}


	float NormalizeAccel(float val) {
		return Mathf.Max (
		          Mathf.Min (Mathf.Abs (val * scaleFactor), 
		                     maxShapeDimension), 
		          minShapeDimension);
	}
}
