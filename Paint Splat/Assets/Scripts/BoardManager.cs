using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoardManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private float directionalIntervals = 0.3f;
    private float currentTime = 0.0f;
    private float moveSpeed = 0.1f;
    private Vector2 moveTarget; // (x, y)

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        transform.position = Vector2.MoveTowards(transform.position, moveTarget, moveSpeed);
        currentTime += Time.deltaTime;
        if(currentTime >= directionalIntervals)
        {
            currentTime = 0.0f;
            RandomDirection();
        }
        
    }

    private void RandomDirection()
    {
        moveTarget = new Vector2(Random.Range(-6.5f, 6.5f), Random.Range(-5f, 5f));
    }
}

