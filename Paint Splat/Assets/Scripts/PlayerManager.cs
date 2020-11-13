using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region IPunObservable implementation

    [SerializeField]
    public int score = 0;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score);
        }
        else
        {
            score = (int)stream.ReceiveNext();
        }
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
    private bool OverBoard;
    private int detectedSplats = 0;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float cooldownTime;
    #endregion

    public Transform board;

    public Joystick joystick;
    public Joybutton joybutton;

    public TextMeshPro Score;
    public TextMeshPro HitText;

    private Color playerColor;
    public Color[] colorList;

    // Start is called before the first frame update
    void Awake()
    {
        
        playerColor = colorList[photonView.CreatorActorNr];
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = playerColor;
        //paint.GetComponent<SpriteRenderer>().color = playerColor;
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine == false)
        {
            return;
        }

        //PhotonView photonView = GetComponent<PhotonView>();
        //photonView.RPC("SetScore", RpcTarget.All, PhotonNetwork.NickName);

        PlayerManager.LocalPlayerInstance = this.gameObject;
        
        /*
        float c1 = 1f;
        float c2 = 150 / 255f;
        float c3 = Random.Range(150 / 255f, 1f);
        
        List<float> ListRandom = new List<float>() { c1, c2, c3 };
        List<float> ListRGB = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            ListRGB.Add(ListRandom[Random.Range(0, ListRandom.Count)]);
            ListRandom.Remove(ListRGB[i]);
        }*/
        
        
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        joystick = FindObjectOfType<Joystick>();//.GetComponent<ConnectControl>().ConnectJoystick();
        joybutton = FindObjectOfType<Joybutton>();//.GetComponent<ConnectControl>().ConnectJoybutton();

        //userName.text = PhotonNetwork.NickName;
        

        if (board == null)
            board = GameObject.FindWithTag("Board").transform;
    }

    // Update is called once per frame
    void Update()
    {
        SetScore(score.ToString());

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
            HitText.text = "HIT!";
            Invoke("EmptyHitText", cooldownTime/2);
            object[] data = new object[5];
            data[0] = playerColor.r;
            data[1] = playerColor.g;
            data[2] = playerColor.b;
            data[3] = transform.position;
            data[4] = board.position;
            var splat = PhotonNetwork.Instantiate(this.paint.name, transform.position, Quaternion.identity, 0, data);
            //splat.transform.parent = board;
            score += 1;
            PhotonNetwork.LocalPlayer.AddScore(1);
        }
        else if (joybutton.Pressed && paint != null && IsFiring != true && (detectedSplats > 0 || !OverBoard))
        {
            HitText.text = "Miss...";
            Invoke("EmptyHitText", cooldownTime / 2);
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

    /// <summary>
    /// Turn the text back to empty
    /// </summary>
    void EmptyHitText()
    {
        HitText.text = "";
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

    void SetScore(string number)
    {
        Score.text = number;
    }

}
