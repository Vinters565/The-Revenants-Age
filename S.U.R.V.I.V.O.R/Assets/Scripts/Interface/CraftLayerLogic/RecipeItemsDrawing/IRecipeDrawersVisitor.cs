using TheRevenantsAge;

namespace Interface.CraftLayerLogic
{
    public interface IRecipeDrawersVisitor<out T>
    {   
        T Visit(CraftRequiredList list);
        
        T Visit(CraftRequiredItem item);
    }
}   