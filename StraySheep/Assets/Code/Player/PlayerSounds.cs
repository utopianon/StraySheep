using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public void PlayFootStep()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Movement/Footsteps/Footsteps", transform.position);
    }

    public void PlayJump()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Movement/Jumps/Jumps", transform.position);
    }
}
