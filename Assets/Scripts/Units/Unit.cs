
using UnityEngine;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour
{
    [Tooltip("Display name.")]
    public string Name;

    public ushort ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    [Tooltip("Internal ID")]
    private ushort _id;

    public Faction Faction { get; private set; }
    public bool RenderBounds;

    public SpriteRenderer Bounds;

    private SpriteRenderer selBounds;

    public void Update()
    {
        // Completely local and client sided. Just visual.
        if (RenderBounds)
        {
            if(Bounds == null)
            {
                Bounds = GetComponentInChildren<SpriteRenderer>();
                if(Bounds == null)
                {
                    Debug.LogWarning("No Bounds sprite renderer assigned in editor, and no SpriteRenderers were found at runtime on children. The bounds are not rendererd. ({0})".Form(name));
                    return;
                }
            }
            if(selBounds == null)
            {
                // Create selection bounds...
                var sel = SelectionBoundsPool.Instance.GetFromPool();
                selBounds = sel.Renderer;
            }
            
            if(selBounds != null)
            {
                // Set position of selection bounds.
                selBounds.color = Color.green;
                selBounds.transform.position = Bounds.bounds.min - Bounds.bounds.size * 0.1f;
                selBounds.size = Bounds.bounds.size * 1.2f;
            }
        }
        else
        {
            if(selBounds != null)
            {
                SelectionBoundsPool.Instance.ReturnToPool(selBounds.GetComponent<SelectionBounds>());
                selBounds = null;
            }
        }
    }
}