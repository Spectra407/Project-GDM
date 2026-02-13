using UnityEngine;

[System.Flags]
public enum EnemyMoveType
{
    None = 0,
    Attack = 1,
    Block = 2,
    Strength = 3,
    Madness = 4
}
