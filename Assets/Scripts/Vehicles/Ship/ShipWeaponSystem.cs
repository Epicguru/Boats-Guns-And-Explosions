
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipWeaponSystem : WeaponSystem
{
    public Ship Ship
    {
        get
        {
            if(_ship == null)
            {
                _ship = GetComponent<Ship>();
            }
            return _ship;
        }
    }
    private Ship _ship;

    public void GetUnitOptions(List<UnitOption> options)
    {
        // No options when in invalid state such as sunk.
        if (!IsValidState())
            return;


    }

    private bool IsValidState()
    {
        if (Ship.Damage.IsSunk())
            return false;

        return true;
    }
}