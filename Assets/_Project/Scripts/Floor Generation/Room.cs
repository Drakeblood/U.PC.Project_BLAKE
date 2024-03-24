using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System;

public enum RoomType
{
    Base,
    Treasure,
    Shop,
    Boss
}

public class Room : MonoBehaviour
{
    [SerializeField]
    private RoomType roomType = RoomType.Base;

    [SerializeField]
    private RoomConnector[] doors;

    [SerializeField]
    private RandomizedRoomObject[] randomObjects;

    [SerializeField]
    private GameObject fog;

    private RoomManager roomManager;

    [SerializeField, Header("Minimap variables")]
    private MinimapRoom minimapRoom;

    [SerializeField]
    private BoxCollider[] overlapColliders;

    [SerializeField, Header("Enemies")]
    private List<EnemySpawner> spawners = new List<EnemySpawner>();

    [SerializeField]
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    [SerializeField]
    private bool isInitialized = false;

    [SerializeField]
    private bool isBeaten = false;

    [SerializeField]
    private Transform spawnPoint;

    private GameObject player;
    private List<RoomTrigger> triggers = new List<RoomTrigger>();
    private List<RoomOverlapTrigger> fogTriggers = new List<RoomOverlapTrigger>();
    private RoomsDoneCounter roomsDoneCounter;
    private List<GameObject> instantiatedWeapons;

    [HideInInspector]
    public int gCost;
    [HideInInspector]
    public int hCost;
    [HideInInspector]
    public int fCost;

    [HideInInspector]
    public Room cameFromRoom;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

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
        instantiatedWeapons = new List<GameObject>();

    }

    public void SetupDoorConnectors()
    {
        foreach (RoomConnector door in doors)
        {
            door.SetRoom(this);
            door.SetDoor();
        }
    }

    public void InitializeRoom(RoomManager rm)
    {
        roomManager = rm;
        spawnedEnemies = new List<GameObject>();
        foreach(RandomizedRoomObject randomObject in randomObjects)
        {
            randomObject.InitializeRandomObject();
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

        SpawnEnemies();

        foreach (RoomConnector roomConnector in doors)
        {
            roomConnector.UnlockDoor();
            roomConnector.CloseDoor();
        }
        isInitialized = true;

    }

    private void SpawnEnemies()
    {
        //Spawn enemies
        foreach (EnemySpawner enemy in spawners)
        {
            GameObject spawnedEnemy = Instantiate(enemy.EnemyToSpawn.gameObject, enemy.EnemySpawnPoint.transform.position, enemy.EnemySpawnPoint.rotation, this.transform);
            spawnedEnemy.GetComponent<AIController>().SetWaypoints(enemy.EnemyWaypoints);
            spawnedEnemy.GetComponent<EnemyCharacter>().SpawnedInRoom = this;
            spawnedEnemies.Add(spawnedEnemy);
        }
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    private void BeatLevel()
    {
        player.GetComponent<BlakeCharacter>().onRespawn += ResetRoom;
        roomsDoneCounter.AddBeatenRoom();
        isBeaten = true;
        minimapRoom.CompleteRoom();
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
        if (IsPlayerInsideFog()) return;
        fog.SetActive(false);
    }

    public void EnableFog()
    {
        if (IsPlayerInsideFog()) return;
        fog.SetActive(true);
    }

    public void DisableRoom()
    {
        gameObject.SetActive(false);
        var toDelete = new List<GameObject>();
        foreach(var weapon in instantiatedWeapons)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(false);
            } else
            {
                toDelete.Add(weapon);
            }
        }

        foreach(var delete in toDelete)
        {
            instantiatedWeapons.Remove(delete);
        }
    }

    public void EnterRoom()
    {
        if (IsPlayerInside()) return;
        minimapRoom.VisitRoom();
        Room activeRoom = roomManager.GetActiveRoom();
        this.gameObject.SetActive(true);
        /*if (activeRoom != null && activeRoom != this)
        {
            List<Room> roomsToDisable = activeRoom.GetNeigbours();
            if (roomsToDisable.Contains(this)) roomsToDisable.Remove(this);
            foreach (Room room in roomsToDisable)
            {
                room.DisableRoom();
            }
        }*/
        /*List<Room> roomsToActivate = GetNeigbours();
        if (roomsToActivate.Contains(activeRoom)) roomsToActivate.Remove(activeRoom);


        foreach (Room room in roomsToActivate)
        {
            room.SeeRoom();
            room.gameObject.SetActive(true);
        }*/

        roomManager.SetActiveRoom(this);

        foreach(GameObject enemy in spawnedEnemies)
        {
            if (enemy == null) continue;
            enemy.GetComponent<AIController>().UpdatePlayerRef();
        }

        

        if (!isBeaten)
        {
            if (spawnedEnemies.Count == 0)
            {
                BeatLevel();
            }
            else
            {
                foreach (RoomConnector roomConnector in doors)
                {
                    if (roomConnector.GetConnector() != null)
                    {
                        roomConnector.GetConnector().LockDoor();
                    }
                    roomConnector.LockDoor();
                }
            }
        }
        if (isBeaten && player != null)
        {
            player.GetComponent<BlakeCharacter>().SetRespawnPosition(GetSpawnPointPosition());
        }

        var toDelete = new List<GameObject>();
        foreach (var weapon in instantiatedWeapons)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                toDelete.Add(weapon);
            }
        }

        foreach (var delete in toDelete)
        {
            instantiatedWeapons.Remove(delete);
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
        if (isBeaten) return;
        foreach (RoomConnector roomConnector in doors)
        {
            roomConnector.UnlockDoor();
            if (roomConnector.GetConnector() != null)
            {
                roomConnector.GetConnector().UnlockDoor();
            }
        }
        minimapRoom.ForgetRoom();

        foreach(RoomTrigger rt in triggers)
        {
            rt.Reset();
        }

        foreach (RoomOverlapTrigger rt in fogTriggers)
        {
            rt.Reset();
        }
        Invoke("ResetEnemies", 0.5f);

        foreach (var weapon in instantiatedWeapons.ToArray())
        {
            Destroy(weapon);
            instantiatedWeapons.Remove(weapon);
        }

        if (player != null)
        {
            player.GetComponent<BlakeCharacter>().onRespawn -= ResetRoom;
        }
    }


    private void ResetEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
        spawnedEnemies.Clear();
        SpawnEnemies();
    }

    private void Update()
    {
        if(!isBeaten && isInitialized && (IsPlayerInsideFog() || IsPlayerInside()))
        {
            if(spawnedEnemies.Count == 0)
            {
                if (player == null) return;
                BeatLevel();
                foreach (RoomConnector roomConnector in doors)
                {
                    roomConnector.UnlockDoor();
                    if (roomConnector.GetConnector() != null)
                    {
                        roomConnector.GetConnector().UnlockDoor();
                    }
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

        if (roomManager != null)
        {
            if (!isBeaten && roomManager.GetActiveRoom() == this && (!IsPlayerInside() && !IsPlayerInsideFog()) && spawnedEnemies.Count > 0)
            {
                ResetRoom();
            }
        }
    }

    public void ExitRoom()
    {
        if (player != null)
        {
            player.GetComponent<BlakeCharacter>().onRespawn -= ResetRoom;
        }
        if (IsPlayerInside()) return;
        /*if(roomManager.GetActiveRoom() == this)
        {
            roomManager.SetActiveRoom(null);

        }*/
        
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

    public bool IsPlayerInside()
    {
        foreach(RoomTrigger rt in triggers)
        {
            if(rt.IsPlayerInside()) return true;
        }
        return false;
    }

    public bool IsPlayerInsideFog()
    {
        foreach (RoomOverlapTrigger rt in fogTriggers)
        {
            if (rt.IsPlayerInside()) return true;
        }
        return false;
    }

    public void AddTrigger(RoomTrigger rt)
    {
        triggers.Add(rt);
    }

    public void AddFogTrigger(RoomOverlapTrigger rt)
    {
        fogTriggers.Add(rt);
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
        if (spawnPoint != null)
        {
            return spawnPoint.position;
        }
        else return transform.position;
    }

    public void AddSpawnedWeapon(GameObject weapon)
    {
        instantiatedWeapons.Add(weapon);
    }

    private void OnDestroy()
    {
        if (player == null) return;
        player.GetComponent<BlakeCharacter>().onRespawn -= ResetRoom;
    }

    ~Room() {
        player.GetComponent<BlakeCharacter>().onRespawn -= ResetRoom;

    }
}
