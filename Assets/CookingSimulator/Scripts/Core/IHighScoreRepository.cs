namespace CookingSimulator.Core
{

    public interface IHighScoreRepository
    {
        int Load();
        void Save(int value);
    }
}