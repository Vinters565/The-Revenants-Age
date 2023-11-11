using System;
using System.Collections.Generic;
using System.Linq;
using TheRevenantsAge;
using UnityEngine;

namespace TheRevenantsAge
{
    public class CharacterClothesController: MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer head;
        [SerializeField] private SkinnedMeshRenderer chest;
        [SerializeField] private SkinnedMeshRenderer legs;
        [SerializeField] private SkinnedMeshRenderer feet;

        private Transform[] bones;
        private Transform rootBone;

        private SkinnedMeshRenderer[] renderers;
        private HashSet<SkinnedMeshRenderer> renderersSet;

        private Dictionary<HidePaths, SkinnedMeshRenderer> compliance;

        private void Awake()
        {
            renderers = new SkinnedMeshRenderer[]
            {
                head,
                chest,
                legs,
                feet
            };
            renderersSet = renderers.ToHashSet();
            
            compliance = new Dictionary<HidePaths, SkinnedMeshRenderer>()
            {
                {HidePaths.Head, head},
                {HidePaths.Chest, chest},
                {HidePaths.Legs, legs},
                {HidePaths.Feet, feet}
            };
            if (!Game.Is3D)
                HideAll();
            bones = head.bones;
            rootBone = head.rootBone;
        }

        private void HideAll()
        {
            foreach (var skinnedMeshRenderer in renderers)
                skinnedMeshRenderer.gameObject.SetActive(false);
        }
        
        public void Wear3DClothes(Clothes clothes)
        {
            var cloth3DExtension = clothes.GetComponent<Cloth3DExtension>();
            if (cloth3DExtension != null &&
                cloth3DExtension.HiddenPath != HidePaths.None)
            {
                var skinMesh = compliance[cloth3DExtension.HiddenPath];
                if (cloth3DExtension.HideAsset != null)
                {
                    var hideAsset = cloth3DExtension.HideAsset;
                    skinMesh.sharedMesh = hideAsset.GetNotHideMesh();
                }
                else
                {
                    skinMesh.gameObject.SetActive(false);
                }
            }
            
            var render = clothes.GetComponentInChildren<SkinnedMeshRenderer>();
            if (render == null)
            {
                Debug.LogWarning("У одежды нет скелета персонажа!");
                Destroy(clothes.gameObject);
                return;
            }
            render.bones = bones;
            render.rootBone = rootBone;
            
        }
    }
}