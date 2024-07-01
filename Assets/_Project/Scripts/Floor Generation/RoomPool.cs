using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="Room Pool", menuName = "Floor Generation/Room Pool")]
public class RoomPool : ScriptableObject
{
    [SerializeField]
    private GameObject[] room;

    public GameObject GetRandomRoomFromPool()
    {
        return room[Random.Range(0, room.Length)];
    }

    public GameObject GetRandomRoomOfSize(RoomSize size)
    {
        List<GameObject> list = room.Where(room => room.GetComponent<Room>() != null && room.GetComponent<Room>().roomSize == size).ToList();
        if(list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
