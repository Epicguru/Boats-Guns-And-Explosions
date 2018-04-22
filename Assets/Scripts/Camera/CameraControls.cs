using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Camera Camera;
    public Transform Target;
    public float CameraSizeToZ = 1f;

    [Header("Controls")]
    [Range(0.5f, 300f)]
    public float TargetSize = 30f;
    public float LerpSpeed = 5f;
    public Vector2 SizeLimits = new Vector2(1f, 100f);

    private Vector2 mousePanStart;

    public bool UseLocalInput = true;

    private bool inDrag = false;

    public void Update()
    {
        if (Camera == null)
            return;

        // Camera size.
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, TargetSize, Time.unscaledDeltaTime * LerpSpeed);

        Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.orthographicSize * CameraSizeToZ);

        // Target following.
        if(Target != null)
        {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, Camera.orthographicSize * CameraSizeToZ);
        }
        else
        {
            if (UseLocalInput)
            {
                // Recieve movement input.
                DoPanInput();

                if (Input.mouseScrollDelta != Vector2.zero)
                {
                    if(Input.mouseScrollDelta.y < 0)
                    {
                        TargetSize *= 1.1f;
                    }
                    else
                    {
                        TargetSize /= 1.1f;
                    }
                }
            }
        }

        TargetSize = Mathf.Clamp(TargetSize, SizeLimits.x, SizeLimits.y);
        Camera.orthographicSize = Mathf.Clamp(Camera.orthographicSize, SizeLimits.x, SizeLimits.y);
    }

    private bool DoPanInput()
    {
        if(InputManager.IsDown("Camera Pan"))
        {
            inDrag = true;
            mousePanStart = InputManager.MousePos;
        }
        if(InputManager.IsUp("Camera Pan"))
        {
            inDrag = false;
        }
        if (inDrag)
        {
            Camera.transform.Translate(mousePanStart - InputManager.MousePos);
            return true;
        }
        else
        {
            return false;
        }
    }
}