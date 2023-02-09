using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public delegate void OnCrossThreshold(GameObject entity, GameObject building, bool isEnter);
    public static event OnCrossThreshold onCrossThreshold;

    public delegate void OnNewFloor(GameObject entity, GameObject floor, bool isEnter);
    public static event OnNewFloor onNewFloor;

    public delegate void OnDemonButton(string buttonID);
    public static event OnDemonButton onDemonButton;

    public delegate void OnJump(List<Vector3> jumpPts);
    public static event OnJump onJump;

    public delegate void OnObjective(string obj, bool isComplete);
    public static event OnObjective onQuest;

    public delegate void OnPause(bool setPause);
    public static event OnPause onPause;

    public delegate void OnSliderChange(string origin, float val);
    public static event OnSliderChange onSliderChange;

    public delegate void OnColourPicker(Vector4 colour);
    public static event OnColourPicker onColourPicker;

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

    public static void DemonButton(string buttonID)
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

    public static void SliderChange(string origin, float val)
    {
        if (onSliderChange != null)
            onSliderChange(origin, val);
    }

    public static void ColourChange(Vector4 colour)
    {
        if (onColourPicker != null)
            onColourPicker(colour);
    }
}
