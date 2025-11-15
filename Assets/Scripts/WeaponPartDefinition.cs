using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponPart", menuName = "WeaponMaster/Weapon Part Definition")]
public class WeaponPartDefinition : ScriptableObject
{
    [Header("Part Identity")]
    public WeaponPartId partId;
    public string displayName;
    public int tier;

    [Header("Visuals")]
    public GameObject worldPrefab;

    [Header("UI Display Settings")]
    public Vector3 uiRotation = new Vector3(0, 0, 0); // Her parçanýn UI'daki rotasyonu
    public float uiScale = 1.0f;                      // Her parçanýn UI'daki ölçeði

    [Header("Attachment Settings")]
    public float attachmentScale = 1f; 
    public Vector3 attachmentRotation = Vector3.zero;
    public float attachmentYOffset = 0f; // YENÝ: Silaha takýldýðýndaki dikey pozisyonu

    [Header("Stats")]
    public float statBonus = 5f;
    
    public float GetTotalBonus()
    {
        return statBonus * tier;
    }
}

