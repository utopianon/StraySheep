﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int score;
    public bool victoryPickup;

    public void Die()
    {
        Destroy(gameObject);
    }
}
