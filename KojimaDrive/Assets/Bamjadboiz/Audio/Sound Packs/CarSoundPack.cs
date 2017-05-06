using UnityEngine;
using System.Collections;

[CreateAssetMenuAttribute]
public class CarSoundPack : ScriptableObject
{
    [Header("Looping")]
    public AudioClip engine;
    public AudioClip acceleration;

    [Header("Impact")]
    public AudioClip smallImpact;
    public AudioClip largeImpact;

    [Header("Other sounds")]
    public AudioClip skidding;
    public AudioClip landing;
    public AudioClip horn;
    public AudioClip pedalSound, gearChangeSound;
    public AudioClip swoosh;
}
