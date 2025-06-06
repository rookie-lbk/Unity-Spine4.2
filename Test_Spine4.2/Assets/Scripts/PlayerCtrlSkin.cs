using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

public partial class PlayerCtrl
{

    [SpineSkin]
    public string[] skinNames = null;
    public SpineAtlasAsset[] skinAssets = null;

    public bool isRepackSkin = false;
    public Texture2D runtimeAtlas;
    public Material runtimeMaterial;


    [Button("换装")]
    private void ChangeSkin()
    {
        Skin skinMix = new Skin("skin-mix");
        if (skinNames.Length == 0)
        {
            return;
        }
        for (int i = 0; i < skinNames.Length; i++)
        {
            Skin addSkin = skeleton.Data.FindSkin(skinNames[i]);
            SetSpineAtlasAsset(skinAssets[i], addSkin);
            // skinMix.AddSkin(addSkin);
            skinMix.CopySkin(addSkin);
        }

        if (runtimeMaterial)
            Destroy(runtimeMaterial);
        if (runtimeAtlas)
            Destroy(runtimeAtlas);

        if (isRepackSkin)
        {
            Material material = skeletonMecanim.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            TextureFormat textureFormat = (material.mainTexture as Texture2D).format;
            Debug.Log("textureFormat: " + textureFormat);
            var skinMixTmp = skinMix.GetRepackedSkin("RepackedSkin", material, out runtimeMaterial, out runtimeAtlas, textureFormat: textureFormat);
            skinMix.Clear();

            skeletonMecanim.Skeleton.Skin = skinMixTmp;
            skeleton.SetSkin(skinMixTmp);
        }
        else
        {
            skeleton.SetSkin(skinMix);
        }

        skeleton.UpdateCache();
        skeleton.SetSlotsToSetupPose();

        AtlasUtilities.ClearCache();
        Resources.UnloadUnusedAssets();
    }

    void SetSpineAtlasAsset(SpineAtlasAsset spineAtlasAsset, Skin skinMix)
    {
        float scale = skeletonMecanim.skeletonDataAsset.scale;
        Atlas atlas = spineAtlasAsset.GetAtlas();
        foreach (var region in atlas.Regions)
        {
            // Debug.Log("111 region.name: " + region.name);
            SlotData slotData = skeleton.Data.FindSlot(region.name);
            if (slotData != null)
            {
                // Debug.Log("222 slotData.Name: " + slotData.Name + " slotData.Index: " + slotData.Index);
                string attachmentName = region.name;
                Attachment attachment = skinMix.GetAttachment(slotData.Index, attachmentName);
                if (attachment != null)
                {
                    // Debug.Log("333 attachment: " + attachment);
                    Attachment newAttachment = attachment.GetRemappedClone(region, true, true, scale);
                    skinMix.SetAttachment(slotData.Index, attachmentName, newAttachment);
                }
            }
        }
    }

    public void RecycleSkinAssets()
    {
        if (runtimeMaterial)
            Destroy(runtimeMaterial);
        if (runtimeAtlas)
            Destroy(runtimeAtlas);
    }
}
