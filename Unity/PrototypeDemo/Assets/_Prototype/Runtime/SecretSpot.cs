using System.Collections.Generic;
using UnityEngine;

namespace Proto.Runtime
{
    /// Observation point: when the player comes close, a one-line description of what they notice.
    public class SecretSpot : MonoBehaviour
    {
        public string id, title, line;
        public float radius = 3.2f;
        bool inside;
        static readonly List<SecretSpot> all = new List<SecretSpot>();

        void OnEnable() { all.Add(this); DemoHUD.Total = all.Count; }
        void OnDisable() => all.Remove(this);

        void Update()
        {
            var p = PlayerController.Instance;
            if (p == null) return;
            var d = p.transform.position - transform.position;
            bool now = new Vector2(d.x, d.z).magnitude < radius && Mathf.Abs(d.y) < 4f;
            if (now && !inside)
            {
                DemoHUD.Found(id);
                DemoHUD.Say($"<b>{title}.</b> {line}", 7f);
            }
            inside = now;
        }
    }
}
