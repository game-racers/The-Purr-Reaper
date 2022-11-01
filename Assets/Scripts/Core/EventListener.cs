using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public delegate void OnCrossThreshold(GameObject entity, GameObject building, bool isEnter);
    public static event OnCrossThreshold onCrossThreshold;

    public delegate void OnNewFloor(GameObject entity, GameObject floor, bool isEnter);
    public static event OnNewFloor onNewFloor;

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
}
