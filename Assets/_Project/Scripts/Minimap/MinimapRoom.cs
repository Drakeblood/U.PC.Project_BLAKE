using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinimapRoom : MonoBehaviour
{
    private enum RoomState
    {
        Unseen,
        NotVisited,
        Visiting,
        Visited
    }

    [SerializeField]
    private Material minimapUnseen;

    [SerializeField]
    private Material minimapNotVisited;

    [SerializeField]
    private Material minimapVisiting;

    [SerializeField]
    private Material minimapVisited;

    [SerializeField]
    private MeshRenderer[] meshRenderers;

    private RoomState state = RoomState.Unseen;

    private void Start()
    {
        ChangeMaterial(minimapUnseen);
    }
    public void ShowRoom()
    {
        if(state == RoomState.Unseen)
        {
            state = RoomState.NotVisited;
            ChangeMaterial(minimapNotVisited);
        }
    }

    public void VisitRoom()
    {
        if(state == RoomState.NotVisited || state == RoomState.Unseen)
        {
            state = RoomState.Visiting;
            ChangeMaterial(minimapVisiting);
        }
    }

    public void CompleteRoom()
    {
            state = RoomState.Visited;
            ChangeMaterial(minimapVisited);
    }

    public void ForgetRoom()
    {
            state = RoomState.NotVisited;
            ChangeMaterial(minimapNotVisited);
    }

    private void ChangeMaterial(Material mat)
    {
        foreach(MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = mat;
        }
    }
}
