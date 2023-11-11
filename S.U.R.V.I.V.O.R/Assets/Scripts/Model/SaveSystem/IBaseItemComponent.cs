using TheRevenantsAge;

namespace TheRevenantsAge
{
    public interface IBaseItemComponent
    {
        public ComponentState CreateState();
        public void Restore(ComponentState state);
    }
}