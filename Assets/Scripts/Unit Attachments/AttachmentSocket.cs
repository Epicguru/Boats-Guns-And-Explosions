using UnityEngine;

[RequireComponent(typeof(NetParent))]
public class AttachmentSocket : MonoBehaviour
{
    public NetParent NetParent
    {
        get
        {
            if(_netParent == null)
            {
                _netParent = GetComponent<NetParent>();
            }
            return _netParent;
        }
    }
    private NetParent _netParent;

    public AttachmentSize MinSize = AttachmentSize.SMALL;
    public AttachmentSize MaxSize = AttachmentSize.HUGE;

    public string Name
    {
        get
        {
            return _name;
        }
    }
    [SerializeField]
    private string _name;

    public bool IsValid(Attachment a)
    {
        if (a == null)
            return false;

        // Check size.
        var size = a.Size;
        byte s = (byte)size;
        bool sizeValid = s >= (byte)MinSize && s <= (byte)MaxSize;
        if (!sizeValid)
            return false;

        return true;
    }

    public Attachment GetCurrentlyAttached()
    {
        foreach (var child in NetParent.Children)
        {
            if(child != null)
            {
                var a = child.GetComponent<Attachment>();

                if (a != null)
                    return a;
            }
        }
        return null;
    }
}