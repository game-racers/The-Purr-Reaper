using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public delegate void OnCrossThreshold(GameObject entity, GameObject building, bool isEnter);
    public static event OnCrossThreshold onCrossThreshold;

    public delegate void OnNewFloor(GameObject entity, GameObject floor, bool isEnter);
    public static event OnNewFloor onNewFloor;

    public delegate void OnDemonButton(int buttonID);
    public static event OnDemonButton onDemonButton;

    public delegate void OnJump(List<Vector3> jumpPts);
    public static event OnJump onJump;

    public delegate void OnObjective(string obj, bool isComplete);
    public static event OnObjective onQuest;

    public delegate void OnPause(bool setPause);
    public static event OnPause onPause;

    public static void CrossBuilding(GameObject entity, GameObject building, bool isEnter)
    {
        if (onCrossThreshold != null)
            onCrossThreshold(entity, building, isEnter);
    }

    public static void NewFloor(GameObject entity, GameObject floor, bool isEnter)
    {
        if (onNewFloor != null)
            onNewFloor(entity, floor, isEnter);
    }

    public static void DemonButton(int buttonID)
    {
        if (onDemonButton != null)
            onDemonButton(buttonID);
    }

    public static void JumpAction(List<Vector3> jumpPts)
    {
        if (onJump != null)
            onJump(jumpPts);
    }

    public static void QuestFinished(string obj, bool isComplete)
    {
        if (onQuest != null)
            onQuest(obj, isComplete);
    }

    public static void PauseGame(bool setPause)
    {
        if (onPause != null)
            onPause(setPause);
    }
}
