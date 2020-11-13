using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttachToBoard : MonoBehaviourPunCallbacks
{
    
    public void Awake()
    {
        object[] data = photonView.InstantiationData;
        GetComponent<SpriteRenderer>().color = new Color((float)data[0], (float)data[1], (float)data[2]);
        var board = GameObject.FindWithTag("Board").transform;
        transform.SetParent(board);
        var pos = (Vector3)data[3] - (Vector3)data[4];
        transform.position = board.position + pos;
    }
}
