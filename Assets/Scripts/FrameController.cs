using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    public static bool hasFrame = false;

   void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
