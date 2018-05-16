using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AttachmentManager))]
public class WeaponSystem : NetworkBehaviour
{
    public AttachmentManager Attachments
    {
        get
        {
            if(_attachments == null)
            {
                _attachments = GetComponent<AttachmentManager>();
            }
            return _attachments;
        }
    }
    private AttachmentManager _attachments;

    [SyncVar]
    public bool Firing;

    public Unit TargetUnit
    {
        get
        {
            return _targetUnit;
        }
        private set
        {
            _targetUnit = value;
        }
    }
    [SerializeField]
    [ReadOnly]
    private Unit _targetUnit;

    [SyncVar]
    public Vector2 TargetPosition;
    public bool IsTargetingUnit
    {
        get
        {
            return TargetUnit != null;
        }
    }

    private List<AttachmentSocket> Sockets = new List<AttachmentSocket>();

    public virtual void Update()
    {
        // Gather valid sockets.
        Sockets.Clear();
        foreach (var pair in Attachments.Sockets)
        {
            var s = pair.Value;
            if(s != null)
            {
                Sockets.Add(s);
            }
        }

        foreach (var s in Sockets)
        {
            var a = s.GetCurrentlyAttached();
            if(a != null)
            {
                // Calculate angle...
                float angle;
                if (IsTargetingUnit)
                {
                    angle = (a.transform.position - TargetUnit.transform.position).ToAngle();
                }
                else
                {
                    angle = ((Vector2)a.transform.position - TargetPosition).ToAngle();
                }

                // Is a cannon?
                var cannon = a.GetComponent<Cannon>();
                if (cannon != null)
                {
                    cannon.IsFiring = Firing;
                    if (Firing)
                    {
                        cannon.TargetAngle = angle;
                    }
                    else
                    {
                        cannon.TargetAngle = 0f;
                    }
                }
            }
        }
    }
}