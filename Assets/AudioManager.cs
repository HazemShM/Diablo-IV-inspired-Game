using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----Audio Source-----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-----Audio Clip-----")]
    public AudioClip background;
    public AudioClip RogueArrow;
    public AudioClip RogueDash;
    public AudioClip RogueDie;
    public AudioClip RogueHeal;
    public AudioClip RogueHealth;
    public AudioClip RogueHit;
    public AudioClip RogueRune;
    public AudioClip RogueShower;
    public AudioClip RogueSmoke;
    public AudioClip BarDie;
    public AudioClip BarHit;
    public AudioClip BarShield;
    public AudioClip BarVoicy;
    public AudioClip SorFire;
    public AudioClip SorFireBall;
    public AudioClip SorWhoosh;
    public AudioClip BossMinion;
    public AudioClip BossDamage;
    public AudioClip BossDeath;
    public AudioClip BossSpike;
    public AudioClip BossBomb;
    public AudioClip Explosion;
    public AudioClip BossDown;
    public AudioClip EnemyDie;
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    
}
