﻿using UnityEngine;
using System.Collections;

public class RoomsEventsExample : MonoBehaviour
{
    public void StartedRoomTransition(int currentRoom, int previousRoom)
    {
        Debug.LogFormat("Started Room Transition - Current Room:{0} - Previous Room:{1}", currentRoom, previousRoom);
    }

    public void FinishedRoomTransition(int currentRoom, int previousRoom)
    {
        Debug.LogFormat("Finished Room Transition - Current Room:{0} - Previous Room:{1}", currentRoom, previousRoom);
    }
}
