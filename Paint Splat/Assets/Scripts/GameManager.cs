using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public Text timeText;
        [SerializeField]
        private float time = 100;
        private bool GameDone = false;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(time);
            }
            else
            {
                time = (float)stream.ReceiveNext();
            }
        }

        public void Update()
        {
            if (GameDone)
                return;

            if (time == 0)
            {
                var winner = "";
                var highscore = -1;
                for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount+1; i++)
                {
                    var score = PhotonNetwork.CurrentRoom.Players[i].GetScore();
                    print(PhotonNetwork.CurrentRoom.Players[i].NickName + " has score: " + PhotonNetwork.CurrentRoom.Players[i].GetScore());
                    if (score > highscore)
                    {
                        highscore = score;
                        print(PhotonNetwork.CurrentRoom.Players[i].NickName + " score was greater than highscore " + highscore);
                        winner = PhotonNetwork.CurrentRoom.Players[i].NickName;
                    }
                }

                timeText.text = winner + " Won!";
                GameDone = true;
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.CurrentRoom.IsOpen = false;

                Invoke("LeaveRoom", 5);
            }
            else
                timeText.text = ((int)time).ToString();


            if (!PhotonNetwork.IsMasterClient)
                return;

            if (time > 0)
                time -= Time.deltaTime;
            else
                time = 0;
            
        }

        #region Photon Callbacks

        private void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        #endregion

        #region Private Methods


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("GameRoom");
        }


        #endregion

        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
            print(other.ActorNumber);
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion

        #region Public Methods

        
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

        

    }
}