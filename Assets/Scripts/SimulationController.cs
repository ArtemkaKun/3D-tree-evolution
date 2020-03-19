﻿using UnityEngine;

public class SimulationController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WorldController.SwitchRun();
            CameraController.SwitchMove();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
}
