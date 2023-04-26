using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerGameplayUIManager : MonoBehaviour
{
    [SerializeField] private FloorManager _floorManager;
    [SerializeField] private MinimapCameraFollow minimapCamera;
    //[SerializeField] private PlayerUIObject playerUI;
    [Space]
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI bulletsLeft;
    [SerializeField] private GameObject interactUI;

    private GameObject player;
    private WeaponsManager _weaponsManager;
    private PlayerInputController playerInputController;
    private PlayerInteractables playerInteractables;

    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
    }

    private void Start()
    {
        _floorManager.FloorGeneratorEnd += FloorManagerOnFloorGeneratorEnd;
        playerInputController.shootEvent += BulletsLeftUI;
    }

    private void FloorManagerOnFloorGeneratorEnd(Transform playerTransform)
    {
        if (player == null || _weaponsManager == null)
        {
            player = playerTransform.gameObject;
            _weaponsManager = player.GetComponent<WeaponsManager>();
            _weaponsManager.changeWeaponEvent += RefreshUI;
            RefreshUI();
        }
        
        minimapCamera.SetPlayer(playerTransform);
        //playerTransform.GetComponent<UIPlayerController>().SetUI(playerUI);
        //minimapCamera.SetPlayer(playerTransform);
        
        playerInteractables = player.GetComponent<PlayerInteractables>();
        playerInteractables.SetInteractUIReference(interactUI);
    }

    private void RefreshUI()
    {
        WeaponNameUI();
        BulletsLeftUI();
    }
    

    private void WeaponNameUI()
    {
        weaponName.text = _weaponsManager.GetWeaponDefinition(_weaponsManager.ActiveWeaponIndex).weaponName;
    }

    private void BulletsLeftUI()
    {
        if (weaponName.text == _weaponsManager.defaultWeapon.weaponName)
        {
            bulletsLeft.text = "∞";
        }
        else
        {
            Weapon weapon = _weaponsManager.GetIWeapon(_weaponsManager.ActiveWeaponIndex).GetGameObject().GetComponent<Weapon>();
            bulletsLeft.text = weapon.BulletsLeft.ToString();
        }
    }
    
}
