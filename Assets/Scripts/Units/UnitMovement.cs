
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class UnitMovement : NetworkBehaviour
{
    // Used to move units that are owned by the player. This is a player script.

    public Player Player;
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
            // Assume (TODO) that the target position is valid.

            // Get all the current units.
            Unit[] units = Unit.CurrentlySelected.ToArray();

            // Filter out the ones we do not own.
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                if(unit.Faction != Player.Faction)
                {
                    units[i] = null;
                }
            }

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
        // Already removed units that are not owned by the host. (Because this is a host method).
        Unit.MoveUnitsTo(units, pos);
    }

    [Client]
    private void MoveUnits_Client(Unit[] units, Vector2 pos)
    {
        // We have a list of units, but the networking system can't transfer an array of Units.
        // We will turn it into an array of GameObjects and then transfer that.

        // The unowned units have theoretically already been removed, but the server performs more validation on the other end.

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
        // TODO position validation.

        // Turn gameobjects back into units.
        Unit[] units = new Unit[gos.Length];

        for (int i = 0; i < gos.Length; i++)
        {
            var go = gos[i];
            if (go == null)
                continue;

            // Make sure that the player that requested this movement owns the selected unit.
            // On the client this test should have already been done, but ya'know, hackers and all that.

            var unit = go.GetComponent<Unit>();
            if(unit.Faction != this.Player.Faction)
            {
                Debug.LogError("The player '{0}' requested to move a unit they do not own, client validation should have removed that possibility, hmmm. BANHAMMER!");
            }
            else
            {
                units[i] = unit;
            }
        }

        MoveUnits_Server(units, pos);
    }
}