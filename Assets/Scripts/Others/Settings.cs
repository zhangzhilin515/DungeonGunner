using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    //Tile Pixel Setting
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    //Dungeon Build
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    //Room Setting
    public const float fadeInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const float doorUnlockDelay = 1f;
    //Animator Parameters
    public static float baseSpeedForPlayerAnimations = 8f;
    public static float baseSpeedForEnemyAnimations = 3f;
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int open = Animator.StringToHash("open");
    public static int destroy = Animator.StringToHash("destroy");
    public static int use = Animator.StringToHash("use");
    public static String stateDestroyed = "Destroyed";
    //GameObject Tags
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    //Audio
    public const float musicFadeOutTime = 0.5f;
    public const float musicFadeInTime = 0.5f;
    //Weapon Setting
    public const float useAimAngleDistance = 3.5f;
    //Astar Parameters
    public const int defaultAstarMovementPenalty = 40;
    public const int preferredPathAstarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    //Enemy Parameters
    public const int defaultEnemyHealth = 20;
    public const float contactDamageCollisionResetDelay = 0.5f;
    //UI Parameters
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    //High Scores
    public const int numberOfHighScoresToSave = 100;
}
