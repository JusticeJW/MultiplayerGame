using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class CountDownTimerMG : MonoBehaviourPunCallbacks
    {
        public delegate void CountDownTimerHasFinished();

        public static event CountDownTimerHasFinished OnCountDownTimerHasFinished;

        public TMP_Text CountDownDisplay;

        public const string CountDownStartTime = "StartTime";

        public bool TimerIsRunning;

        public float startTime;

        public float Countdown = 5.0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!TimerIsRunning)
            {
                return;
            }

            float timer = (float)PhotonNetwork.Time - startTime;
            float countdown = Countdown - timer;

            CountDownDisplay.text = string.Format("Game starts in {0} seconds", countdown.ToString("n2"));

            if (countdown > 0.0f)
            {
                return;
            }

            TimerIsRunning = false;

            CountDownDisplay.text = string.Empty;

            if (OnCountDownTimerHasFinished != null)
            {
                OnCountDownTimerHasFinished();
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            object startTimeFromProps;

            if (propertiesThatChanged.TryGetValue(CountDownTimerMG.CountDownStartTime, out startTimeFromProps))
            {
                TimerIsRunning = true;
                startTime = (float)startTimeFromProps;
                Debug.Log("Countdown started");
            }
        }

    }
}

