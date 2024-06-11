using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayer : MonoBehaviour
{
    // Start is called before the first frame update
    public void Change(int layerNumber)
    {
        gameObject.layer = layerNumber;
    }
}
