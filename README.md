# Tiltan-EditorTools-FinalSubmission-AmirCurielBenAldes
סיקור כללי של הפרויקט:
`Weapon Designer` הוא כלי קטן לעורך של Unity,שמיועד להגדרה וניהול של נתוני נשקים פשוטים.

איך לבדוק:
1. לפתוח קובץ נשק מתוך `Assets/WeaponDesigner/Examples`.
2. לשנות שם, אייקון ספרייט, תיאור, ערך נזק, ואת כמות התחמושת וזמן הטעימה.
3. לפתוח את `Assets/WeaponDesigner/Examples/WeaponPreviewTester.prefab` כדי לראות את ה-custom inspector השני.
4. להשתמש בכפתור בתוך ה-inspector כדי לסמן את קובץ הנשק המשויך.
5. אם ה-prefab של הבדיקה חסר, ליצור אותו מחדש דרך `Weapon Designer/Create Preview Tester Prefab`.

איך ענינו על דרישות המטלה:
- Custom Inspectors: `WeaponDefinitionEditor`, `WeaponPreviewTesterEditor`
- Property Drawers: `DamageStatDrawer`, `ConditionalFieldDrawer`
- תמונות בתוך העורך: תצוגות מקדימות של ספרייטים לנשקים, כולל ספרייטים חתוכים מתוך atlas.
- הצגה והסתרה של ערכים: שדות תחמושת מוסתרים כאשר `usesAmmo` כבוי.
- ולידציה: אזורים חסרים או לא תקינים נצבעים באדום ומציגים HelpBoxes למצבי שגיאה או אזהרה.
- עיצוב: כותרות פשוטות, אזורים ממוסגרים, ריווחים, ו-HelpBoxes.
