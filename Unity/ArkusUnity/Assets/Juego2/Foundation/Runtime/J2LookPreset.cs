using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Fixed overcast baseline look (H2F-01 S01): sun, trilight ambient, exp² fog, generated panoramic sky and the
    /// global Volume profile. A scene applies it through the Juego2 editor utility; final lighting is ART/CITY-owned.
    /// </summary>
    [CreateAssetMenu(menuName = "Juego2/Foundation/Look Preset", fileName = "J2_LookPreset")]
    public sealed class J2LookPreset : ScriptableObject
    {
        public Color sunColor = new Color(0.83f, 0.89f, 0.93f);
        public float sunIntensity = 0.9f;
        public Vector3 sunEuler = new Vector3(43f, -28f, 0f);
        public Color ambientSky = new Color(0.62f, 0.67f, 0.7f);
        public Color ambientEquator = new Color(0.5f, 0.54f, 0.54f);
        public Color ambientGround = new Color(0.3f, 0.32f, 0.3f);
        public Color fogColor = new Color(0.66f, 0.7f, 0.72f);
        public float fogDensity = 0.012f;
        public Material skybox;
        public UnityEngine.Object volumeProfile;
    }
}
