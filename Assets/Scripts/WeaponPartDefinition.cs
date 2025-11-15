using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Part", menuName = "Weapon/Weapon Part Definition")]
public class WeaponPartDefinition: ScriptableObject
{
    [Header("Part Identity")]
    public WeaponPartId partId;
    public string displayName;
    public int tier;

    [Header("Visuals")]
    public GameObject worldPrefab;

    [Header("UI Display Settings")]
    public Vector3 uiRotation;
    public float uiScale = 1.0f;

    [Header("Attachment Settings")]
    public float attachmentScale = 1.0f;
    public Vector3 attachmentRotation;
    public float attachmentYOffset;

    [Header("Stats")]
        public float statBonus = 0f;

    public float GetTotalBonus()
    {
        return statBonus * tier;
    }
}

