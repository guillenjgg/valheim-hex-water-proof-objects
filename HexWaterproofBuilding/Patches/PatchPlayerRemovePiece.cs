using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
    internal static class PatchPlayerRemovePiece
    {
        private static readonly AccessTools.FieldRef<Player, int> RemoveRayMaskRef = AccessTools.FieldRefAccess<Player, int>("m_removeRayMask");

        private static readonly AccessTools.FieldRef<Player, EffectList> RemoveEffectsRef = AccessTools.FieldRefAccess<Player, EffectList>("m_removeEffects");

        private static readonly AccessTools.FieldRef<Player, ItemDrop.ItemData> RightItemRef = AccessTools.FieldRefAccess<Player, ItemDrop.ItemData>("m_rightItem");

        private static readonly AccessTools.FieldRef<Character, ZSyncAnimation> ZanimRef = AccessTools.FieldRefAccess<Character, ZSyncAnimation>("m_zanim");

        private static readonly FastInvokeHandler CheckCanRemovePieceRef = MethodInvoker.GetHandler(AccessTools.Method(typeof(Player), "CheckCanRemovePiece"));

        private static readonly FastInvokeHandler FaceLookDirectionRef = MethodInvoker.GetHandler(AccessTools.Method(typeof(Player), "FaceLookDirection"));

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

            int mask = RemoveRayMaskRef(__instance);

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

            if (!PieceUtility.CanUseExtendedPlacementForPiece(piece))
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

            bool canRemovePiece = (bool)CheckCanRemovePieceRef(__instance, piece);

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

                    EffectList removeEffects = RemoveEffectsRef(__instance);

                    removeEffects.Create(piecePosition, Quaternion.identity);

                    ZNetScene.instance.Destroy(piece.gameObject);
                }
            }

            var rightItem = RightItemRef(__instance);

            if (rightItem != null)
            {
                FaceLookDirectionRef(__instance);

                var zanim = ZanimRef(__instance);

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