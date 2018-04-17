
public interface IPoolable
{
    /// <summary>
    /// Called when borrowed from the pool of objects, may be before or after Unity methods such as Start or Awake.
    /// Here you should active the game object, parent/unparent it etc.
    /// </summary>
    void Begin();

    /// <summary>
    /// Called when returning to the pool. This should remove any resources held (unless they can be reused) and prepare this object
    /// to have Begin() called again.
    /// Here you should deactiveate/hide the game object, unparent/parent it etc.
    /// </summary>
    void End();
}
