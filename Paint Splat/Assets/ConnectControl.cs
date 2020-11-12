using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectControl : MonoBehaviourPunCallbacks
{
    public Joystick ConnectJoystick()
    {
        if (GetComponent<FixedJoystick>() && photonView.IsMine)
            return GetComponent<FixedJoystick>();

        Debug.LogError("No Joystick");
        return null;
    }

    public Joybutton ConnectJoybutton()
    {
        if (GetComponent<Joybutton>() && photonView.IsMine)
            return GetComponent<Joybutton>();

        Debug.LogError("No Joybutton");
        return null;
    }
}
