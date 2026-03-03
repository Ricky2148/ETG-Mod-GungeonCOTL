using Alexandria.VisualAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace GungeonCOTL.custom_class_data
{
    public static class VFXPlayerCOTL
    {
        private static List<string> DoctrineVFXSpritePath = new List<string>
        {
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_001",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_002",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_003",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_004",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_005",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_006",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_007",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_008",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_009",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_010",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_011",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_012",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_013",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_014",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_015",
            "GungeonCOTL/Resources/vfxs/doctrine_vfx/commandment_aura_016",
            "GungeonCOTL/Resources/one_off_sprites/blank_sprite",
        };

        private static GameObject DoctrineEffectVFX = VFXBuilder.CreateVFX
        (
            "doctrine_vfx",
            DoctrineVFXSpritePath,
            11,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Once,
            true
        );

        public static GameObject PlayDoctrineEffectOnActor(PlayerController player, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            Vector3 offset = new Vector3(20 / 16f, 44 / 16f, 0f);

            GameObject vfxObject = player.PlayEffectOnActor(DoctrineEffectVFX, offset, attached, alreadyMiddleCenter, useHitbox);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = 1f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                /*sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 1f);*/
            }

            return vfxObject;
        }

        private static List<string> RedCrownVFXSpritePath = new List<string>
        {
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_001",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_002",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_003",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_004",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_005",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_006",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_007",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_008",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_009",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_010",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_011",
            "GungeonCOTL/Resources/vfxs/red_crown_activation_vfx/blackfire_aura_012",
        };

        private static GameObject RedCrownEffectVFX = VFXBuilder.CreateVFX
        (
            "red_crown_activation_vfx",
            RedCrownVFXSpritePath,
            8,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Loop,
            true
        );

        public static GameObject PlayRedCrownEffectOnActor(PlayerController player, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            Vector3 offset = new Vector3(11 / 16f, 36 / 16f, 0f);

            GameObject vfxObject = player.PlayEffectOnActor(RedCrownEffectVFX, offset, attached, alreadyMiddleCenter, useHitbox);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -3f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 0.7f);
            }

            return vfxObject;
        }

        private static List<string> BlackfireVFXSpritePath = "GungeonCOTL/Resources/vfxs/blackfire_fadeaway_vfx/blackfire_aura".GetResourceFrames(41);

        private static GameObject BlackfireEffectVFX = VFXBuilder.CreateVFX
        (
            "blackfire_fadeaway_vfx",
            BlackfireVFXSpritePath,
            8,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Once,
            true
        );

        public static GameObject PlayBlackfireEffectOnActor(PlayerController player, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            Vector3 offset = new Vector3(11 / 16f, 36 / 16f, 0f);

            GameObject vfxObject = player.PlayEffectOnActor(BlackfireEffectVFX, offset, attached, alreadyMiddleCenter, useHitbox);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = -3f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 0.85f);
            }

            return vfxObject;
        }

        private static List<string> RitualActivationVFXSpritePath = "GungeonCOTL/Resources/vfxs/ritual_activation_vfx/pentacle_aura".GetResourceFrames(71);

        private static GameObject RitualActivationEffectVFX = VFXBuilder.CreateVFX
        (
            "ritual_activation_vfx",
            RitualActivationVFXSpritePath,
            15,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Once,
            true
        );

        public static GameObject PlayRitualActivationEffectOnActor(PlayerController player, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            Vector3 offset = new Vector3(18 / 16f, 28 / 16f, 0f);

            GameObject vfxObject = player.PlayEffectOnActor(RitualActivationEffectVFX, offset, attached, alreadyMiddleCenter, useHitbox);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = 2f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                /*sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 1f);*/
            }

            return vfxObject;
        }

        private static List<string> CrownUpgradeVFXSpritePath = "GungeonCOTL/Resources/vfxs/crown_upgrade_vfx/upgrade_aura".GetResourceFrames(45);

        private static GameObject CrownUpgradeEffectVFX = VFXBuilder.CreateVFX
        (
            "crown_upgrade_vfx",
            CrownUpgradeVFXSpritePath,
            15,
            new IntVector2(0, 0),
            tk2dBaseSprite.Anchor.MiddleCenter,
            false,
            0,
            -1,
            Color.cyan,
            tk2dSpriteAnimationClip.WrapMode.Once,
            true
        );

        public static GameObject PlayCrownUpgradeEffectOnActor(PlayerController player, bool attached = true, bool alreadyMiddleCenter = false, bool useHitbox = false)
        {
            Vector3 offset = new Vector3(64 / 16f, 60 / 16f, 0f);

            GameObject vfxObject = player.PlayEffectOnActor(CrownUpgradeEffectVFX, offset, attached, alreadyMiddleCenter, useHitbox);

            var sprite = vfxObject.GetComponent<tk2dSprite>();

            if (sprite != null)
            {
                sprite.HeightOffGround = 2f;

                //sprite.scale = new Vector3(2.5f, 2.5f, 0f);

                sprite.UpdateZDepth();

                /*sprite.usesOverrideMaterial = true;

                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/SimpleAlphaFadeUnlit");
                sprite.renderer.material.SetFloat("_Fade", 1f);*/
            }

            return vfxObject;
        }

        public static System.Collections.IEnumerator HardCodedCrownUpgradeEffectSFXPlayer(PlayerController player)
        {
            List<string> SFXList = new List<string>
            {
                "pop_1",
                "pop_2",
                "pop_3",
                "pop_4",
                "pop_5",
                "pop_6",
                "pop_7",
            };

            yield return new WaitForSeconds(10 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(1 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(7 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(4 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(3 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(4 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(3 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);

            yield return new WaitForSeconds(8 * (1 / 15f));

            HelpfulMethods.PlayRandomSFX(player.gameObject, SFXList);
        }
    }
}
