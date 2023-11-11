namespace TheRevenantsAge
{
    public interface IGameEventAction
    {
        public string OnButtonText { get; }
        public string Name { get; }
        public string Description { get; }
        public void Rise();
    }
}