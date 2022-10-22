using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Locking the Cursor of the mouse to the center of the screen when application is focused.
/// </summary>
public class CenterMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
