
using System;
using CookingSimulator.Core;

namespace CookingSimulator.Scripts.Core
{
    public sealed class ScoreService
    {
        private readonly IHighScoreRepository repository;
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public event Action<int> ScoreChanged; 
        public event Action<int> HighScoreChanged; 
        public ScoreService(IHighScoreRepository repository)
        {
            this.repository = repository;
            HighScore = repository.Load();
        }

        public void ResetRun()
        {
            Score = 0;
            ScoreChanged?.Invoke(Score);
        }

        public void Add(int delta)
        {
            Score += delta;
            ScoreChanged?.Invoke(Score);
        }

        public bool CommitRun()
        {
            if (Score <= HighScore) return false;
            HighScore = Score;
            repository.Save(HighScore);
            HighScoreChanged?.Invoke(HighScore);
            return true;
        }
        
    }
    
}