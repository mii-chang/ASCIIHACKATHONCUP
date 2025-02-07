using UnityEngine;
using System.Collections;

// Drag/Drop this script on a Particle System (or an object having Particle System objects as children) to prevent a Shuriken bug
// where a system would emit at its original instantiated position before being translated, resulting in particles in-between
// the two positions.
// Possibly a threading bug from Unity (as of 3.5.4)

public class CFX_ShurikenThreadFix : MonoBehaviour {
	private ParticleSystem[] systems;

	void Awake() {
		systems = GetComponentsInChildren<ParticleSystem>();

		foreach (ParticleSystem ps in systems) {
			var em = ps.emission;
			em.enabled = false;
		}

		StartCoroutine("WaitFrame");
	}

	IEnumerator WaitFrame() {
		yield return null;

		foreach (ParticleSystem ps in systems) {
			var em = ps.emission;
			em.enabled = true;
			ps.Play(true);
		}
	}
}