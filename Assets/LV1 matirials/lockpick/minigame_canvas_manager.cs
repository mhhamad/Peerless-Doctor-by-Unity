using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class minigame_canvas_manager : MonoBehaviour
{
   public GameObject first;
   public GameObject second;
   public GameObject last;

public void firstaction()
{
    first.SetActive(false);
    second.SetActive(true);
    Time.timeScale = 0;
}
public void secondaction()
{
    second.SetActive(false);
    last.SetActive(true);
    Time.timeScale = 1;}

}
