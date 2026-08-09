namespace HexWaterproofBuilding.Models
{
    internal class PierDefinition
    {
        internal string PrefabName { get; }
        internal float SegmentLength { get; }
        internal float VerticalOffset { get; }
        internal float InteractionColliderWidth { get; }
        internal float InteractionColliderTopOffset { get; }

        internal PierDefinition(string prefabName, float segmentLength, float verticalOffset, float interactionColliderWidth, float interactionColliderTopOffset)
        {
            PrefabName = prefabName;
            SegmentLength = segmentLength;
            VerticalOffset = verticalOffset;
            InteractionColliderWidth = interactionColliderWidth;
            InteractionColliderTopOffset = interactionColliderTopOffset;
        }
    }
}