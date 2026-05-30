using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
    internal static class PatchPlayerRemovePiece
    {
        private static bool Prefix(Player __instance, ref bool __result)
        {
            if (__instance == null || !FeatureFlags.CanUseExtendedPlacement())
            {
                return true;
            }

            if (GameCamera.instance == null || GameCamera.instance.transform == null)
            {
                return true;
            }

            Vector3 origin = GameCamera.instance.transform.position;
            Vector3 direction = GameCamera.instance.transform.forward;

            int mask = Traverse.Create(__instance)
                .Field("m_removeRayMask")
                .GetValue<int>();

            RaycastHit hit;

            if (!Physics.Raycast(origin, direction, out hit, Constants.ExtendedRemoveDistance, mask))
            {
                return true;
            }

            if (hit.collider == null)
            {
                return true;
            }

            var piece = hit.collider.GetComponentInParent<Piece>();
            var heightmap = hit.collider.GetComponent<Heightmap>();

            if (piece == null && heightmap != null)
            {
                piece = TerrainModifier.FindClosestModifierPieceInRange(
                    hit.point,
                    Constants.ClosestRangeModifier);
            }

            if (!PieceUtility.IsWaterproofPiece(piece))
            {
                return true;
            }

            if (!piece.m_canBeRemoved)
            {
                __result = false;
                return false;
            }

            Vector3 piecePosition = piece.transform.position;
            Quaternion pieceRotation = piece.transform.rotation;
            Transform pieceTransform = piece.transform;

            if (Location.IsInsideNoBuildLocation(piecePosition))
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_nobuildzone");
                __result = false;
                return false;
            }

            if (!PrivateArea.CheckAccess(piecePosition))
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                __result = false;
                return false;
            }

            bool canRemovePiece = Traverse.Create(__instance)
                .Method("CheckCanRemovePiece", new object[] { piece })
                .GetValue<bool>();

            if (!canRemovePiece)
            {
                __result = false;
                return false;
            }

            var zNetView = piece.GetComponent<ZNetView>();

            if (zNetView == null)
            {
                __result = false;
                return false;
            }

            if (!piece.CanBeRemoved())
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_cantremovenow");
                __result = false;
                return false;
            }

            piece.GetComponent<IRemoved>()?.OnRemoved();

            var wearNTear = piece.GetComponent<WearNTear>();

            if (wearNTear != null)
            {
                wearNTear.Remove();
            }
            else
            {
                Character character = piece.GetComponent<Character>();

                if (character != null)
                {
                    character.Damage(new HitData(Constants.CharacterHitDataValue));
                }
                else
                {
                    ZLog.Log("Removing non WNT object with hammer " + piece.name);

                    zNetView.ClaimOwnership();
                    piece.DropResources();

                    piece.m_placeEffect.Create(
                        piecePosition,
                        pieceRotation,
                        pieceTransform);

                    EffectList removeEffects = Traverse.Create(__instance)
                        .Field("m_removeEffects")
                        .GetValue<EffectList>();

                    removeEffects.Create(piecePosition, Quaternion.identity);

                    ZNetScene.instance.Destroy(piece.gameObject);
                }
            }

            var rightItem = Traverse.Create(__instance)
                .Field("m_rightItem")
                .GetValue<ItemDrop.ItemData>();

            if (rightItem != null)
            {
                Traverse.Create(__instance)
                    .Method("FaceLookDirection")
                    .GetValue();

                var zanim = Traverse.Create(__instance)
                    .Field("m_zanim")
                    .GetValue<ZSyncAnimation>();

                if (zanim != null)
                {
                    zanim.SetTrigger(rightItem.m_shared.m_attack.m_attackAnimation);
                }
            }

            __result = true;
            return false;
        }
    }
}