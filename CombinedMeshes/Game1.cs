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
            cameraComponent.FarPlane = 10000f;
            camera.AddComponent(cameraComponent);
            camera.SceneNode.Position = -new Vector3(-10, 0, 0);
            cameraComponent.LookAt = new Vector3(0, 0, 0);
            camera.AddComponent(new CameraEditorController());
            camera.Parent = scene.Root;

            // create the combined meshes component
            CombinedMeshesRenderer combined = scene.Root.AddComponent(new CombinedMeshesRenderer()) as CombinedMeshesRenderer;

            // get model to add
            var model = Resources.GetModel("GeonBit.Core/BasicMeshes/Cube");

            // add lots of entities to the combined renderer
            int amount = 15;
            float shapeSize = 10f;
            for (int x = -amount; x <= amount; ++x)
            {
                for (int y = -amount; y <= amount; ++y)
                {
                    for (int z = -amount; z <= amount; ++z)
                    {
                        Matrix transform = Matrix.CreateTranslation(new Vector3(x, y, z) * shapeSize * 0.5f) * Matrix.CreateScale(shapeSize);
                        combined.AddModel(model, transform);
                    }
                }
            }

            // build the combined meshes
            combined.Build();

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
        }
    }
}
