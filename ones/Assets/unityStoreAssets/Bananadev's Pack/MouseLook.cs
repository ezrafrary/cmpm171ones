using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook instance;

    [Header("Settings")]
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor = true;
    public bool enableSmoothing = false;
    public Vector2 smoothing = new Vector2(3f, 3f);

    public float sensX = 2f;
    public float sensY = 2f;

    private Vector2 sensitivity = new Vector2(2f, 2f);

    [Header("First Person")]
    public GameObject characterBody;

    private Vector2 targetDirection;
    private Vector2 targetCharacterDirection;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;
    private Vector2 mouseDelta;

    private float defaultSens = 2f;

    private bool rotatePlayerAllowed = true;

    Movement movescript;
    [HideInInspector]
    public bool scoped;

    void Start()
    {
        movescript = GetComponentInParent<Movement>();
        loadSettings();
        instance = this;

        targetDirection = transform.localRotation.eulerAngles;
        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;

        if (lockCursor)
            LockCursor();
    }

    public void loadSettings()
    {
        float testSens = PlayerPrefs.GetFloat("SensXY", defaultSens);
        sensitivity = new Vector2(testSens, testSens);
        sensX = testSens;
        sensY = testSens;
    }

    public void setSensitivity(float _sensX, float _sensY)
    {
        sensitivity = new Vector2(_sensX, _sensY);
        sensX = _sensX;
        sensY = _sensY;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void UnlockCursorStatic()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockPlayerMouseMovementRotation()
    {
        rotatePlayerAllowed = false;
    }

    public void UnlockPlayerMouseMovementRotation()
    {
        rotatePlayerAllowed = true;
    }

    void Update()
    {
        if (!rotatePlayerAllowed) return;

        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta.y += movescript.recoildegrees / 25f;
        mouseDelta.x += Random.Range(-movescript.horizontalRecoilDegrees, movescript.horizontalRecoilDegrees) / 25f;

        movescript.recoildegrees = 0;
        movescript.horizontalRecoilDegrees = 0;

        // Apply sensitivity
        mouseDelta.x *= sensX;
        mouseDelta.y *= sensY;

        // Clamp for safety
        mouseDelta.x = Mathf.Clamp(mouseDelta.x, -10f, 10f);
        mouseDelta.y = Mathf.Clamp(mouseDelta.y, -10f, 10f);

        if (enableSmoothing)
        {
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
            _mouseAbsolute += _smoothMouse;
        }
        else
        {
            _mouseAbsolute += mouseDelta;
        }

        if (clampInDegrees.x < 360f)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        if (clampInDegrees.y < 360f)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, Vector3.right) * targetOrientation;

        if (characterBody)
        {
            characterBody.transform.localRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up) * targetCharacterOrientation;
        }
        else
        {
            transform.localRotation *= Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        }
    }
}
