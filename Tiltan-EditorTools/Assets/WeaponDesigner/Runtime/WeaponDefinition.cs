using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Definition", menuName = "Weapon Designer/Weapon Definition")]
/// Just holds the basic weapon data.
public class WeaponDefinition : ScriptableObject
{
    public string weaponName = "New Weapon";
    public Sprite icon;
    [TextArea(2, 4)] public string description = "A simple weapon definition.";

    public DamageStat damage = new DamageStat();

    public bool usesAmmo = true;
    [ConditionalField(nameof(usesAmmo), true)] public int magazineSize = 12;
    [ConditionalField(nameof(usesAmmo), true)] public float reloadTime = 1.4f;
}
