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

    public static int number = 0;

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
    private bool OverBoard;
    private int detectedSplats = 0;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float cooldownTime;
    #endregion

    public Transform board;

    public Joystick joystick;
    public Joybutton joybutton;

    // Start is called before the first frame update
    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine == false)
        {
            return;
        }


        PlayerManager.LocalPlayerInstance = this.gameObject;
        

        float c1 = 1f;
        float c2 = 150 / 255f;
        float c3 = Random.Range(150 / 255f, 1f);
        
        List<float> ListRandom = new List<float>() { c1, c2, c3 };
        List<float> ListRGB = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            ListRGB.Add(ListRandom[Random.Range(0, ListRandom.Count)]);
            ListRandom.Remove(ListRGB[i]);
        }
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(ListRGB[0], ListRGB[1], ListRGB[2]);
        paint.GetComponent<SpriteRenderer>().color = new Color(ListRGB[0], ListRGB[1], ListRGB[2]);
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        joystick = FindObjectOfType<Joystick>();//.GetComponent<ConnectControl>().ConnectJoystick();
        joybutton = FindObjectOfType<Joybutton>();//.GetComponent<ConnectControl>().ConnectJoybutton();

        if (board == null)
            board = GameObject.FindWithTag("Board").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        transform.Translate(transform.right * h * playerSpeed * Time.deltaTime);
        transform.Translate(transform.up * v * playerSpeed * Time.deltaTime);

        // spawn paint
        if (joybutton.Pressed && paint != null && IsFiring != true && detectedSplats == 0 && OverBoard)
        {
            IsFiring = true;
            Invoke("Cooldown", cooldownTime);
            var splat = PhotonNetwork.Instantiate(this.paint.name, transform.position, Quaternion.identity, 0);
            //splat.transform.parent = board;
            number += 1;
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
            detectedSplats++;
        } else if(collision.CompareTag("Board")) // Mighit need an IsMine
        {
            OverBoard = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Paint") && photonView.IsMine == true)
        {
            detectedSplats--;
            if (detectedSplats < 0)
                detectedSplats = 0;
        } else if(collision.CompareTag("Board"))
        {
            OverBoard = false;
        }
    }

}
