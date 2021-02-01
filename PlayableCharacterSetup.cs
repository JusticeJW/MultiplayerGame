using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class PlayableCharacterSetup : MonoBehaviourPunCallbacks
    {
        public List<PlayableCharacter> PlayableCharacters = new List<PlayableCharacter>();

        private void Awake()
        {
            PlayableCharacters.Add(new PlayableCharacter("Machine Creature")); //0
            PlayableCharacters.Add(new PlayableCharacter("Holy Knight")); //1
        }
    }
}
