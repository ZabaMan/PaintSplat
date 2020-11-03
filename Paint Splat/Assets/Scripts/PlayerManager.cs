using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }


    #endregion


    #region Private Fields

    [SerializeField]
    private float playerSpeed = 5f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("The Paint GameObject to shoot")]
    [SerializeField]
    private GameObject paint;
    //True, when the user is firing
    private bool IsFiring;
    private bool canFire = true;
    [SerializeField]
    private float cooldownTime;
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(transform.right * h * playerSpeed * Time.deltaTime);
        transform.Translate(transform.up * v * playerSpeed * Time.deltaTime);
        

        // spawn paint
        if (Input.GetButtonDown("Fire1") && paint != null && IsFiring != true && canFire)
        {
            IsFiring = true;
            Invoke("Cooldown", cooldownTime);
            var splat = PhotonNetwork.Instantiate(this.paint.name, transform.position, Quaternion.identity, 0);
        }

    }

    #region Custom

    /// <summary>
    /// Disable IsFiring
    /// </summary>
    void Cooldown ()
    {
        IsFiring = false;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Paint") && photonView.IsMine == true)
        {
            canFire = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Paint") && photonView.IsMine == true)
        {
            canFire = true;
        }
    }

}
