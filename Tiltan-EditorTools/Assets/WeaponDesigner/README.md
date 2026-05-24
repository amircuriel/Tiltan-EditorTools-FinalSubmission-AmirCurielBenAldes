# Weapon Designer

Weapon Designer is a small Unity Editor extension for configuring simple weapon data.

## How to Test

1. Open a weapon asset in `Assets/WeaponDesigner/Examples`.
2. Change the name, sprite icon, description, damage value, and ammo toggle.
3. Open `Assets/WeaponDesigner/Examples/WeaponPreviewTester.prefab` to see the second custom inspector.
4. Use the inspector button to ping the assigned weapon asset.
5. If the tester prefab is missing, recreate it from `Weapon Designer/Create Preview Tester Prefab`.

## Requirement Map

- Custom Inspectors: `WeaponDefinitionEditor`, `WeaponPreviewTesterEditor`
- Property Drawers: `DamageStatDrawer`, `ConditionalFieldDrawer`
- Images in editor: weapon sprite previews, including sliced sprites from an atlas
- Show/hide values: ammo fields hide when `usesAmmo` is disabled
- Validation: missing or invalid sections turn red, with HelpBoxes for error/warning states
- Styling: simple headers, boxed sections, spacing, and HelpBoxes
