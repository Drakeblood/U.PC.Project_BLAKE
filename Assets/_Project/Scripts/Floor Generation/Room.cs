using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System;

public class Room : MonoBehaviour
{
    [SerializeField]
    private RoomConnector[] doors;
    [SerializeField]
    private RandomizedRoomObject[] randomObjects;
    [SerializeField]
    private GameObject fog;
    private RoomManager roomManager;

    [Header("Minimap variables")]
    [SerializeField]
    private MinimapRoom minimapRoom;

    [Header("Spawning Enemies")]
    [SerializeField]
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    [SerializeField]
    private List<GameObject> spawnedEnemies;

    [Serializable]
    public struct EnemySpawner
    {
        public Transform EnemySpawnPoint;
        public GameObject EnemyToSpawn;
        public Waypoints EnemyWaypoints;
    }

    public void InitializeRoom(RoomManager rm)
    {
        roomManager = rm;
        spawnedEnemies = new List<GameObject>();
        foreach(RandomizedRoomObject randomObject in randomObjects)
        {
            randomObject.InitializeRandomObject();
        }
        foreach(RoomConnector door in doors)
        {
            door.SetRoom(this);
            door.SetDoor();
        }

        if(minimapRoom != null && roomManager.GetMinimapFloor() != null)
        {
            minimapRoom.transform.parent = roomManager.GetMinimapFloor();
        }

        //Build NavMesh
        NavMeshSurface[] surfaces = GetComponentsInChildren<NavMeshSurface>();
        foreach(NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }

        //Spawn enemies
        foreach(EnemySpawner enemy in spawners)
        {
            GameObject spawnedEnemy = Instantiate(enemy.EnemyToSpawn.gameObject, enemy.EnemySpawnPoint.transform.position, enemy.EnemySpawnPoint.rotation, this.transform);
            spawnedEnemy.GetComponent<EnemyAIManager>().SetWaypoints(enemy.EnemyWaypoints);
            spawnedEnemies.Add(spawnedEnemy);
        }
        
    }

    public RoomConnector[] GetDoors()
    {
        return doors;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EnterRoom();
        }
    }

    public void SeeRoom()
    {
        minimapRoom.ShowRoom();
    }

    private void EnterRoom()
    {
        minimapRoom.VisitRoom();
        fog.SetActive(false);
        Room activeRoom = roomManager.GetActiveRoom();
        if (activeRoom != null)
        {
            List<Room> roomsToDisable = activeRoom.GetNeigbours();
            if (roomsToDisable.Contains(this)) roomsToDisable.Remove(this);
            foreach (Room room in roomsToDisable)
            {
                room.gameObject.SetActive(false);
            }
        }
        List<Room> roomsToActivate = GetNeigbours();
        if (roomsToActivate.Contains(activeRoom)) roomsToActivate.Remove(activeRoom);


        foreach (Room room in roomsToActivate)
        {
            room.SeeRoom();
            room.gameObject.SetActive(true);
        }

        roomManager.SetActiveRoom(this);

        foreach(GameObject enemy in spawnedEnemies)
        {
            if (enemy == null) continue;
            enemy.GetComponent<EnemyAIManager>().UpdatePlayerRef();
            enemy.GetComponent<EnemyFOV>().FindPlayer();
        }
    }

    private void ExitRoom()
    {
        fog.SetActive(true);
        if(roomManager.GetActiveRoom() == this)
        {
            roomManager.SetActiveRoom(null);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(roomManager.GetActiveRoom() != this)
            {
                ExitRoom();
            }
            if (roomManager.GetActiveRoom() == null)
            {
                EnterRoom();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitRoom();
        }
    }

    public List<Room> GetNeigbours()
    {
        List<Room> neigbours = new List<Room>();
        foreach(RoomConnector door in doors)
        {
            RoomConnector neighbour = door.GetConnector();
            if (neighbour == null) continue;

            neigbours.Add(neighbour.GetRoom());
        }
        return neigbours;
    }

}
