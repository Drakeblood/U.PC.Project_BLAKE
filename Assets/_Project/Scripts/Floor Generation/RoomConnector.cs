using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    [SerializeField]
    private RoomConnector connectedDoor;
    [SerializeField]
    private GameObject doorObject;
    [SerializeField]
    private GameObject wallObject;


    private Room mainRoom;
    public void SetConnector(RoomConnector room)
    {
        connectedDoor = room;
    }

    public void SetDoor()
    {
        if(connectedDoor != null)
        {
            doorObject.SetActive(true);
            wallObject.SetActive(false);
        } else
        {
            doorObject.SetActive(false);
            wallObject.SetActive(true);
        }
    }

    public RoomConnector GetConnector()
    {
        return connectedDoor;
    }


    public void SetRoom(Room room)
    {
        mainRoom = room;
    }

    public Room GetRoom()
    {
        return mainRoom;
    }
}