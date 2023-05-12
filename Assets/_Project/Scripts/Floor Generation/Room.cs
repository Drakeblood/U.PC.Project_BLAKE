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

    [SerializeField]
    private BoxCollider[] overlapColliders;

    [Header("Enemies")]
    [SerializeField]
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    [SerializeField]
    private List<GameObject> spawnedEnemies;
    [SerializeField]
    private bool isInitialized = false;
    [SerializeField]
    private bool isBeaten = false;

    [SerializeField]
    private Transform spawnPoint;
    private GameObject player;

    private RoomsDoneCounter roomsDoneCounter;

    [Serializable]
    public struct EnemySpawner
    {
        public Transform EnemySpawnPoint;
        public GameObject EnemyToSpawn;
        public Waypoints EnemyWaypoints;
    }

    private void Awake()
    {
        roomsDoneCounter = FindObjectOfType<RoomsDoneCounter>();
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

        foreach (RoomConnector roomConnector in doors)
        {
            roomConnector.OpenDoor();
        }

        if (spawnedEnemies.Count == 0)
        {
            isBeaten = true;

        }
        isInitialized = true;

    }


    public RoomConnector[] GetDoors()
    {
        return doors;
    }

    public void SeeRoom()
    {
        minimapRoom.ShowRoom();
    }

    public void DisableFog()
    {
        fog.SetActive(false);
    }

    public void EnableFog()
    {
        fog.SetActive(true);
    }

    public void EnterRoom()
    {
        minimapRoom.VisitRoom();
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

        if(!isBeaten)
        {
            foreach(RoomConnector roomConnector in doors)
            {
                roomConnector.CloseDoor();
            }
        } else if (isBeaten && player != null)
        {
            player.GetComponent<BlakeCharacter>().SetRespawnPosition(GetSpawnPointPosition());
        }
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
        player.GetComponent<BlakeCharacter>().onRespawn += ResetRoom;

    }

    private void ResetRoom()
    {
        if (roomManager.GetActiveRoom() != this) return;
        foreach (RoomConnector roomConnector in doors)
        {
            roomConnector.OpenDoor();
        }
        minimapRoom.ForgetRoom();

        foreach(GameObject enemy in spawnedEnemies)
        {
            enemy.GetComponent<EnemyAIManager>().SwitchCurrentState(enemy.GetComponent<EnemyAIManager>().PatrolState);
        }
        Invoke("ResetEnemies", 0.5f);
    }


    private void ResetEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            enemy.GetComponent<EnemyAIManager>().SwitchCurrentState(enemy.GetComponent<EnemyAIManager>().PatrolState);
        }
    }

    private void Update()
    {
        if(!isBeaten && isInitialized)
        {
            if(spawnedEnemies.Count == 0)
            {
                isBeaten = true;
                roomsDoneCounter.AddBeatenRoom();
                foreach (RoomConnector roomConnector in doors)
                {
                    roomConnector.OpenDoor();
                }
                if(player != null)
                {
                    player.GetComponent<BlakeCharacter>().SetRespawnPosition(GetSpawnPointPosition());
                }
            }
        }

        if(spawnedEnemies.Count > 0)
        {
            for(int i = spawnedEnemies.Count -1; i >= 0; i--)
            {
                if (spawnedEnemies[i] == null)
                {
                    spawnedEnemies.RemoveAt(i);
                }
            }
        }
    }

    public void ExitRoom()
    {
        if(roomManager.GetActiveRoom() == this)
        {
            roomManager.SetActiveRoom(null);

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

    public BoxCollider[] GetOverlapColliders()
    {
        return overlapColliders;
    }

    public RoomManager GetRoomManager()
    {
        return roomManager;
    }

    public Vector3 GetSpawnPointPosition()
    {
        return spawnPoint.position;
    }

}
