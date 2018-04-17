using UnityEngine;

public class RigidbodyDrag : MonoBehaviour
{
    public Camera Camera;
    public Rigidbody2D CurrentlySelected;

    public void Update()
    {
        if (Camera == null)
            return;

        if(CurrentlySelected == null)
        {
            Physics2D.GetRayIntersectionNonAlloc()
        }
        else
        {

        }
    }

    public Ray MakeRay()
    {
        if (Camera == null)
            return null;


    }
}