using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{


    TMPro.TMP_Text collect;
    // Start is called before the first frame update
    void Start()
    {
        collect = GetComponent<TMPro.TMP_Text>();
        collect.text = "Hello, WOrld";
    }
}
