using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdditionalScripts
{
    /// <summary>
    /// This class represents the tool controller in the game.
    /// </summary>
    public class ToolController : MonoBehaviour
    {
        /// <summary>
        /// The number of collected coins.
        /// </summary>
        public static int CollectedCoin;

        /// <summary>
        /// The current score.
        /// </summary>
        public static int Score;

        /// <summary>
        /// The number of lives remaining.
        /// </summary>
        public static int Live;

        /// <summary>
        /// The current level.
        /// </summary>
        public static int CurrentLevel;

        /// <summary>
        /// The high score achieved.
        /// </summary>
        public static int _highScore;

        /// <summary>
        /// Indicates if the player is dead.
        /// </summary>
        public static bool IsDead;

        /// <summary>
        /// Indicates if the game is over.
        /// </summary>
        public static bool IsGameOver;

        /// <summary>
        /// Indicates if the stage is clear.
        /// </summary>
        public static bool IsStageClear;

        /// <summary>
        /// Indicates if the player is big.
        /// </summary>
        public static bool IsBigPlayer;

        /// <summary>
        /// Indicates if the player is on fire.
        /// </summary>
        public static bool IsFirePlayer;

        /// <summary>
        /// Indicates if it is a boss battle.
        /// </summary>
        public static bool IsBossBattle;

        /// <summary>
        /// Indicates if the game has finished.
        /// </summary>
        public static bool IsGameFinish;

        /// <summary>
        /// Indicates if an enemy has died or a coin has been eaten.
        /// </summary>
        public static bool IsEnemyDieOrCoinEat;

        /// <summary>
        /// Indicates if a power-up has been eaten.
        /// </summary>
        public static bool IsPowerUpEat;

        /// <summary>
        /// Indicates if a message should be shown.
        /// </summary>
        public static bool IsShowMessage;

        /// <summary>
        /// The tag of the player.
        /// </summary>
        public static string PlayerTag;
    }
}