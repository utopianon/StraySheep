using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    // :)
    public BoxCollider2D boxCollider;

    // scoring
    public int currentScore, maxScore;
    public float levelTimer, distance;

    private void Awake()
    {
        #region Singleton

        if (GM == null)
            GM = this;

        if (GM != this)
            Destroy(gameObject);

        #endregion
    }

    #region Static methods

    public static Vector2 GetBoxCastSize(BoxCollider2D boxCol)
    {
        return new Vector2(boxCol.transform.localScale.x * boxCol.size.x, boxCol.transform.localScale.y * boxCol.size.y);
    }

    #endregion

    public void DoDangerAndPickup()
    {
        // done once Awake/Start
        Vector2 castSize = GetBoxCastSize(boxCollider);

        // get this from movement
        Vector3 velocity = new Vector3();

        // do this in player and in FixedUpdate
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, castSize, transform.eulerAngles.z, velocity.normalized, velocity.magnitude);

        // need for loop because deleting objects on array
        for (int i = 0; i < hits.Length; i++)
        {
            Pickup pickup = hits[i].collider.GetComponent<Pickup>();
            if (pickup != null)
            {
                currentScore += pickup.score;
            }
        }
    }
}
