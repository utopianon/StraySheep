using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Vector3Int offset = new Vector3Int(-3, 0, 0);

    private Transform _player;

    private void Start()
    {
        _player = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (_player.position.x >= transform.position.x + offset.x)
        {
            GameManager.GM.EndScreen(true);
            // TODO: animations and audio

            // bug fix: not forcing endscreen to be visible :D
            enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position + offset + Vector3.up * -100, transform.position + offset + Vector3.up * 100);
    }
}
