using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically destroys the GameObject when there are no children left.
/// </summary>

public class CFX_AutodestructWhenNoChildren : MonoBehaviour {
	// Update is called once per frame
	void Update() {
		if (transform.childCount == 0) {
			Destroy(this.gameObject);
		}
	}
}
