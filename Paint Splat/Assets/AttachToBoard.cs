using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttachToBoard : MonoBehaviourPunCallbacks
{
    
    public void Awake()
    {
        transform.SetParent(GameObject.FindWithTag("Board").transform);
    }
}
