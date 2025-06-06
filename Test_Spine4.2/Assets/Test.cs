using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

public class Test : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Skeleton skeleton;

    [SpineSkin]
    public string[] addSkinNames = null;
    public SpineAtlasAsset[] addSkinAssets = null;

    public int loadCount = 0;

    public Texture2D runtimeAtlas;
    public Material runtimeMaterial;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeleton = skeletonAnimation.Skeleton;

        // Output all slot names separated by commas
        var slotNames = new List<string>();
        var slotAttachmentNames = new List<string>();
        foreach (var slot in skeleton.Data.Slots)
        {
            slotNames.Add(slot.Name);
            slotAttachmentNames.Add(slot.AttachmentName);
        }
        Debug.Log("******Slots: " + string.Join(", ", slotNames));
        Debug.Log("******SlotAttachmentNames: " + string.Join(", ", slotAttachmentNames));

        // Output all skin names separated by commas
        var skinNames = new List<string>();
        foreach (var skin in skeleton.Data.Skins)
        {
            skinNames.Add(skin.Name);
        }
        Debug.Log("******Skins: " + string.Join(", ", skinNames));
    }

    // Start is called before the first frame update
    void Start()
    {

        Skin skinMix = new Skin("skin-mix");

        for (int i = 0; i < addSkinNames.Length; i++)
        {
            if (i < loadCount)
            {
                Skin addSkin = skeleton.Data.FindSkin(addSkinNames[i]);
                SetSpineAtlasAsset(addSkinAssets[i], addSkin);
                // skinMix.AddSkin(addSkin);
                skinMix.CopySkin(addSkin);
            }
        }

        for (int i = 0; i < addSkinAssets.Length; i++)
        {
            if (i < loadCount)
            {
            }
        }

        if (runtimeMaterial)
            Destroy(runtimeMaterial);
        if (runtimeAtlas)
            Destroy(runtimeAtlas);

        Material material = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        TextureFormat textureFormat = (material.mainTexture as Texture2D).format;
        Debug.Log("textureFormat: " + textureFormat);
        var skinMixTmp = skinMix.GetRepackedSkin("RepackedSkin", material, out runtimeMaterial, out runtimeAtlas, textureFormat: textureFormat);
        skinMix.Clear();

        skeletonAnimation.Skeleton.Skin = skinMixTmp;
        skeleton.SetSkin(skinMixTmp);
        // skeleton.SetSkin(skinMix);

        skeleton.UpdateCache();
        skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeleton);

        AtlasUtilities.ClearCache();
        Resources.UnloadUnusedAssets();
    }

    void SetSpineAtlasAsset(SpineAtlasAsset spineAtlasAsset, Skin skinMix)
    {
        float scale = skeletonAnimation.skeletonDataAsset.scale;
        Atlas atlas = spineAtlasAsset.GetAtlas();
        foreach (var region in atlas.Regions)
        {
            Debug.Log("111 region.name: " + region.name);
            // string slotName = region.name.Split("/")[1];
            string slotName = region.name;
            SlotData slotData = skeleton.Data.FindSlot(slotName);
            if (slotData != null)
            {
                Debug.Log("222 slotData.Name: " + slotData.Name + " slotData.Index: " + slotData.Index);
                string attachmentName = region.name;
                Attachment attachment = skinMix.GetAttachment(slotData.Index, attachmentName);
                if (attachment != null)
                {
                    Debug.Log("333 attachment: " + attachment);
                    Attachment newAttachment = attachment.GetRemappedClone(region, true, true, scale);
                    skinMix.SetAttachment(slotData.Index, attachmentName, newAttachment);
                }
            }
        }
    }
}
