using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Proto.Runtime
{
    /// Minimal HUD (IMGUI): controls, one-line observations, dialogue line, prompt, ford state.
    public class DemoHUD : MonoBehaviour
    {
        static string message, speaker, prompt;
        static float messageUntil, lineUntil;
        public static bool Talking => Time.time < lineUntil && speaker != null && holdStill;
        static bool holdStill;
        static readonly HashSet<string> found = new HashSet<string>();
        public static int Total;
        bool showHelp = true;
        GUIStyle box, text, small, title;

        public static void Say(string msg, float seconds) { message = msg; messageUntil = Time.time + seconds; }
        public static void Line(string who, string line, float seconds) { speaker = who; message = line; messageUntil = lineUntil = Time.time + seconds; holdStill = false; }
        public static void Prompt(string p) => prompt = p;
        public static void Found(string id) => found.Add(id);

        void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.hKey.wasPressedThisFrame) showHelp = !showHelp;
            if (Time.time > lineUntil) speaker = null;
        }

        void Styles()
        {
            if (box != null) return;
            var bg = new Texture2D(1, 1); bg.SetPixel(0, 0, new Color(0.06f, 0.07f, 0.08f, 0.62f)); bg.Apply();
            box = new GUIStyle(GUI.skin.box) { normal = { background = bg }, padding = new RectOffset(14, 14, 10, 10), alignment = TextAnchor.MiddleLeft };
            text = new GUIStyle(GUI.skin.label) { wordWrap = true, normal = { textColor = new Color(0.95f, 0.93f, 0.88f) } };
            small = new GUIStyle(text) { wordWrap = false };
            title = new GUIStyle(text) { fontStyle = FontStyle.Bold, wordWrap = false };
        }

        void OnGUI()
        {
            Styles();
            float s = Screen.height / 900f;
            text.fontSize = Mathf.RoundToInt(19 * s); small.fontSize = Mathf.RoundToInt(15 * s); title.fontSize = Mathf.RoundToInt(22 * s);
            // title
            GUI.Label(new Rect(20 * s, 16 * s, 700 * s, 30 * s), "Del Puente Viejo al Bar", title);
            GUI.Label(new Rect(20 * s, 42 * s, 700 * s, 24 * s), $"Prototipo Juego2 · no canónico · hallazgos {found.Count}/{Total}", small);
            // ford state
            if (Ford.Instance != null)
                GUI.Label(new Rect(Screen.width - 320 * s, 16 * s, 300 * s, 24 * s), Ford.Instance.high ? "Vado X5: AGUAS ALTAS" : "Vado X5: aguas bajas", small);
            if (showHelp)
            {
                var r = new Rect(20 * s, Screen.height - 150 * s, 520 * s, 130 * s);
                GUI.Box(r, GUIContent.none, box);
                GUI.Label(new Rect(r.x + 14 * s, r.y + 8 * s, r.width, r.height),
                    "WASD andar · Shift correr · Ratón cámara · Rueda zoom\nE hablar · F aguas bajas/altas del vado · R volver al inicio\nEsc liberar ratón (clic para recuperarlo) · H ocultar ayuda", small);
            }
            if (!string.IsNullOrEmpty(prompt))
            {
                var pr = new Rect(Screen.width * 0.5f - 160 * s, Screen.height * 0.62f, 320 * s, 34 * s);
                GUI.Box(pr, GUIContent.none, box);
                GUI.Label(new Rect(pr.x + 12 * s, pr.y + 5 * s, pr.width, pr.height), prompt, small);
            }
            prompt = null;
            if (Time.time < messageUntil && !string.IsNullOrEmpty(message))
            {
                string full = speaker != null ? $"<b>{speaker}:</b> {message}" : message;
                text.richText = true;
                float w = Mathf.Min(900 * s, Screen.width - 40 * s);
                float h = text.CalcHeight(new GUIContent(full), w - 28 * s) + 22 * s;
                var r = new Rect((Screen.width - w) * 0.5f, Screen.height - h - 40 * s, w, h);
                GUI.Box(r, GUIContent.none, box);
                GUI.Label(new Rect(r.x + 14 * s, r.y + 10 * s, w - 28 * s, h), full, text);
            }
        }
    }
}
