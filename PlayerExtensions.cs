using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    [System.Serializable]
    public struct PlayableCharacter
    {
        public string CharacterName;
        public int HP;
        public PlayableCharacter(string Char)
        {
            CharacterName = Char;
            switch (Char)
            {
                case "Machine Creature":
                    HP = 1500;
                    break;
                case "Holy Knight":
                    HP = 1500;
                    break;
                default:
                    HP = 0;
                    break;
            }
        }
    }
    public class PunPlayerProps : MonoBehaviour
    {
        public const string PlayerTeamProp = "team";
        public const string PlayerCharacterProp = "character";
        public const string PlayerCharacterHPProp = "HP";
        public const string PlayerCharacterLivesProp = "Lives";
    }

    public static class PlayerExtensions
    {
        public static void SetTeam(this Player player, int Team)
        {
            Hashtable team = new Hashtable();
            team[PunPlayerProps.PlayerTeamProp] = Team;

            player.SetCustomProperties(team);
        }

        public static int GetTeam(this Player player)
        {
            object team;
            if (player.CustomProperties.TryGetValue(PunPlayerProps.PlayerTeamProp, out team))
            {
                return (int)team;
            }
            return 0;
        }

        public static void SetCharacter(this Player player, string Character)
        {
            //Hashtable character = new Hashtable();
            //character[PunPlayerProps.PlayerCharacterProp] = Character;
            PlayableCharacter c = new PlayableCharacter(Character);

            Hashtable character = new Hashtable
            {
                {PunPlayerProps.PlayerCharacterProp, Character},
                {PunPlayerProps.PlayerCharacterHPProp, c.HP}
            };

            player.SetCustomProperties(character);
        }

        public static PlayableCharacter GetCharacter(this Player player)
        {
            object character;
            if (player.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterProp, out character))
            {
                return new PlayableCharacter((string)character);
            }
            return new PlayableCharacter((string)character);
        }


    }
}