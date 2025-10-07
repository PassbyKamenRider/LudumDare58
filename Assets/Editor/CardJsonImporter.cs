// Assets/Editor/CardJsonImporter.cs
#if UNITY_EDITOR
using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CardJsonImporter
{
    [MenuItem("Tools/Cards/Import (Resources/CardLoader) - Built-in JSON")]
    public static void ImportAll()
    {
        // Load all JSON TextAssets from Resources/CardLoader
        TextAsset[] files = Resources.LoadAll<TextAsset>("CardLoader");
        if (files == null || files.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Card Import",
                "No JSON files found under Assets/Resources/CardLoader/.\n" +
                "Place your .json there (as TextAssets).",
                "OK");
            return;
        }

        int totalCreated = 0, totalSkipped = 0;

        for (int fi = 0; fi < files.Length; fi++)
        {
            TextAsset ta = files[fi];
            if (ta == null) continue;

            string srcPath = AssetDatabase.GetAssetPath(ta);   // e.g. Assets/Resources/CardLoader/cards.json
            string outDir  = Path.GetDirectoryName(srcPath);   // e.g. Assets/Resources/CardLoader
            if (string.IsNullOrEmpty(outDir)) { totalSkipped++; continue; }
            Directory.CreateDirectory(outDir);

            // JsonUtility cannot parse a top-level JSON array; wrap it:
            // Also: remap "Card Name" -> CardName (JsonUtility can't handle spaces in field names)
            string normalized = NormalizeKeys(ta.text);
            string wrapped = "{\"items\":" + normalized + "}";

            CardRowArray rows;
            try
            {
                rows = JsonUtility.FromJson<CardRowArray>(wrapped);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Card Import] JSON parse error in " + ta.name + ": " + e.Message);
                totalSkipped++;
                continue;
            }

            if (rows == null || rows.items == null || rows.items.Length == 0)
            {
                Debug.LogWarning("[Card Import] No rows found in " + ta.name);
                continue;
            }

            for (int i = 0; i < rows.items.Length; i++)
            {
                CardRow r = rows.items[i];
                if (r == null) { totalSkipped++; continue; }

                string cardName = Safe(r.CardName);
                string classStr = Safe(r.Class);
                string rareStr  = Safe(r.Rarity);
                string codeNum  = Safe(r.CodeNum);

                if (string.IsNullOrWhiteSpace(cardName) || string.IsNullOrWhiteSpace(codeNum))
                {
                    Debug.LogWarning("[Card Import] Skip row: missing CardName or CodeNum");
                    totalSkipped++;
                    continue;
                }

                CardClass cls;
                if (!EnumTryParseTrim(classStr, out cls))
                {
                    Debug.LogWarning("[Card Import] Skip '" + cardName + "': bad Class '" + classStr + "'");
                    totalSkipped++;
                    continue;
                }

                CardRarity rar;
                if (!EnumTryParseTrim(rareStr, out rar))
                {
                    Debug.LogWarning("[Card Import] Skip '" + cardName + "': bad Rarity '" + rareStr + "'");
                    totalSkipped++;
                    continue;
                }

                // Create ScriptableObject
                CardData so = ScriptableObject.CreateInstance<CardData>();
                so.cardID          = codeNum;             // exactly CodeNum
                so.cardName        = cardName.Trim();
                so.cardSprite      = null;                // leave blank
                so.cardCost        = r.Cost;
                so.cardAtk         = r.ATK;
                so.cardDef         = r.DEF;
                so.cardClass       = cls;
                so.cardDescription = NormalizeDesc(Safe(r.Description));
                so.cardRarity      = rar;
                so.cardPrice       = 0f;                  // not in JSON

                string fileName = SanitizeFile(SafeShort(cardName) + "_" + rar + ".asset");
                string path     = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(outDir, fileName));
                AssetDatabase.CreateAsset(so, path);
                totalCreated++;
            }

            Debug.Log("[Card Import] " + ta.name + ": created " + totalCreated + ", skipped " + totalSkipped);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Card Import (Built-in JSON)",
            "Created: " + totalCreated + "\nSkipped: " + totalSkipped, "OK");
    }

    // --------- JSON DTOs (for JsonUtility) ---------
    [Serializable]
    private class CardRowArray { public CardRow[] items; }

    [Serializable]
    private class CardRow
    {
        public string CardName;     // from "Card Name"
        public string Class;        // "Class"
        public int    Cost;         // "Cost"
        public string Description;  // "Description"
        public int    ATK;          // "ATK"
        public int    DEF;          // "DEF"
        public string Rarity;         // "Rarity"
        public string CodeNum;      // "CodeNum"
    }

    // --------- helpers ---------
    private static string NormalizeKeys(string json)
    {
        if (string.IsNullOrEmpty(json)) return "[]";

        // Carefully replace the specific key with a space: "Card Name" -> CardName
        // (Only replaces keys; values containing that phrase are extremely unlikely,
        // but we pattern match the exact JSON key label form to be safe.)
        // NOTE: JsonUtility is strict about field names.
        string s = json;

        // Ensure CRLF normalized (not strictly required)
        s = s.Replace("\r\n", "\n").Replace("\r", "\n");

        // Replace the *key* "Card Name": we look for the exact JSON token with quotes and colon.
        // This is a simple, targeted transformation and avoids bringing in external JSON libs.
        s = s.Replace("\"Card Name\"", "\"CardName\"");

        return s;
    }

    private static string Safe(string s) => s ?? "";

    private static string NormalizeDesc(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd();
    }

    private static bool EnumTryParseTrim<T>(string s, out T value) where T : struct
    {
        if (s == null) { value = default(T); return false; }
        string t = s.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
        return Enum.TryParse<T>(t, true, out value);
    }

    private static string SafeShort(string s)
    {
        if (string.IsNullOrEmpty(s)) return "noname";
        string t = s.Trim();
        char[] bad = Path.GetInvalidFileNameChars();
        for (int i = 0; i < bad.Length; i++) t = t.Replace(bad[i].ToString(), "");
        t = t.Replace(",", "").Replace("\"", "").Replace("\n", " ").Replace("\t", " ");
        if (t.Length > 24) t = t.Substring(0, 24);
        return t;
    }

    private static string SanitizeFile(string s)
    {
        string t = s ?? "";
        char[] bad = Path.GetInvalidFileNameChars();
        for (int i = 0; i < bad.Length; i++) t = t.Replace(bad[i].ToString(), "");
        return t;
    }
}
#endif
