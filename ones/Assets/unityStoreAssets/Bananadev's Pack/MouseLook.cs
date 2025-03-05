using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook instance;

    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor = true;
    [Space]

    public float sensX = 2;
    public float sensY = 2;

    private Vector2 sensitivity = new Vector2(2, 2);
    [Space]
    public Vector2 smoothing = new Vector2(3, 3);

    [Header("First Person")]
    public GameObject characterBody;

    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;

    private Vector2 mouseDelta;
    private float defaultSens = 2;

    private bool rotatePlayerAllowed = true;
    Movement movescript;
    [HideInInspector]
    public bool scoped;

    void Start()
    {
        movescript = GetComponentInParent<Movement>();
        loadSettings();
        instance = this;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
        
        if (lockCursor)
            LockCursor();

    }

    public void loadSettings(){
        //Debug.Log("loaded settings - mouselook");
        float testSens = PlayerPrefs.GetFloat("SensXY", defaultSens);
        sensitivity = new Vector2(testSens, testSens);
    }

    public void setSensitivity(float _sensX, float _sensY){
        sensitivity = new Vector2(_sensX, _sensY);
    }

    public void LockCursor()
    {
        // make the cursor hidden and locked
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void UnlockCursor(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void UnlockCursorStatic(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockPlayerMouseMovementRotation(){
        rotatePlayerAllowed = false;
    }
    public void UnlockPlayerMouseMovementRotation(){
        rotatePlayerAllowed = true;
    }

    void Update(){
        // Get raw mouse input for a cleaner reading on more sensitive mice.
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //mouseDelta.y += movescript.recoildegrees/25;
        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        //mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
    }

    void FixedUpdate()
    {
        mouseDelta.y += movescript.recoildegrees/25;
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
        if(rotatePlayerAllowed){
            // Allow the script to clamp based on a desired target value.
            var targetOrientation = Quaternion.Euler(targetDirection);
            var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

            

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;

            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
                transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

                
                
            // If there's a character body that acts as a parent to the camera
            if (characterBody)
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
                characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
            }
            else //NOT AN ISSUE
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }
        }
        movescript.recoildegrees = 0;
    }
}
