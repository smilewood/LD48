using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public enum PlayerSound
    {
        Footsteps,
        Jump,
        Die,
        Land,
        HitGhost,
        HitLava,
        HitSpike
    }


    public AudioSource Footsteps;
    public AudioSource Jump;
    public AudioSource Death;
    public AudioSource Land;
    public AudioSource HitGhost;
    public AudioSource HitLava;
    public AudioSource HitSpike;

    public void PlaySound(PlayerSound sound)
    {
        switch (sound)
        {
            case PlayerSound.Footsteps:
            if (!Footsteps.isPlaying)
            {
                Footsteps.Play();
            }
            break;
            case PlayerSound.Jump:
            Jump.Play();
            break;
            case PlayerSound.Die:
            Death.Play();
            break;
            case PlayerSound.Land:
            Land.Play();
            break;
            case PlayerSound.HitGhost:
            HitGhost.Play();
            break;
            case PlayerSound.HitLava:
            HitLava.Play();
            break;
            case PlayerSound.HitSpike:
            HitSpike.Play();
            break;
        }
    }
}
