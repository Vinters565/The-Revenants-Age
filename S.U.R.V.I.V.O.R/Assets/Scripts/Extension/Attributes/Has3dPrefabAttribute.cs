using System;
using System.Collections.Generic;

namespace Extension
{
    public class Has3dPrefabAttribute: Attribute
    {
        public IReadOnlyCollection<Type> requireComponentsIn2DPrefab;
        public IReadOnlyCollection<Type> requireComponentsIn3DPrefab;
        
        public Has3dPrefabAttribute() : this(
            Type.EmptyTypes,
            Type.EmptyTypes)
        {}
        public Has3dPrefabAttribute(Type[] requireComponentsIn2DPrefab) : this(
            requireComponentsIn2DPrefab,
            Type.EmptyTypes)
        {}
        
        public Has3dPrefabAttribute(
            Type[] requireComponentsIn2DPrefab,
            Type[] requireComponentsIn3DPrefab)
        {
            this.requireComponentsIn2DPrefab = requireComponentsIn2DPrefab;
            this.requireComponentsIn3DPrefab = requireComponentsIn3DPrefab;
        }
    }
}