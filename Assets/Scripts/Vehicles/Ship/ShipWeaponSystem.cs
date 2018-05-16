
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

        if (!Firing)
        {
            options.Add(UnitOption.WS_FIRE_AT);
        }
        else
        {
            options.Add(UnitOption.WS_CEASE_FIRE);
        }
    }

    [Server]
    public void ExecuteOption(OptionAndParams item)
    {
        // No options when in invalid state such as sunk.
        if (!IsValidState())
            return;

        if(item.Option == UnitOption.WS_FIRE_AT && !Firing)
        {
            // We are now firing, determine the target...
            Firing = true;

            bool isUnit = item.Params.IsOfType<GameObject>(0);
            if (isUnit)
            {
                // Targeting a unit...
                var go = item.Params.Get<GameObject>(0);
                if (go != null)
                {
                    Unit unit = go.GetComponent<Unit>();
                    TargetUnit = unit;
                }
            }
            else
            {
                // Targeting a static position.
                TargetUnit = null;
                TargetPosition = item.Params.Get<Vector2>(0);
            }
        }
        if(item.Option == UnitOption.WS_CEASE_FIRE && Firing)
        {
            Firing = false;
            TargetUnit = null;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!isServer)
            return;

        if (!IsValidState())
        {
            Firing = false;
            TargetUnit = null;
        }
    }

    private bool IsValidState()
    {
        if (Ship.Damage.IsSunk())
            return false;

        return true;
    }
}