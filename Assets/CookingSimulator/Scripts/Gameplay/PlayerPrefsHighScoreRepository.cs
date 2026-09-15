using UnityEngine;
using CookingSimulator.Core;

namespace CookingSimulator.Scripts.Gameplay
{
    public sealed class PlayerPrefsHighScoreRepository : IHighScoreRepository
    {
        private const string Key = "CookingSimulator.HighScore";
        public int Load()=> PlayerPrefs.GetInt(Key, 0);

        public void Save(int value)
        {
            PlayerPrefs.SetInt(Key, value);
            PlayerPrefs.Save();
        }
    }
}