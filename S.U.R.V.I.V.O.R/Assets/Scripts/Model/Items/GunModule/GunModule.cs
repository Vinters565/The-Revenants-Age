using TheRevenantsAge;
using UnityEngine;

[RequireComponent(typeof(BaseItem))]
public class GunModule : MonoBehaviour, IBaseItemComponent
{
    [SerializeField] private GunModuleData data;
    public GunModuleData Data => data;

    public ComponentState CreateState()
    {
        return new GunModuleState();
    }

    public void Restore(ComponentState state) {}
}