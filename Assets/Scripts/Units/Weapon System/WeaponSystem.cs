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
        protected set
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

    public float MaxFiringAngleOffset = 20f;

    private List<AttachmentSocket> Sockets = new List<AttachmentSocket>();

    public virtual void Update()
    {
        if (!isServer)
            return;

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
                    angle = (TargetUnit.transform.position - a.transform.position).ToAngle();
                }
                else
                {
                    angle = (TargetPosition - (Vector2)a.transform.position).ToAngle();
                }

                // Is a cannon?
                var cannon = a.GetComponent<Cannon>();
                if (cannon != null)
                {
                    float current = cannon.transform.eulerAngles.z;
                    float diff = Mathf.Abs(Mathf.DeltaAngle(current, angle));
                    bool inAngleRange = diff <= Mathf.Abs(MaxFiringAngleOffset);
                    cannon.IsFiring = Firing && inAngleRange;
                    if (Firing)
                    {
                        cannon.TargetAngle = angle;
                        cannon.Active = true;
                    }
                    else
                    {
                        cannon.TargetAngle = 0f;
                        cannon.Active = false;
                    }
                }
            }
        }
    }
}