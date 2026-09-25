using UnityEngine;

namespace Arkus.CITY
{
    // Local spatial-measurement controls only. No CITY or gameplay semantics live here.
    [RequireComponent(typeof(CharacterController))]
    public sealed class City04TraversalProbe : MonoBehaviour
    {
        public Camera View;
        public GameObject X5Crossing;
        public GameObject RouteABarrier;
        public GameObject BusyMarkers;
        public GameObject ProxyActor;

        private CharacterController controller;
        private float pitch;
        private float verticalSpeed;
        private bool x5Available = true;
        private bool routeABlocked;
        private bool busy;
        private float legStart;
        private Vector2[] proxyRoute;
        private int proxyNext;
        private string lastStart = "free walk";

        private static readonly Vector2[] Stops =
        {
            new Vector2(42, -64), new Vector2(50, -32), new Vector2(80, 35),
            new Vector2(92, 52), new Vector2(150, 58), new Vector2(195, 70),
            new Vector2(0, 0), new Vector2(92, 108), new Vector2(90, 122)
        };

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            legStart = Time.time;
        }

        private void Update()
        {
            for (int i = 0; i < Stops.Length; i++)
            {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
                {
                    var p = Stops[i];
                    controller.enabled = false;
                    transform.position = new Vector3(p.x, GroundHeight(p) + 0.15f, p.y);
                    controller.enabled = true;
                    verticalSpeed = 0;
                    legStart = Time.time;
                    lastStart = new[] { "O.X1", "W.X1", "W.CASCO", "F01", "W.PLAZA", "W.SHOP", "W.LANDING", "W.X5", "E.X5" }[i];
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                x5Available = !x5Available;
                if (X5Crossing != null)
                {
                    foreach (var collider in X5Crossing.GetComponentsInChildren<Collider>())
                        collider.enabled = x5Available;
                    var renderer = X5Crossing.GetComponentInChildren<Renderer>();
                    if (renderer != null) renderer.enabled = x5Available;
                }
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                routeABlocked = !routeABlocked;
                if (RouteABarrier != null) RouteABarrier.SetActive(routeABlocked);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                busy = !busy;
                if (BusyMarkers != null) BusyMarkers.SetActive(busy);
            }
            if (Input.GetKeyDown(KeyCode.T)) legStart = Time.time;
            if (Input.GetKeyDown(KeyCode.L))
                Debug.Log("CITY04_HUMAN_LEG start=" + lastStart + " end_U=" + transform.position.x.ToString("F2") +
                          " end_V=" + transform.position.z.ToString("F2") + " elapsed_s=" + (Time.time - legStart).ToString("F1") +
                          " X5=" + (x5Available ? "AVAILABLE" : "CLOSED") +
                          " micro_A=" + (routeABlocked ? "BLOCKED" : "OPEN"));
            if (Input.GetKeyDown(KeyCode.Z)) StartProxy("casco.micro.A");
            if (Input.GetKeyDown(KeyCode.X)) StartProxy("casco.micro.B");
            UpdateProxy();

            if (Input.GetMouseButton(1))
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * 2f, 0);
                pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2f, -75f, 75f);
                if (View != null) View.transform.localEulerAngles = new Vector3(pitch, 0, 0);
            }

            Vector3 move = (transform.right * Input.GetAxisRaw("Horizontal") +
                            transform.forward * Input.GetAxisRaw("Vertical")).normalized;
            float speed = Input.GetKey(KeyCode.LeftShift) ? 3.5f : 1.4f;
            verticalSpeed = controller.isGrounded ? -0.5f : verticalSpeed - 9.81f * Time.deltaTime;
            controller.Move((move * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 600, 135),
                "CITY-04 physical probe — greybox only\n" +
                "WASD move, right mouse look, Shift faster; 1-9 anchor starts, T timer reset, L log leg\n" +
                "G X5 low-water crossing: " + (x5Available ? "AVAILABLE" : "CLOSED") +
                "   B micro.A: " + (routeABlocked ? "BLOCKED" : "OPEN") +
                "   Q market: " + (busy ? "BUSY" : "QUIET") +
                "\nZ proxy micro.A, X proxy micro.B (spatial follow marker only)" +
                "\nElapsed leg: " + (Time.time - legStart).ToString("F1") + " s");
        }

        private void StartProxy(string routeId)
        {
            if (ProxyActor == null) return;
            foreach (var route in City04Layout.ROUTES)
            {
                if (route.Id != routeId) continue;
                proxyRoute = route.Points;
                proxyNext = 1;
                ProxyActor.transform.position = new Vector3(proxyRoute[0].x,
                    GroundHeight(proxyRoute[0]) + 0.85f, proxyRoute[0].y);
                ProxyActor.SetActive(true);
                Debug.Log("CITY04_PROXY_START route=" + routeId);
                return;
            }
        }

        private void UpdateProxy()
        {
            if (ProxyActor == null || proxyRoute == null || proxyNext >= proxyRoute.Length) return;
            Vector2 point = proxyRoute[proxyNext];
            Vector3 goal = new Vector3(point.x, GroundHeight(point) + 0.85f, point.y);
            ProxyActor.transform.position = Vector3.MoveTowards(ProxyActor.transform.position, goal, Time.deltaTime);
            if (Vector3.Distance(ProxyActor.transform.position, goal) < 0.01f)
            {
                proxyNext++;
                if (proxyNext == proxyRoute.Length)
                    Debug.Log("CITY04_PROXY_FINISH");
            }
        }

        public static float GroundHeight(Vector2 p)
        {
            float bridgehead = 2.5f * Mathf.Exp(-((p.x - 46f) * (p.x - 46f) +
                (p.y + 48f) * (p.y + 48f)) / 1800f);
            return 0.04f * Mathf.Min(p.x, 150f) + 0.02f * p.y + bridgehead;
        }
    }
}
