using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : GroundEnemy
{
    private Vector2 Movement(GlobeObject target)
    {
        Vector2 move = new Vector2();
        move.x = GlobePosition.x < target.GlobePosition.x ? 1 : -1;
        return move;
    }
}
