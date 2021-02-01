using UnityEngine;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class MPGame
    {
        public const string HOST_READY = "IsHostReady";
        public const string CHAR_SELECT_TIME = "TimeForCharacterSelect";

        public const int WIN_SCORE = 9;

        public const float PLAYER_RESPAWN_TIME = 0.0f;

        public const int PLAYABLE_CHARACTER_COUNT = 1;
        public const int PLAYER_MAX_LIVES = 1;

        public const string PLAYER_LIVES = "PlayerLives";
        public const string PLAYER_READY = "IsPlayerReady";
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
        public const string TEAM1_SCORE = "Team 1 Score";
        public const string TEAM2_SCORE = "Team 2 Score";
        public const string GAME_ENDING = "Initiating End of Game";

        public static Color GetColor(int colorChoice)
        {
            switch (colorChoice)
            {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.grey;
                case 6: return Color.magenta;
                case 7: return Color.white;
            }

            return Color.black;
        }
    }
}
