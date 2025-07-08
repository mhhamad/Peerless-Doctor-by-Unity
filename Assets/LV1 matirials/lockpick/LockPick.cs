using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;  // For UI touch detection

public class LockPick : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                   // Camera for converting world to screen coordinates.
    public Transform rotationPivot;      // The pivot around which the pin rotates (center of the lock).

    [Header("Settings")]
    public float maxAngle = 90f;         // Maximum absolute angle allowed.
    public float unlockTolerance = 10f;  // Acceptable error (in degrees) when checking the rotation.

    // Internal variables.
    private float currentAngle = 0f;     // The current rotation angle (relative to the upward direction, computed from input).
    private float unlockAngle;           // The secret angle required to unlock the lock.

    void Start()
    {
        // Generate a new secret angle at startup.
        GenerateNewUnlockAngle();
    }

    void Update()
    {
        // Get input (touch or mouse) ignoring UI interactions.
        Vector3 inputPos = GetInputPosition();

        // Only update the rotation if there is valid input.
        if (inputPos != Vector3.zero)
        {
            RotatePin(inputPos);
        }
    }

    // Returns the first input position (touch or mouse) not over a UI element.
    Vector3 GetInputPosition()
    {
        // Check for touch input.
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return touch.position;
                }
            }
        }
        // Fallback to mouse input.
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            return Input.mousePosition;
        }
        return Vector3.zero;
    }

    // Rotates the pin around the specified pivot based on the input position.
    void RotatePin(Vector3 inputPos)
    {
        // Use the pivot's screen position as the rotation center.
        Vector3 pivotScreenPos = cam.WorldToScreenPoint(rotationPivot.position);
        Vector3 dir = inputPos - pivotScreenPos;
        if (dir == Vector3.zero)
            return;

        // Calculate the angle between the input direction and the upward vector.
        float angle = Vector3.Angle(dir, Vector3.up);
        Vector3 cross = Vector3.Cross(Vector3.up, dir);
        if (cross.z < 0)
            angle = -angle; // Make angle negative if needed.

        // Clamp the computed angle so it stays within [-maxAngle, maxAngle].
        float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

        // Calculate the change in angle since the last frame.
        float deltaAngle = clampedAngle - currentAngle;
        currentAngle = clampedAngle;

        // Rotate the pin around the pivot by the delta angle.
        transform.RotateAround(rotationPivot.position, Vector3.forward, deltaAngle);
    }

    // Called via a UI Check Button. It tests if the current rotation is within tolerance of the unlock angle.
    public void OnCheckButtonPressed()
    {
        if (Mathf.Abs(currentAngle - unlockAngle) <= unlockTolerance)
        {
            Debug.Log("Unlocked!");
            // Optionally, trigger any unlocking actions (e.g., open a door, play a sound, etc.)
            GenerateNewUnlockAngle();  // Set a new secret angle for subsequent attempts.
        }
        else
        {
            Debug.Log("Wrong angle. Try again!");
            // On a wrong attempt, the pin stays at its current rotation.
        }
    }

    // Randomly sets a new unlock angle within the allowed range.
    void GenerateNewUnlockAngle()
    {
        unlockAngle = Random.Range(-maxAngle + unlockTolerance, maxAngle - unlockTolerance);
        Debug.Log("New unlock angle is: " + unlockAngle);
    }
}
