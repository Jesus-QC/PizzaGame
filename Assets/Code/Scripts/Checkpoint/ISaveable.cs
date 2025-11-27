namespace Code.Scripts.Checkpoint
{
    public interface ISaveable
    {
        string id { get; }
        void Save(GameStateData data);
        void Load(GameStateData data);
    }
}