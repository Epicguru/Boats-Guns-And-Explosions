
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipUnit : Unit
{
    public Ship Ship
    {
        get
        {
            if (_ship == null)
            {
                _ship = GetComponent<Ship>();
            }
            return _ship;
        }
    }
    private Ship _ship;

    public override void SetMovementTarget(Vector2 target)
    {
        Ship.Navigation.TargetPos = target;
        Ship.Navigation.Activate();
    }
}