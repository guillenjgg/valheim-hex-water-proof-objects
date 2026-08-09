using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Models;
using UnityEngine;

namespace HexWaterproofBuilding.Components
{
    internal class PierComponent : MonoBehaviour
    {
        private const float MaxRaycastDistance = 100f;
        private const float GhostUpdateInterval = 0.1f;
        private const string GeneratedSupportsName = "GeneratedSupports";
        private const string InteractionColliderName = "PierInteractionCollider";
        private const string CloneSuffix = "(Clone)";

        private bool _isPlacementGhost;
        private bool? _lastGhostValidation;
        private int _terrainMask;
        private ZNetView _nview;
        private WearNTear _wear;
        private Transform _visual;
        private Vector3 _originalVisualLocalPosition;
        private PierDefinition _definition;

        private void Awake()
        {
            _isPlacementGhost = ZNetView.m_forceDisableInit;
            _terrainMask = LayerMask.GetMask("terrain");
            _nview = GetComponent<ZNetView>();
            _wear = GetComponent<WearNTear>();

            string prefabName = GetPrefabName(gameObject);
            _definition = PierDefinitionRegistry.Get(prefabName);

            if (_definition == null)
            {
                Jotunn.Logger.LogWarning($"No pier definition found for {prefabName}.");
                return;
            }

            if (_wear == null)
            {
                Jotunn.Logger.LogWarning($"{prefabName} has no WearNTear component.");
                return;
            }

            _wear.m_noSupportWear = false;

            if (_wear.m_new == null)
            {
                Jotunn.Logger.LogWarning($"{prefabName} has no WearNTear new visual.");
                return;
            }

            _visual = _wear.m_new.transform;
            _originalVisualLocalPosition = _visual.localPosition;

            ApplyVisualOffset();
        }

        private void Start()
        {
            if (_definition == null || _wear == null || _visual == null)
            {
                return;
            }

            if (_isPlacementGhost)
            {
                InvokeRepeating(nameof(UpdatePlacementGhost), 0f, GhostUpdateInterval);
                return;
            }

            if (_nview == null || _nview.GetZDO() == null)
            {
                return;
            }

            CheckPlacedPiece();
        }

        private void ApplyVisualOffset()
        {
            Vector3 offset = Vector3.up * _definition.VerticalOffset;

            ApplyOffsetToVisual(_wear.m_new, offset);

            if (_wear.m_worn != _wear.m_new)
            {
                ApplyOffsetToVisual(_wear.m_worn, offset);
            }

            if (_wear.m_broken != _wear.m_new && _wear.m_broken != _wear.m_worn)
            {
                ApplyOffsetToVisual(_wear.m_broken, offset);
            }
        }

        private void ApplyOffsetToVisual(GameObject visualObject, Vector3 offset)
        {
            if (visualObject == null)
            {
                return;
            }

            visualObject.transform.localPosition += offset;
        }

        private void UpdatePlacementGhost()
        {
            RaycastHit hit;
            bool isValid = IsOverUnderwaterTerrain(out hit);

            if (_lastGhostValidation.HasValue && _lastGhostValidation.Value == isValid)
            {
                return;
            }

            _lastGhostValidation = isValid;
        }

        private void CheckPlacedPiece()
        {
            RaycastHit hit;

            if (!IsOverUnderwaterTerrain(out hit))
            {
                return;
            }

            int segmentCount = Mathf.CeilToInt(hit.distance / _definition.SegmentLength);
            int additionalSegments = Mathf.Max(0, segmentCount - 1);

            BuildSupports(additionalSegments);
            BuildInteractionCollider(segmentCount);
        }

        private void BuildSupports(int additionalSegments)
        {
            if (_visual == null)
            {
                return;
            }

            Transform existingContainer = transform.Find(GeneratedSupportsName);

            if (existingContainer != null)
            {
                Destroy(existingContainer.gameObject);
            }

            if (additionalSegments <= 0)
            {
                return;
            }

            GameObject container = new GameObject(GeneratedSupportsName);
            container.transform.SetParent(transform, false);

            Vector3 shiftedVisualPosition =
                _originalVisualLocalPosition +
                Vector3.up * _definition.VerticalOffset;

            for (int i = 1; i <= additionalSegments; i++)
            {
                GameObject segment = Instantiate(_visual.gameObject, container.transform);

                segment.name = $"Segment_{i}";
                segment.transform.localPosition =
                    shiftedVisualPosition +
                    Vector3.down * _definition.SegmentLength * i;

                segment.transform.localRotation = _visual.localRotation;
                segment.transform.localScale = _visual.localScale;
                segment.SetActive(true);

                Collider[] colliders = segment.GetComponentsInChildren<Collider>();

                foreach (Collider collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }

        private void BuildInteractionCollider(int segmentCount)
        {
            if (segmentCount <= 0)
            {
                return;
            }

            Transform interactionTransform = transform.Find(InteractionColliderName);
            GameObject interactionObject;

            if (interactionTransform == null)
            {
                interactionObject = new GameObject(InteractionColliderName);
                interactionObject.transform.SetParent(transform, false);
            }
            else
            {
                interactionObject = interactionTransform.gameObject;
            }

            interactionObject.layer = gameObject.layer;
            interactionObject.transform.localRotation = Quaternion.identity;
            interactionObject.transform.localScale = Vector3.one;

            float totalHeight = _definition.SegmentLength * segmentCount;
            float topY = _definition.VerticalOffset + _definition.SegmentLength * 0.5f + _definition.InteractionColliderTopOffset;
            float bottomY = topY - totalHeight;
            float centerY = (topY + bottomY) * 0.5f;

            interactionObject.transform.localPosition = new Vector3(0f, centerY, 0f);

            BoxCollider interactionCollider = interactionObject.GetComponent<BoxCollider>();

            if (interactionCollider == null)
            {
                interactionCollider = interactionObject.AddComponent<BoxCollider>();
            }

            interactionCollider.center = Vector3.zero;
            interactionCollider.size = new Vector3(
                _definition.InteractionColliderWidth,
                totalHeight,
                _definition.InteractionColliderWidth
            );

            interactionCollider.isTrigger = false;
            interactionCollider.enabled = true;
        }

        private bool IsOverUnderwaterTerrain(out RaycastHit hit)
        {
            hit = default(RaycastHit);

            if (ZoneSystem.instance == null)
            {
                return false;
            }

            if (!Physics.Raycast(
                transform.position,
                Vector3.down,
                out hit,
                MaxRaycastDistance,
                _terrainMask,
                QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            return hit.point.y < ZoneSystem.instance.m_waterLevel;
        }

        private static string GetPrefabName(GameObject prefab)
        {
            if (prefab == null)
            {
                return string.Empty;
            }

            string prefabName = prefab.name;

            if (prefabName.EndsWith(CloneSuffix))
            {
                prefabName = prefabName.Substring(0, prefabName.Length - CloneSuffix.Length);
            }

            return prefabName;
        }

        private void OnDestroy()
        {
            if (_isPlacementGhost)
            {
                CancelInvoke(nameof(UpdatePlacementGhost));
            }
        }
    }
}