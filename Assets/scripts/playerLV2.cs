using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerLV2 : MonoBehaviour
{
    // Define the distance between lanes. For example, if laneDistance is 3,
    // then left lane x = -3, center lane x = 0, right lane x = 3.
    public float laneDistance = 35f;  
    // Starting lane index: 0 = left, 1 = center, 2 = right.
    private int currentLane = 1;

    // How fast the player transitions between lanes.
    public float smoothTime = 10f;

    // Forward movement speed.
    public float forwardSpeed = 35f;

    // For swipe detection.
    public float swipeThreshold = 50f;

    public GameObject LV2_lastscene;
    public GameObject neighborhood ;
    public GameObject pause_scene;
     Animator animator;   
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    // Stop flag if colliding with an obstacle.
    private bool isStop = false;
    void Start()
    {
         animator = GetComponent<Animator>();
    }
    void Update()
    {

        // Always move forward if not stopped.
        if (!isStop)
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Smoothly update the lateral (x) position.
        Vector3 desiredPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothTime * Time.deltaTime);

        HandleSwipe();
    }

    public void load_scene3()
    {
        SceneManager.LoadScene("LV3");
    }
        public void Pause(){

        pause_scene.SetActive(true);
        Time.timeScale = 0;
        
    }
    public void Resume(){
        pause_scene.SetActive(false);
        Time.timeScale = 1;
        
    }
    public void main_menu(){
        Time.timeScale = 1;
        SceneManager.LoadScene("main menu");
    }
    private void HandleSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                float swipeDeltaX = touch.position.x - touchStartPos.x;
                if (Mathf.Abs(swipeDeltaX) > swipeThreshold)
                {
                    if (swipeDeltaX > 0)
                    {
                        ChangeLane(1); // swipe right
                    }
                    else
                    {
                        ChangeLane(-1); // swipe left
                    }
                    isSwiping = false;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isSwiping = false;
            }
        }
    }

    // Change lane based on direction (-1 for left, 1 for right)
    private void ChangeLane(int direction)
    {
        // Update current lane index, clamped to the range 0 to 2.
        currentLane = Mathf.Clamp(currentLane + direction, 0, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SceneManager.LoadScene("LV2");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            isStop = true;
        if (collision.gameObject.CompareTag("kills"))
            SceneManager.LoadScene("LV2");
        if (collision.gameObject.CompareTag("kid"))
            {LV2_lastscene.SetActive(true);
            neighborhood .SetActive(false);
            }
        
        if (collision.gameObject.CompareTag("start"))
            {forwardSpeed *= 3;
            animator.SetBool("runing", true);}
        if (collision.gameObject.CompareTag("end"))
            {forwardSpeed /= 3;
            animator.SetBool("runing", false);}
        }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            isStop = false;
    }

    
}
