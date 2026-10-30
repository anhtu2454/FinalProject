using UnityEngine;

public class WallPiece : MonoBehaviour
{
    [HideInInspector] public BreakableWall wall;

    void OnCollisionEnter(Collision collision)
    {
        if (wall != null)
        {
            wall.OnPieceHit(collision);
        }
    }
}