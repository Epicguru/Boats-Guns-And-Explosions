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

    public bool UseLocalInput = true;

    private Vector2 oldMousePos;

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
                Vector2 direction = Vector2.zero;

                if(InputManager.IsPressed("Camera Right"))
                {
                    direction.x += 1;
                }
                if (InputManager.IsPressed("Camera Left"))
                {
                    direction.x -= 1;
                }

                if (InputManager.IsPressed("Camera Up"))
                {
                    direction.y += 1;
                }
                if (InputManager.IsPressed("Camera Down"))
                {
                    direction.y -= 1;
                }
                direction.Normalize();
                direction *= 20f; // General speed.
                direction *= Camera.orthographicSize / 50f;
                Camera.transform.Translate(direction * Time.unscaledDeltaTime);

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
        oldMousePos = InputManager.ScreenMousePos;
    }
}