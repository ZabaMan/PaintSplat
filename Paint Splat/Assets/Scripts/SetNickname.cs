using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class SetNickname : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
         GetComponent<TextMeshPro>().text = GetComponentInParent<PhotonView>().Owner.NickName;
        
    }
}
