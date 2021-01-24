using UnityEngine;

namespace Infra.Gameplay {
    /// <summary>
    /// Updates its mesh according to its collider.
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D), typeof(MeshFilter))]
    // [ExecuteInEditMode]
    public class PolygonColliderWithMesh : MonoBehaviour {
        [SerializeField] bool autoUpdateToMatchPolygon;

        private PolygonCollider2D polygonCollider;
        private MeshFilter meshFilter;

        protected void Awake() {
            polygonCollider = GetComponent<PolygonCollider2D>();
            meshFilter = GetComponent<MeshFilter>();
            if (Application.isPlaying) {
                // In edit mode, this will cause leaks.
                InstantiateNewMesh();
                RandomizePolygonPoints();
            }

        }

        /// <summary>
        /// Randomize the polygon points of the mesh
        /// </summary>
        public void RandomizePolygonPoints()
        {
            Vector2[] points = new Vector2[13];
            // First 5 points need to be hard coded to create the first 5 points of the polygon collider
            points[0] = new Vector2(-0.4f, Random.Range(0.1f, 0.7f));
            points[1] = new Vector2(-0.5f, 0.5f);
            points[2] = new Vector2(-0.5f, -0.5f);
            points[3] = new Vector2(0.5f, -0.5f);
            points[4] = new Vector2(0.5f, 0.5f);
            float xPos = 0.4f;
            for (int i = 5; i < points.Length; i++)
            {
                points[i] = new Vector2(xPos + Random.Range(0.01f, 0.1f), Random.Range(0.1f, 0.75f));
                xPos -= 0.1f;
            }
            SetPoints(points);
            
        }

        [ContextMenu("Instantiate New Mesh")]
        private void InstantiateNewMesh() {
            meshFilter.sharedMesh = meshFilter.mesh;
        }

        protected void Update() {
            if (autoUpdateToMatchPolygon) {
                UpdateMesh();
            }
        }

        /// <summary>
        /// Sets the bounding points of the mesh and the collider.
        /// </summary>
        public void SetPoints(Vector2[] points) {
            polygonCollider.points = points;
            UpdateMesh();
        }

        /// <summary>
        /// Sets the bounding points of the mesh to the collider.
        /// </summary>
        public void UpdateMesh() {
            var points = polygonCollider.points;

            // Triangulate points for mesh.
            int[] indices = Triangulator.Triangulate(points);

            // Create 3D mesh vertices.
            var vertices = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++) {
                var point = points[i];
                vertices[i] = point;
            }

            // Reset mesh.
            Mesh sharedMesh;
            (sharedMesh = meshFilter.sharedMesh).Clear();
            sharedMesh.vertices = vertices;
            sharedMesh.triangles = indices;
            meshFilter.sharedMesh.RecalculateBounds();
        }
    }
}
