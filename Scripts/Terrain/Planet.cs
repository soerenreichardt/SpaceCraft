using Noise.Api;
using Terrain.Height;
using Terrain.Mesh;
using UnityEngine;

namespace Terrain
{
    public class Planet
    {
        public const float SCALE = 0.1f;

        private static readonly string[] DirectionNames = { "up", "down", "left", "right", "front", "back" };
        private static readonly Vector3[] Directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        private readonly MeshGenerator meshGenerator;
        private readonly TerrainQuadTree[] planetSides;
        private readonly float diameter;

        public Planet(INoiseEvaluator noiseEvaluator, BaseTerrainSettings terrainSettings, Material chunkMaterial, Transform transform)
        {
            this.meshGenerator = new MeshGenerator(noiseEvaluator, terrainSettings);
            this.planetSides = new TerrainQuadTree[6];
            this.diameter = Mathf.Pow(2, terrainSettings.planetSize) * SCALE;

            InitializePlanetSides(terrainSettings.planetSize, chunkMaterial, meshGenerator, transform);
        }

        private void InitializePlanetSides(int planetSize, Material chunkMaterial, MeshGenerator meshGenerator, Transform transform)
        {
            var planetDiameter = Mathf.Pow(2, planetSize) * SCALE;
            for (var i = 0; i < 6; i++)
            {
                var planetSide = new TerrainQuadTree(
                    transform.position,
                    planetDiameter,
                    Directions[i],
                    3.0f,
                    planetSize,
                    chunkMaterial,
                    meshGenerator
                );
                planetSide.terrain.transform.parent = transform;
                planetSide.terrain.name = DirectionNames[i];
                planetSides[i] = planetSide;
            }

            planetSides[0].neighbors = new[] {planetSides[2], planetSides[4], planetSides[3], planetSides[5]};
            planetSides[1].neighbors = new[] {planetSides[3], planetSides[4], planetSides[2], planetSides[5]};
            planetSides[2].neighbors = new[] {planetSides[4], planetSides[0], planetSides[5], planetSides[1]};
            planetSides[3].neighbors = new[] {planetSides[5], planetSides[0], planetSides[4], planetSides[1]};
            planetSides[4].neighbors = new[] {planetSides[3], planetSides[0], planetSides[2], planetSides[1]};
            planetSides[5].neighbors = new[] {planetSides[2], planetSides[0], planetSides[3], planetSides[1]};
        }

        // Update is called once per frame
        public void Update()
        {
            for (var i=0; i<6; i++) 
            {
                planetSides[i].Update();
            }
        }

        public void LateUpdate() {
            meshGenerator.Consume();
        }

        public void RecomputeTerrain()
        {
            foreach (var planetSide in planetSides)
            {
                planetSide.RecomputeTerrain();
            }
        }

        public float Diameter()
        {
            return this.diameter;
        }
    }
}