using UnityEngine;

public enum SkillType { Shield, Teleport }

[CreateAssetMenu(fileName = "SpinnerData", menuName = "Spinner/SpinnerData")]
public class SpinnerData : ScriptableObject
{
    [Header("Identity")]
    public string spinnerName;
    public Sprite icon;
    public Color tint = Color.white;
    [TextArea(2, 4)] public string description;

    [Header("Skill Type")]
    public SkillType skillType = SkillType.Shield;

    [Header("Spin")]
    public float minSpinSpeed = 180f;
    public float maxSpinSpeed = 1440f;

    [Header("Move")]
    public float minMoveSpeed = 3f;
    public float maxMoveSpeed = 14f;
    public float acceleration = 1.5f;
    public float deceleration = 2.5f;
    public float reverseThreshold = 0.05f;
    public float idleDeceleration = 3f;

    [Header("Skill - Chung")]
    public float skillCooldown = 3f;
    public float fadeAmount = 0.5f;

    [Header("Skill - Shield")]
    public float shieldDuration = 2f;
    public float shieldRadius = 1.2f;

    [Header("Skill - Teleport (Skill 3)")]
    public float teleportRange = 5f;

    [Header("Health")]
    public int maxHealth = 5;
}
