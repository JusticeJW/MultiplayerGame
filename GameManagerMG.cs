using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    /* 
      host updates own props
      when client join game update own props
      onplayerpropsupdated gets called
      host checks all players loaded
      if so then host updates room props
      onroompropsupdated countdown timer starts
      countdown timer starts game

        base object for each player with - main player script, photon view, photon rigidbody view
     
     */
    public class Team
    {
        public int score;
        public string name;
        public int pteamsync;

        public Team(string teamname, int pteamnum)
        {
            name = teamname;
            pteamsync = pteamnum;
        }
    }

    public class GameManagerMG : MonoBehaviourPunCallbacks
    {
        public static GameManagerMG Instance = null;

        List<Team> teams = new List<Team>();
        public List<GameObject> Characters = new List<GameObject>();

        Hashtable PrevRoundLives;

        Team team1 = new Team("Team 1", 1);
        Team team2 = new Team("Team 2", 2);

        public GameObject MachineCreature;
        public GameObject HolyKnight;

        public TMP_Text TeamDisplay;
        public TMP_Text GameOverDisplay;
        public TMP_Text ScoreDisplay;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountDownTimerMG.OnCountDownTimerHasFinished += OnCountDownTimerFinish;
        }

        // Start is called before the first frame update
        void Start()
        {
            Hashtable props = new Hashtable
            {
                {MPGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            Hashtable props2 = new Hashtable
            {
                {PunPlayerProps.PlayerCharacterLivesProp, MPGame.PLAYER_MAX_LIVES},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props2);

            teams.Add(team1);
            teams.Add(team2);
            Debug.Log("You have loaded in");

            ScoreDisplay.text = team1.name + ": " + team1.score.ToString() + "\n" + team2.name + ": " + team2.score.ToString();

            if (PhotonNetwork.IsMasterClient)
            {
                PrevRoundLives = new Hashtable();
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    object playerLives;
                    if (p.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterLivesProp, out playerLives))
                    {
                        PrevRoundLives.Add(p, (int)playerLives);
                    }
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountDownTimerMG.OnCountDownTimerHasFinished -= OnCountDownTimerFinish;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void StartGame()
        {


            if (PhotonNetwork.IsMasterClient)
            {

            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PunPlayerProps.PlayerTeamProp))
            {
                TeamDisplay.text = "Team" + PhotonNetwork.LocalPlayer.GetTeam().ToString();
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (changedProps.ContainsKey(MPGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    Debug.Log("All players loaded in");
                    Hashtable props = new Hashtable
                    {
                        {CountDownTimerMG.CountDownStartTime, (float) PhotonNetwork.Time},
                    };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                    SetupTeams();
                    TeamDisplay.text = team1.name + ": " + team1.score.ToString() + "\n" + team2.name + ": " + team2.score.ToString();
                }
            }

            if (changedProps.ContainsKey(PunPlayerProps.PlayerCharacterLivesProp))
            {
                int CTD = CheckTeamDead();

                if (CTD == 0)
                    return;

                if (CTD == 2)
                    Team2WinsRound();

                if (CTD == 1)
                    Team1WinsRound();
            }
        }
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if(propertiesThatChanged.ContainsKey(MPGame.TEAM1_SCORE) || propertiesThatChanged.ContainsKey(MPGame.TEAM2_SCORE))
            {
                ScoreDisplay.text = team1.name + ": " + team1.score.ToString() + "\n" + team2.name + ": " + team2.score.ToString();

                int iii = 0;
                int jjj = 0;
                foreach (GameObject Char in Characters)
                {
                    if (iii % 2 == 0)
                    {
                        jjj = 1;
                    }
                    else
                    {
                        jjj = -1;
                    }
                    Char.transform.position = new Vector3(iii + 9, 0, jjj * 30);
                    object health;
                    if(Char.GetComponent<PhotonView>().Owner.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterHPProp, out health))
                    {
                        health = Char.GetComponent<PhotonView>().Owner.GetCharacter().HP;
                    }

                    Hashtable props = new Hashtable
                    {
                        {PunPlayerProps.PlayerCharacterHPProp, health}
                    };
                    Char.GetComponent<PhotonView>().Owner.SetCustomProperties(props);
                    Char.GetComponent<Character>().Health = Char.GetComponent<PhotonView>().Owner.GetCharacter().HP;

                    iii++;
                }

            }

            if (propertiesThatChanged.ContainsKey(MPGame.GAME_ENDING))
            {
                object teamwin;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PunPlayerProps.PlayerTeamProp, out teamwin))
                {
                    GameOverDisplay.text = "Team " + teamwin.ToString() + " Wins";
                    GameOverDisplay.gameObject.SetActive(true);
                }

                PhotonNetwork.LeaveLobby();
            }
        }

        /*
        public void OnRoundEnd()
        {
            CheckEndOfGame();
        }

        private void CheckEndOfGame()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (team1.score == 9)
                {
                    Team1Wins();
                    break;
                }
                if (team2.score == 9)
                {
                    Team2Wins();
                    break;
                }
            }
            //StartCoroutine(EndOfGame(winner, score));
        } */
    

        public void SetupTeams()
        {
            int iii = 0;
            int jjj = 0;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (iii % 2 == 0)
                {
                    p.SetTeam(1);
                    jjj = 1;
                }
                else
                {
                    p.SetTeam(2);
                    jjj = -1;
                }

                GameObject TempChar = PhotonNetwork.Instantiate(p.GetCharacter().CharacterName, new Vector3(iii + 9, 0, jjj * 30), Quaternion.Euler(0, 0, 0), 0);
                Debug.Log("Player " + (iii + 1) + " Spawned");
                TempChar.GetComponent<PhotonView>().TransferOwnership(p);
                TempChar.GetComponent<PhotonView>().RPC("UpdateCharProps", RpcTarget.All, p);
                Characters.Add(TempChar);

                iii++;
            }
            iii = 0;
        }


        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(MPGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private int CheckTeamDead()
        {
            int team1dead = 0;
            int team2dead = 0;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object team;
                if (p.CustomProperties.TryGetValue(PunPlayerProps.PlayerTeamProp, out team))
                {
                    if ((int)team == 1)
                    {
                        object playerLives;
                        if (p.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterLivesProp, out playerLives))
                        {
                            if (PrevRoundLives[p] != playerLives)
                            {
                                team1dead++;
                            }
                        }

                    }
                    if ((int)team == 2)
                    {
                        object playerLives;
                        if (p.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterLivesProp, out playerLives))
                        {
                            if (PrevRoundLives[p] != playerLives)
                            {
                                team2dead++;
                            }
                        }
                    }
                }
            }

            if (team1dead == (PhotonNetwork.PlayerList.Length / 2))
                return 1;
            else if (team2dead == (PhotonNetwork.PlayerList.Length / 2))
                return 2;
            else
                return 0;
        }

        public void OnCountDownTimerFinish()
        {
            StartGame();
        }

        public void Team1WinsRound()
        {
            team1.score++;
            Hashtable props = new Hashtable
            {
                {MPGame.TEAM1_SCORE, team1.score}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            if (team1.score == MPGame.WIN_SCORE)
            {
                Hashtable props2 = new Hashtable
                {
                    {MPGame.GAME_ENDING, 1}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props2);
            }
        }

        public void Team2WinsRound()
        {
            team2.score++;
            Hashtable props = new Hashtable
            {
                {MPGame.TEAM2_SCORE, team2.score}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            if (team2.score == MPGame.WIN_SCORE)
            {
                Hashtable props2 = new Hashtable
                {
                    {MPGame.GAME_ENDING, 2}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props2);
            }
        }
    }
}

