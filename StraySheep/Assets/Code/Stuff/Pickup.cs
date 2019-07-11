using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int score;
    public bool victoryPickup;

    public void Die()
    {
        if (victoryPickup)
            GameManager.GM.LoadNextScene();
        Destroy(gameObject);
    }
}
