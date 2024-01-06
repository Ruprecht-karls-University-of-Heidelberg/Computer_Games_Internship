using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdditionalScripts
{
    public class ToolController : MonoBehaviour
    {
        public static int CollectedCoin;
        public static int Score;
        public static int Live;
        public static int CurrentLevel;
        public static int _highScore;
        public static bool IsDead;
        public static bool IsGameOver;
        public static bool IsStageClear;
        public static bool IsBigPlayer;
        public static bool IsFirePlayer;
        public static bool IsBossBattle;
        public static bool IsGameFinish;
        public static bool IsEnemyDieOrCoinEat;
        public static bool IsPowerUpEat;
        public static bool IsShowMessage;
        public static string PlayerTag;

    }
}