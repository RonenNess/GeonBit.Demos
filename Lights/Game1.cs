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
        int tilemapSize = 10;

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
        }

        // game object that hold lights
        GameObject lightsCenter;

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
            LitMaterial tilesMaterial = new LitMaterial();
            tilesMaterial.Texture = Resources.GetTexture("game/floor");
            tilesMaterial.TextureEnabled = true;
            tilesMaterial.SpecularColor = Color.Black;

            // set ambient light
            scene.Lights.AmbientLight = new Color(20, 20, 20, 255);

            // add robot model
            GameObject robot = new GameObject();
            var robotRenderer = robot.AddComponent(new ModelRenderer("Game/robot")) as ModelRenderer;
            var robotMat = new LitMaterial();
            robotMat.Texture = Resources.GetTexture("Game/robottexture_0");
            robotMat.DiffuseColor = Color.White;
            robotRenderer.SetMaterial(robotMat);
            robot.SceneNode.Scale = Vector3.One * tileSize * 0.75f;
            robot.SceneNode.Position = new Vector3(tilemapSize * tileSize * 0.5f, 10f, tilemapSize * tileSize * 0.5f);
            robot.Parent = scene.Root;

            // attach light to camera
            {
                var lightComponent = camera.AddComponent(new Light()) as Light;
                lightComponent.Intensity = 2f;
                lightComponent.Range = tileSize * 15f;
                lightComponent.Color = Color.Green;
            }

            // add directional light
            {
                var lightComponent = scene.Root.AddComponent(new Light()) as Light;
                lightComponent.Intensity = 0.25f;
                lightComponent.Range = 0f;
                lightComponent.Direction = Vector3.Normalize(new Vector3(0.2f, 1f, 0.1f));
                lightComponent.Color = Color.White;
            }

            // attach lights
            lightsCenter = new GameObject();
            lightsCenter.Name = "LightsCenter";
            lightsCenter.SceneNode.Position = robot.SceneNode.Position;
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject lightGo = new GameObject();
                lightGo.Name = "Light" + i.ToString();
                lightGo.SceneNode.PositionX = i * tileSize * 3f;
                var lightComponent = lightGo.AddComponent(new Light()) as Light;
                lightComponent.Intensity = 3;
                lightComponent.Range = tileSize * 15.5f;
                lightComponent.Color = i == -1 ? Color.Red : Color.Blue;
                lightGo.Parent = lightsCenter;
            }
            lightsCenter.Parent = scene.Root;

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
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
            // rotate lights
            lightsCenter.SceneNode.RotationY += 0.01f;
        }
    }
}
