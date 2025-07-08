using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main_menu_manager : MonoBehaviour
{
   public GameObject first;
   public GameObject second;
   public GameObject three;
   
    public void play()
    {
         first.SetActive(false);
         second.SetActive(true);
    }
    public void exit_game()
    {
        Application.Quit();
    }
    public void OK1()
    {three.SetActive(true);
         second.SetActive(false);}
    public void OK2()
    {
        SceneManager.LoadScene("LV1");
    }
}
