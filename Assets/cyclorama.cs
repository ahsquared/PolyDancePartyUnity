using UnityEngine;
using System.Collections;

public class cyclorama : MonoBehaviour {
	// Use this for initialization
	void Start () {
		RenderSettings.skybox.SetColor ("_Tint", new Color (0.0f, 120.0f, 0.0f));
	}

	public Color colorStart = Color.blue;
	public Color colorEnd = Color.green;
	public float duration = 1.0F;

	void Update() {
		float lerp = Mathf.PingPong(Time.time, duration) / duration;
		RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorStart, colorEnd, lerp));
	}

}
