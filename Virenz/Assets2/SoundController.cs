using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioSource explosion;
    public AudioSource whistle;
    public ParticleSystem fireworks;
    private int particleCount;

    // Update is called once per frame
    void Update() {
        if (fireworks.particleCount > particleCount) {
            for (int i = 0; i < fireworks.particleCount - particleCount; i++) {
                whistle.Play();
            }
        }
        if (fireworks.particleCount < particleCount) {
            for (int i = 0; i < particleCount - fireworks.particleCount; i++) {
                explosion.Play();
            }
        }
        particleCount = fireworks.particleCount;
    }
}
