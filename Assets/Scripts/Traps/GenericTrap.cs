using UnityEngine;
using System.Collections.Generic;

public class GenericTrap : MonoBehaviour
{
    [System.Serializable]
    public struct RendererIdentifier
    {
        public int materialIndex;
        public Renderer renderer;
    }

    public enum TrapState { placed, placing }
    public enum TrapColor { normal, flashWhite, flashRed };

    [Header("Make the following item's indices match each other.")]
    public Material[] transparentMaterials;
    public Material[] opaqueMaterials;
    public RendererIdentifier[] trapRenderers;

    [Header("Collider of this trap. Must have no scaling!")]
    public BoxCollider trapCollider;

    public int cost;
    [Header("If barricade, pls set to very high ...")]
    public float pathingPenalty;

    private TrapState state;
    private TrapColor color;

    /// <summary>
    /// Call this on each frame during placing.
    /// </summary>
    public void PlacingUpdate(
        BuildingResourceManager buildResourceManager,
        LayerMask characterLayerMask)
    {
        if (cost <= buildResourceManager.GetBuildingResource() &&
            CheckForCharacters(characterLayerMask))
            SetInternalState(TrapState.placing, TrapColor.flashWhite);
        else
            SetInternalState(TrapState.placing, TrapColor.flashRed);
    }

    public void StartDelete()
    {
        if (state == TrapState.placed)
            SetInternalState(TrapState.placed, TrapColor.flashRed);
        else
            Debug.LogError("Cannot delete a trap is not placed down!");
    }

    public void EndDelete(
        bool confirm,
        BuildingResourceManager buildResourceManager,
        float refundAmount)
    {
        if (state == TrapState.placed)
        {
            if (confirm)
            {
                Destroy(gameObject);
                buildResourceManager.AddBuildingResource(GetRefundCost(refundAmount));
            }
            else
                SetInternalState(TrapState.placed, TrapColor.normal);
        }
        else
            Debug.LogError("Cannot delete a trap is not placed down!");
    }

    public bool TryPlace(
        BuildingResourceManager buildResourceManager,
        LayerMask characterLayerMask,
        TrapSlot trapSlot)
    {
        if (trapSlot.IsTrapPlaced() == false &&
            CheckForCharacters(characterLayerMask) &&
            buildResourceManager.TakeBuildingResource(cost))
        {
            GenericTrap newTrap = trapSlot.CopyAndPlace(this);
            newTrap.SetInternalState(TrapState.placed, TrapColor.normal);
            return true;
        }
        else return false;
    }

    public int GetRefundCost(float refundAmount)
    {
        return Mathf.FloorToInt(cost * refundAmount);
    }

    /// <param name="state">Is it flashing?</param>
    /// <param name="isPhysical">Physical collider turned on?</param>
    private void SetInternalState(TrapState state, TrapColor color)
    {
        this.state = state;
        this.color = color;

        trapCollider.enabled = state == TrapState.placed;
        Material[] setMaterials = color == TrapColor.normal ? opaqueMaterials : transparentMaterials;

        UnityEngine.Rendering.ShadowCastingMode shadowMode = 
            color == TrapColor.normal ?
            UnityEngine.Rendering.ShadowCastingMode.On :
            UnityEngine.Rendering.ShadowCastingMode.Off;

        for (int i = 0; i < trapRenderers.Length; ++i)
        {
            trapRenderers[i].renderer.material = setMaterials[trapRenderers[i].materialIndex];
            trapRenderers[i].renderer.shadowCastingMode = shadowMode;
        }
    }

    private Color GetFlashedColor()
    {
        return new Color(1.0f, 1.0f, 1.0f,
            Mathf.Sin(2 * Mathf.PI * Time.time) * 0.3f + 0.7f);
    }

    private void FixedUpdate()
    {
        switch (color)
        {
            case TrapColor.flashWhite:
                for (int i = 0; i < transparentMaterials.Length; ++i)
                    transparentMaterials[i].color = GetFlashedColor();
                break;
            case TrapColor.flashRed:
                for (int i = 0; i < transparentMaterials.Length; ++i)
                {
                    Color flashedColor = GetFlashedColor();
                    // just red pls
                    flashedColor.g = 0;
                    flashedColor.b = 0;
                    transparentMaterials[i].color = flashedColor;
                }
                break;
            default:
                break;
        }
    }

    /// <returns>true if true when no characters, otherwise false</returns>
    private bool CheckForCharacters(LayerMask characterLayerMask)
    {
        return 
            Physics.CheckBox(transform.position, trapCollider.size / 2.0f, transform.rotation, characterLayerMask) == false;
    }

    [ContextMenu("Find Renderers And Materials")]
    private void FindRenderersAndMaterials()
    {
        List<Material> materials = new List<Material>();
        List<RendererIdentifier> renderers = new List<RendererIdentifier>();
        Queue<Transform> children = new Queue<Transform>();

        children.Enqueue(transform);
        while (0 < children.Count)
        {
            Transform child = children.Dequeue();
            for (int i = 0; i < child.childCount; ++i)
                children.Enqueue(child.GetChild(i));

            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = renderer.sharedMaterial;
                int index = materials.IndexOf(mat);
                if (-1 < index)
                {
                    renderers.Add(new RendererIdentifier() { renderer = renderer, materialIndex = index });
                }
                else
                {
                    materials.Add(mat);
                    renderers.Add(new RendererIdentifier() { renderer = renderer, materialIndex = materials.Count - 1 });
                }
            }
        }

        transparentMaterials = new Material[materials.Count];
        opaqueMaterials = materials.ToArray();
        trapRenderers = renderers.ToArray();
    }
}
