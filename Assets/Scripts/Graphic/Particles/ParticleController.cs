using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ParticleController : MonoBehaviour {
	public ParticleSystem snow;
	public ParticleSystem confetti;
	public ParticleSystem ramen;
	public enum Type {
		Snow,
		Confetti,
		Ramen,
		None
	};
	private Type type = Type.None;
	// Start is called before the first frame update
	void Start() {
		snow.Stop();
		confetti.Stop();
		ramen.Stop();
	}

	// Update is called once per frame
	void Update() {

	}

	private ParticleSystem GetParticle(Type type) {
		switch (type) {
		case Type.Snow:
			return snow;
		case Type.Confetti:
			return confetti;
		case Type.Ramen:
			return ramen;
		default:
			return null;
		}
	}

	public void Play(Type type) {
		if (this.type != type) {
			Stop();
		}
		ParticleSystem particle = GetParticle(type);
		if (particle) {
			particle.Play();
			this.type = type;
		} else {
			this.type = Type.None;
		}
	}

	public void Stop() {
		ParticleSystem particle = GetParticle(type);
		if (particle) {
			particle.Stop();
		}
		type = Type.None;
	}
}
