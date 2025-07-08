using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnivStudios
{
    public class PadlockController : MonoBehaviour
    {
        public GameObject curr_lock;
        public GameObject next_lock;

        [Header("Padlock Settings")]
        [Tooltip("Enter the correct combination as an integer (e.g., 1234).")]
        public int combination;
        [Tooltip("Assign the four gear objects (child objects) in the correct order.")]
        public Transform[] gears;  // Assign 4 gears in the Inspector

        // Internal arrays to store the current numbers and the target combination
        private int[] currentNumbers;
        private int[] targetCombination;

        // Flags to control interaction
        private bool padLockStop = false;
        private bool isRotating = false;

        void Start()
        {
            // Convert the combination integer to an array of digits (e.g., 1234 -> [1,2,3,4])
            targetCombination = combination.ToString()
                                           .Select(c => int.Parse(c.ToString()))
                                           .ToArray();

            // Ensure the target combination has exactly 4 digits
            if(targetCombination.Length != 4)
            {
                Debug.LogError("Combination must be exactly 4 digits!");
            }

            // Initialize current numbers for each gear (default to 0)
            currentNumbers = new int[gears.Length];
            for (int i = 0; i < currentNumbers.Length; i++)
            {
                currentNumbers[i] = 0;
            }
        }

        void Update()
        {
            if (padLockStop || isRotating)
                return;

            // Capture input position from mouse (desktop) or touch (mobile)
            Vector3 inputPosition = Vector3.zero;

            // For editor and desktop
            if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
            }
            
            // For mobile: detect touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    inputPosition = touch.position;
                }
            }

            if (inputPosition != Vector3.zero)
            {
                Ray ray = Camera.main.ScreenPointToRay(inputPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if one of the gears was tapped
                    for (int i = 0; i < gears.Length; i++)
                    {
                        if (hit.transform == gears[i])
                        {
                            StartCoroutine(RotateGear(i));
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator RotateGear(int gearIndex)
        {
            isRotating = true;
            
            // Rotate the gear gradually: 12 steps of 3 degrees each (total 36°)
            for (int i = 0; i < 12; i++)
            {
                gears[gearIndex].Rotate(0, 3, 0);
                yield return new WaitForSeconds(0.01f);
            }

            // Update the number for this gear, wrapping around after 9
            currentNumbers[gearIndex] = (currentNumbers[gearIndex] + 1) % 10;

            // Check if the current combination is correct
            if (IsCombinationCorrect())
            {
                padLockStop = true;
                StartCoroutine(OpenPadlock());
            }

            isRotating = false;
        }

        // Checks whether the current numbers match the target combination
        private bool IsCombinationCorrect()
        {
            if (currentNumbers.Length != 4 || targetCombination.Length != 4)
                return false;

            for (int i = 0; i < 4; i++)
            {
                if (currentNumbers[i] != targetCombination[i])
                    return false;
            }
            return true;
        }

        IEnumerator OpenPadlock()
        {
            // Instead of playing an animation, simply log the success.
            curr_lock.SetActive(false);
            next_lock.SetActive(true);

            Debug.Log("Padlock solved! Door is now unlocked.");
            Invoke("LoadScene", 2);
           yield return null;

    }

    void LoadScene()
    {
        SceneManager.LoadScene("LV2");
    }
    }
}
