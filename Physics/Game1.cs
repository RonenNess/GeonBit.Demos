using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Physics;
using GeonBit.Core.Graphics;
using GeonBit.Core.Graphics.Materials;
using GeonBit.Core.Physics.CollisionShapes;

namespace Sprite
{
    /// <summary>
    /// Your main game class!
    /// </summary>
    internal class Game1 : GeonBitGame
    {
        /// <summary>
        /// Initialize your GeonBitGame properties here.
        /// </summary>
        public Game1()
        {
            InitParams.UiTheme = "editor";
            InitParams.DebugMode = true;
            InitParams.EnableVsync = true;
            InitParams.Title = "Sprites";
        }

        // size of the tilemap
        int tilemapSize = 25;

        // size of a single tile
        float tileSize = 10f;

        // create random object
        System.Random rand = new System.Random();

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Update(GameTime gameTime)
        {
            /// exit application on escape
            if (Managers.GameInput.IsKeyDown(GeonBit.Input.GameKeys.Escape))
            {
                Exit();
            }

            // on mouse click throw shapes
            if (Managers.GameInput.MouseButtonReleased(GeonBit.Input.MouseButton.Left))
            {
                GameObject obj = CreateRandomShape();
                var body = obj.GetComponent<RigidBody>();
                body.Position = ActiveCamera._GameObject.SceneNode.WorldPosition;
                body.ApplyImpulse(Vector3.Normalize(ActiveCamera.GetMouseRay().Direction) * 50f);
                obj.Parent = ActiveScene.Root;
            }
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // create the scene
            GameScene scene = new GameScene();

            // create skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);
            Managers.Diagnostic.DebugRenderPhysics = false;

            // create camera object
            GameObject camera = new GameObject();
            Camera cameraComponent = new Camera();
            camera.AddComponent(cameraComponent);
            cameraComponent.LookAt = new Vector3(100, 2, 100);
            camera.SceneNode.Position = new Vector3(0, 5, 0);
            camera.AddComponent(new CameraEditorController());
            camera.Parent = scene.Root;

            // create a tilemap for the floor
            TileMap tilemap = scene.Root.AddComponent(new TileMap(Vector2.One * 10f, 10)) as TileMap;

            // create floor material 
            BasicMaterial tilesMaterial = new BasicMaterial();
            tilesMaterial.Texture = Resources.GetTexture("game/floor");
            tilesMaterial.TextureEnabled = true;
            tilesMaterial.SpecularColor = Color.Black;

            // create some floor tiles
            for (int i = 0; i < tilemapSize; ++i)
            {
                for (int j = 0; j < tilemapSize; ++j)
                {
                    GameObject tile = tilemap.GetTile(new Point(i, j));
                    ShapeRenderer tileModel = tile.AddComponent(new ShapeRenderer(ShapeMeshes.Plane)) as ShapeRenderer;
                    tileModel.SetMaterial(tilesMaterial);
                    tile.SceneNode.Scale = Vector3.One * tileSize * 0.5f;
                    tile.SceneNode.RotationX = (float)System.Math.PI * -0.5f;
                }
            }

            // add floor physical body
            GameObject floorPhysics = new GameObject();
            float wholePlaneSize = tilemapSize * tileSize;
            StaticBody floorBody = floorPhysics.AddComponent(new StaticBody(new CollisionBox(wholePlaneSize, 5f, wholePlaneSize))) as StaticBody;
            floorBody.Restitution = 0.75f;
            floorPhysics.SceneNode.Position = new Vector3((wholePlaneSize - tileSize) * 0.5f, -2.5f, (wholePlaneSize - tileSize) * 0.5f);
            floorPhysics.Parent = scene.Root;

            // create some random physical objects
            for (int i = 0; i < 100; ++i)
            {
                GameObject obj = CreateRandomShape();
                obj.Parent = scene.Root;
            }

            // add diagnostic data paragraph to scene
            var diagnosticData = new GeonBit.UI.Entities.Paragraph("", GeonBit.UI.Entities.Anchor.BottomLeft, offset: Vector2.One * 10f, scale: 0.7f);
            diagnosticData.BeforeDraw = (GeonBit.UI.Entities.Entity entity) =>
            {
                diagnosticData.Text = Managers.Diagnostic.GetReportString();
            };
            scene.UserInterface.AddEntity(diagnosticData);

            // set scene
            GeonBitMain.Instance.Application.LoadScene(scene);
        }

        /// <summary>
        /// Create and return a random shape object.
        /// </summary>
        private GameObject CreateRandomShape()
        {
            // tilemap actual size
            float tilemapActualSize = tilemapSize * tileSize;

            // collision shape and type
            ICollisionShape collisionShape = null;
            ShapeMeshes shapeType = ShapeMeshes.Cylinder;

            // random collision shape and type
            int randType = rand.Next(0, 2);
            switch (randType)
            {
                case 0:
                    shapeType = ShapeMeshes.SphereSmooth;
                    collisionShape = new CollisionSphere();
                    break;

                case 1:
                    shapeType = ShapeMeshes.Cube;
                    collisionShape = new CollisionBox(2f, 2f, 2f);
                    break;
            }

            // create shape
            float sizeFactor = 1.0f + (float)rand.NextDouble() * 1.25f;
            GameObject shape = new GameObject();
            var shapeRenderer = shape.AddComponent(new ShapeRenderer(shapeType)) as ShapeRenderer;
            shapeRenderer.MaterialOverride.DiffuseColor = new Color((rand.Next(0, 255)), (rand.Next(0, 255)), (rand.Next(0, 255)));
            var body = shape.AddComponent(new RigidBody(collisionShape, sizeFactor, 1f, 1f)) as RigidBody;
            body.Scale = Vector3.One * tileSize * 0.25f * sizeFactor;
            body.Restitution = 0.5f;
            body.Position = new Vector3(
                (float)rand.NextDouble() * tilemapActualSize, 
                10f + (float)rand.NextDouble() * 45f, 
                (float)rand.NextDouble() * tilemapActualSize);
            return shape;
        }

        /// <summary>
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
            /// TBD add any custom drawing functionality here.
            /// Note: since GeonBit handle its own drawing internally, usually you don't need to do anything here.
        }
    }
}
