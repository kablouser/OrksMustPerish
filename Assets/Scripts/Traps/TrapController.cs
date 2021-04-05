using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to player, place traps in the appropriate public game objects, and that's about it
/// </summary>
public class TrapController : MonoBehaviour
{
    public enum TrapControlMode { none, placing, deleting };

    public BuildingResourceManager buildingResourceManager;
    public WandWeapon wandWeapon;
    public GenericTrap[] traps;

    public LayerMask trapSlotsLayerMask;
    public Transform mainCamera;
    public float controlDistance = 100.0f;
    [Range(0,1)]
    public float refundAmount = 0.5f;

    public LayerMask characterLayerMask;

    private GenericTrap currentPlacingTrap;
    private TrapSlot currentTrapSlot;
    private TrapControlMode mode;

    public int PlacingTrapIndex { get; private set; }
    public string UIMessage { get; private set; }

    private const string Fire1 = "Fire1", Fire2 = "Fire2";
    private const string
        PlacingTrapsMessage = "Placing Trap (cost ${0})\n<size=20>(left-click to place, right-click to cancel)</size>",
        DeletingTrapsMessage = "Destroy Trap (refund ${0})\n<size=20>(left-click to destroy, right-click to cancel)</size>";        

    private void Awake()
    {
        PlacingTrapIndex = -1;
        UIMessage = string.Empty;
    }

    private void Update()
    {
        HotKeyTrap();

        TrapSlot lookingAt;
        switch (mode)
        {
            case TrapControlMode.placing:
                lookingAt = GetLookingAtTrapSlot();                
                if(lookingAt == null || lookingAt.IsTrapPlaced())
                {
                    currentPlacingTrap.gameObject.SetActive(false);
                }
                else
                {
                    currentPlacingTrap.gameObject.SetActive(true);
                    currentPlacingTrap.PlacingUpdate(buildingResourceManager, characterLayerMask);
                    currentPlacingTrap.transform.position = lookingAt.transform.position;
                    currentPlacingTrap.transform.rotation = lookingAt.transform.rotation;
                    if (Input.GetButtonDown(Fire1))
                    {
                        if (currentPlacingTrap.TryPlace(buildingResourceManager, characterLayerMask, lookingAt))
                        {
                            FindObjectOfType<AudioManager>().Play("PlaceTrap");
                            break;
                        }
                    }
                }
                break;

            case TrapControlMode.deleting:
                lookingAt = GetLookingAtTrapSlot();
                SetDeleteTrapSlot(lookingAt);
                if (lookingAt != null && Input.GetButtonDown(Fire1))
                {
                    FindObjectOfType<AudioManager>().Play("TrapDestroy");
                    lookingAt.EndDeleteTrap(true, buildingResourceManager, refundAmount);
                }
                break;

            default:
                break;
        }
    }

    private void HotKeyTrap()
    {
        for (int i = 0, key = (int)KeyCode.Alpha1;
            i < traps.Length && key < (int)KeyCode.Alpha9 + 2;
            ++i, ++key)
        {
            if (key == (int)KeyCode.Alpha9 + 1)
                key = (int)KeyCode.Alpha0;
            if (Input.GetKeyDown((KeyCode)key))
            {
                // toggle
                if (mode == TrapControlMode.placing && currentPlacingTrap == traps[i])
                    SetMode(TrapControlMode.none);
                else
                {
                    SetMode(TrapControlMode.placing);
                    SetPlaceTrap(traps[i]);
                    PlacingTrapIndex = i;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // toggle
            if(mode == TrapControlMode.deleting)
                SetMode(TrapControlMode.none);
            else
                SetMode(TrapControlMode.deleting);
        }

        if (Input.GetButtonDown(Fire2))
        {
            // exit out of any mode
            if (mode != TrapControlMode.none)
                SetMode(TrapControlMode.none);
        }
    }

    private void SetPlaceTrap(GenericTrap trap)
    {
        if (currentPlacingTrap == trap)
            return;

        if (currentPlacingTrap != null)
            currentPlacingTrap.gameObject.SetActive(false);

        currentPlacingTrap = trap;

        if (trap != null)
        {
            trap.gameObject.SetActive(true);
            UIMessage = string.Format(PlacingTrapsMessage, trap.cost);
        }            
    }

    private void SetDeleteTrapSlot(TrapSlot slot)
    {
        if (currentTrapSlot == slot)
            return;

        if (currentTrapSlot != null)
            currentTrapSlot.EndDeleteTrap(false, buildingResourceManager, refundAmount);

        currentTrapSlot = slot;

        if (slot != null)
            slot.StartDeleteTrap();

        if (mode == TrapControlMode.deleting)
        {
            int refundCost = 0;
            if (slot != null && slot.GetPlacedTrap() != null)
                refundCost = slot.GetPlacedTrap().GetRefundCost(refundAmount);
            UIMessage = string.Format(DeletingTrapsMessage, refundCost);
        }
    }

    private void SetMode(TrapControlMode mode)
    {
        if (this.mode != mode)
        {
            switch(this.mode)
            {
                case TrapControlMode.none:
                    wandWeapon.enabled = false;
                    break;
                case TrapControlMode.placing:
                    SetPlaceTrap(null);
                    PlacingTrapIndex = -1;
                    break;
                case TrapControlMode.deleting:
                    SetDeleteTrapSlot(null);
                    break;
                default:
                    break;
            }
            switch(mode)
            {
                case TrapControlMode.none:
                    wandWeapon.enabled = true;
                    UIMessage = string.Empty;
                    break;
                case TrapControlMode.deleting:
                    UIMessage = string.Format(DeletingTrapsMessage, 0);
                    break;
                default:
                    break;
            }
        }
        this.mode = mode;
    }

    private TrapSlot GetLookingAtTrapSlot()
    {
        if (Physics.Raycast(
            new Ray(mainCamera.position, mainCamera.forward),
            out RaycastHit hit,
            controlDistance,
            trapSlotsLayerMask))
        {
            TrapSlot trapSlot = hit.collider.GetComponent<TrapSlot>();
            if (trapSlot != null)
                return trapSlot;
        }
        return null;
    }
}
