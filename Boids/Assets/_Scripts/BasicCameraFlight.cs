using UnityEngine;

public class BasicCameraFlight : MonoBehaviour
{
    public KeyCode fastKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.LeftControl;
    
    public float fastSpeed = 1;
    public float defaultSpeed = 1;
    public float slowSpeed = 1;
    public float turn = 1;
    
    private Vector3 _deltaPos = Vector3.zero;
    private bool _frozen;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Focus management
        if (Input.GetKey(KeyCode.Escape))
        {
            // Set frozen
            _frozen = true;
            
            // Update cursor visuals and lock state
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            // Skip all camera movement behavior if unfocused
            return;
        }
        if (Input.GetMouseButton(0))
        {
            // Set frozen
            _frozen = false;

            // Update cursor visuals and lock state
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (_frozen) return;
        
        // Reset change in position
        _deltaPos = Vector3.zero;
        
        // Determine speed and early exit for double with half (freeze the cam)
        float speed = defaultSpeed;
        bool fast = Input.GetKey(fastKey), slow = Input.GetKey(slowKey);
        if (fast && slow) return;
        if (fast) speed = fastSpeed;
        else if (slow) speed = fastSpeed;

        // Reduce calls to this.transform
        Transform t = transform;
        
        // Mouse movement
        Vector2 mouseDelta = Input.mousePositionDelta;
        Vector3 rotation = new Vector3(mouseDelta.y, -mouseDelta.x, 0) *
                           (turn * Time.deltaTime);
        t.rotation = Quaternion.Euler(t.rotation.eulerAngles - rotation);

        // Get new change in position
        if (Input.GetKey(KeyCode.W))
            _deltaPos += t.forward * (speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.S))
            _deltaPos -= t.forward * (speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.D))
            _deltaPos += t.right * (speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            _deltaPos -= t.right * (speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            _deltaPos += t.up * (speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q))
            _deltaPos -= t.up * (speed * Time.deltaTime);
        
        // Set new position
        t.position += _deltaPos;
    }
}
