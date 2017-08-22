using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Particles;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Sound;

namespace EmptyGeonBitProject
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
            InitParams.UiTheme = "hd";
            InitParams.DebugMode = true;
            InitParams.EnableVsync = false;
            InitParams.FullScreen = false;
            InitParams.Title = "Octree";
        }

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

            // toggle rendering octree debug data
            if (Managers.GameInput.IsKeyboardKeyReleased(GeonBit.Input.KeyboardKeys.Space))
            {
                Managers.Diagnostic.DebugRenderOctree = !Managers.Diagnostic.DebugRenderOctree;
            }
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // set scene size
            int sceneSize = 10000;

            // create the scene
            GameScene scene = new GameScene();
            GameObject.OctreeSceneBoundaries = new BoundingBox(Vector3.One * -sceneSize, Vector3.One * sceneSize);
            GameObject.OctreeMaxDivisions = 7;
            GameObject octree = new GameObject("octree", SceneNodeType.OctreeCulling);
            octree.Parent = scene.Root;

            // create skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);

            // default no culling node types
            GameObject.DefaultSceneNodeType = SceneNodeType.Simple;

            // create camera
            GameObject camera = new GameObject();
            Camera cameraComponent = new Camera();
            camera.AddComponent(cameraComponent);
            cameraComponent.LookAt = new Vector3(100, 2, 100);
            camera.SceneNode.Position = new Vector3(0, 100, 0);
            camera.AddComponent(new CameraEditorController());
            cameraComponent.FarPlane = 5000;
            camera.Parent = scene.Root;

            // create floor material 
            GeonBit.Core.Graphics.Materials.BasicMaterial tilesMaterial = new GeonBit.Core.Graphics.Materials.BasicMaterial();
            tilesMaterial.Texture = Resources.GetTexture("test/floor");
            tilesMaterial.TextureEnabled = true;
            tilesMaterial.SpecularColor = Color.Black;

            // for random positions
            System.Random rand = new System.Random();

            // create some starting tiles
            for (int i = 0; i < 80; ++i)
            {
                for (int j = 0; j < 80; ++j)
                {
                    GameObject obj = new GameObject();
                    ShapeRenderer tileModel = obj.AddComponent(new ShapeRenderer(ShapeMeshes.Cube)) as ShapeRenderer;
                    tileModel.SetMaterial(tilesMaterial);
                    obj.SceneNode.Scale = Vector3.One * 10;
                    obj.SceneNode.Position = new Vector3(
                        rand.Next(-sceneSize, sceneSize),
                        rand.Next(-sceneSize, sceneSize),
                        rand.Next(-sceneSize, sceneSize));
                    obj.Parent = octree;
                }
            }

            // add diagnostic data paragraph to scene
            var diagnosticData = new GeonBit.UI.Entities.Paragraph("", GeonBit.UI.Entities.Anchor.BottomLeft, offset: Vector2.One * 10f, scale: 0.7f);
            diagnosticData.BeforeDraw = (GeonBit.UI.Entities.Entity entity) =>
            {
                diagnosticData.Text = Managers.Diagnostic.GetReportString();
            };
            scene.UserInterface.AddEntity(diagnosticData);

            // set scene
            scene.Load();
        }

        /// <summary>
        /// Draw function to implement per main type.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        override public void Draw(GameTime gameTime)
        {
        }
    }
}
