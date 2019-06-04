using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

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
}
