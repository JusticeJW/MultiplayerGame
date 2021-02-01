using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class LobbyThing : MonoBehaviourPunCallbacks
    {
        public PlayableCharacterSetup CharScript;

        public GameObject LogInButton;
        public GameObject CreateRoomButton;
        public GameObject JoinRoomButton;
        public GameObject LobbyPlayerList;
        public GameObject CharacterSelectList;
        public GameObject StartGameButton;
        public GameObject CharacterSelectedDisplay;
        public GameObject UserImage;
        public GameObject CharacterSelectImage;
        public TMP_InputField UsernameInput;
        public TMP_InputField RoomNameInput;
        public TMP_InputField MaxPlayersInput;
        public TMP_InputField RoomNumber;
        public TMP_Text UsernameDisplay;
        public TMP_Text RoomNumberDisplay;
        public List<GameObject> UserImages = new List<GameObject>();
        // Start is called before the first frame update

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SwitchToLogInMenu()
        {
            LogInButton.SetActive(true);
            UsernameInput.gameObject.SetActive(true);
            CreateRoomButton.SetActive(false);
            JoinRoomButton.SetActive(false);
            RoomNameInput.gameObject.SetActive(false);
            MaxPlayersInput.gameObject.SetActive(false);
            RoomNumber.gameObject.SetActive(false);
            RoomNumberDisplay.gameObject.SetActive(false);
            LobbyPlayerList.gameObject.SetActive(false);
            StartGameButton.SetActive(false);
            CharacterSelectList.SetActive(false);
            CharacterSelectedDisplay.SetActive(false);
        }
        public void SwitchToMainMenu()
        {
            LogInButton.SetActive(false);
            UsernameInput.gameObject.SetActive(false);
            CreateRoomButton.SetActive(true);
            JoinRoomButton.SetActive(true);
            RoomNameInput.gameObject.SetActive(true);
            MaxPlayersInput.gameObject.SetActive(true);
            RoomNumber.gameObject.SetActive(true);
            RoomNumberDisplay.gameObject.SetActive(false);
            LobbyPlayerList.gameObject.SetActive(false);
            StartGameButton.SetActive(false);
            CharacterSelectList.SetActive(false);
            CharacterSelectedDisplay.SetActive(false);
        }

        public void SwitchToLobbyMenu()
        {
            LogInButton.SetActive(false);
            UsernameInput.gameObject.SetActive(false);
            CreateRoomButton.SetActive(false);
            JoinRoomButton.SetActive(false);
            RoomNameInput.gameObject.SetActive(false);
            MaxPlayersInput.gameObject.SetActive(false);
            RoomNumber.gameObject.SetActive(false);
            RoomNumberDisplay.gameObject.SetActive(true);
            LobbyPlayerList.gameObject.SetActive(true);
            CharacterSelectList.SetActive(false);
            CharacterSelectedDisplay.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
            {
                StartGameButton.SetActive(true);
            }
        }

        public void SwitchToCharacterSelect()
        {
            LogInButton.SetActive(false);
            UsernameInput.gameObject.SetActive(false);
            CreateRoomButton.SetActive(false);
            JoinRoomButton.SetActive(false);
            RoomNameInput.gameObject.SetActive(false);
            MaxPlayersInput.gameObject.SetActive(false);
            RoomNumber.gameObject.SetActive(false);
            RoomNumberDisplay.gameObject.SetActive(false);
            LobbyPlayerList.gameObject.SetActive(false);
            StartGameButton.SetActive(false);
            foreach (PlayableCharacter c in CharScript.PlayableCharacters)
            {
                GameObject TempCharacterSelectImage = Instantiate(CharacterSelectImage);
                TempCharacterSelectImage.transform.parent = CharacterSelectList.transform;
                TempCharacterSelectImage.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = c.CharacterName;
                TempCharacterSelectImage.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(()=> OnSelectCharacterButtonClicked(c.CharacterName));
                TempCharacterSelectImage.SetActive(true);
            }
            CharacterSelectList.SetActive(true);
            CharacterSelectedDisplay.SetActive(true);
        }

        public void OnLoginButtonClicked()
        {
            string localplayerName = UsernameInput.text;

            if (!localplayerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = localplayerName;
                Debug.Log(localplayerName);
                PhotonNetwork.ConnectUsingSettings();
                Debug.Log(PhotonNetwork.NetworkClientState);
                UsernameDisplay.text = "Username: " + UsernameInput.text;
                SwitchToMainMenu();
            }
            else
            {
                Debug.LogError("No player name set. Please enter player name.");
            }
        }

        public void OnCreateRoomButtonClicked()
        {
            string roomName = RoomNameInput.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInput.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

            PhotonNetwork.CreateRoom(roomName, options, null);
            Debug.Log(roomName);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room created Successfully");
        }

        public void OnJoinRoomButtonClicked()
        {
            PhotonNetwork.JoinRoom(RoomNumber.text);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined to room successfully");
            RoomNumberDisplay.text = PhotonNetwork.CurrentRoom.ToString();
            SwitchToLobbyMenu();
            //PhotonNetwork.CurrentRoom.Players
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject TempUserImage = Instantiate(UserImage);
                TempUserImage.transform.parent = LobbyPlayerList.transform;
                TempUserImage.GetComponentInChildren<TMP_Text>().text = p.NickName;
                UserImages.Add(TempUserImage);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            RoomNumberDisplay.text = PhotonNetwork.CurrentRoom.ToString();
            GameObject TempUserImage = Instantiate(UserImage);
            TempUserImage.transform.parent = LobbyPlayerList.transform;
            TempUserImage.GetComponentInChildren<TMP_Text>().text = newPlayer.NickName;
            UserImages.Add(TempUserImage);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            RoomNumberDisplay.text = PhotonNetwork.CurrentRoom.ToString();
            Destroy(UserImages[UserImages.Count - 1]);
            UserImages.RemoveAt(UserImages.Count - 1);
        }

        public void OnStartGameButtonClicked()
        {
            SwitchToCharacterSelect();
            Hashtable props = new Hashtable
            {
                {MPGame.CHAR_SELECT_TIME, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public void OnSelectCharacterButtonClicked(string c)
        {
           CharacterSelectedDisplay.GetComponentInChildren<TMP_Text>().text = c;
           PhotonNetwork.LocalPlayer.SetCharacter(c);
        }

        public override void OnJoinRoomFailed(short returncode, string message)
        {
            Debug.Log("Failed to join room: " + message);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (changedProps.ContainsKey(MPGame.CHAR_SELECT_TIME))
                    SwitchToCharacterSelect();
                if (changedProps.ContainsKey(MPGame.HOST_READY))
                    SceneManager.LoadScene("Game");
                return;
            }

            if (changedProps.ContainsKey(PunPlayerProps.PlayerCharacterProp))
            {
                if (CheckAllPlayerSelectedCharacter())
                {
                    Debug.Log("All players selected character");
                    Hashtable props = new Hashtable
                    {
                        {MPGame.HOST_READY, true}
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    SceneManager.LoadScene("Game");
                }
            }
        }

        private bool CheckAllPlayerSelectedCharacter()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerselectedcharacter;

                if (!p.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterProp, out playerselectedcharacter))
                {
                    return false;
                }    
            }

            return true;
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Successfully connected to master");
        }
    }
}

