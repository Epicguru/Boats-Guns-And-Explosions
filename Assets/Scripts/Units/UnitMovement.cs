
using UnityEngine;
using UnityEngine.Networking;

public class UnitMovement : NetworkBehaviour
{
    // Used to move units that are owned by the player. This is a player script.

    public float ActiveTime = 0.5f;

    private TargetArrow arrow;
    private float timer;
    private bool removed;

    public void Update()
    {
        if (!isLocalPlayer)
            return;

        // When move button pressed, place movement marker and move currently selected units.
        if(InputManager.IsDown("Move To"))
        {
            if(arrow == null)
            {
                arrow = TargetArrowPool.Instance.GetFromPool();
                arrow.ReturnToPool = false;
            }
            arrow.PlaceAt(InputManager.MousePos);
            timer = 0f;
            removed = false;

            // Call the data-oriented move units function.
            // Works on both client and server.
            this.MoveUnits(InputManager.MousePos);
        }

        timer += Time.unscaledDeltaTime;

        if(timer >= ActiveTime && !removed && arrow != null)
        {
            removed = true;
            arrow.Remove();
        }
    }

    public void MoveUnits(Vector2 target)
    {
        if (!isClient)
            return;
        if (!isLocalPlayer)
            return;

        // Check that there are actually some selected units...
        int selected = Unit.CurrentlySelected.Count;

        if(selected > 0)
        {
            // Tell the server that we want to move units to the target position.
            // Assume (TODO) that we own the units.
            // Assume (TODO) that the target position is valid.

            // Get all the current units.
            Unit[] units = Unit.CurrentlySelected.ToArray();

            if (isServer)
            {
                MoveUnits_Server(units, target);
            }
            else
            {
                MoveUnits_Client(units, target);
            }
        }
    }

    [Server]
    private void MoveUnits_Server(Unit[] units, Vector2 pos)
    {
        // Move all units to position, on server.
        Unit.MoveUnitsTo(units, pos);
    }

    [Client]
    private void MoveUnits_Client(Unit[] units, Vector2 pos)
    {
        // We have a list of units, but the networking system can't transfer an array of Units.
        // We will turn it into an array of GameObjects and then transfer that.

        GameObject[] gos = new GameObject[units.Length];
        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];
            gos[i] = unit == null ? null : unit.gameObject;
        }

        CmdMoveUnits(gos, pos);
    }

    [Command]
    private void CmdMoveUnits(GameObject[] gos, Vector2 pos)
    {
        // TODO validation.

        // Turn gameobjects back into units.
        Unit[] units = new Unit[gos.Length];

        for (int i = 0; i < gos.Length; i++)
        {
            var go = gos[i];
            if (go == null)
                continue;
            units[i] = go.GetComponent<Unit>();
        }

        MoveUnits_Server(units, pos);
    }
}