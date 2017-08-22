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
            InitParams.EnableVsync = true;
            InitParams.Title = "Skinned Models";
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

            /// TBD add any custom Update functionality here.
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // create the scene
            GameScene scene = new GameScene();

            // Added GPU-animated model
            {

                // create skinned mesh object
                GameObject modelObject = new GameObject("mech");
                SkinnedModelRenderer model = new SkinnedModelRenderer("game/mech/Mech");

                // set model materials color and disable lighting
                foreach (var material in model.GetMaterials())
                {
                    material.DiffuseColor = Color.Gray;
                    material.SpecularColor = Color.White;
                    material.SpecularPower = 50f;
                    material.AmbientLight = new Color(150, 180, 225);
                }

                // add to gameobject and set its position and rotation
                modelObject.AddComponent(model, "model");
                modelObject.SceneNode.Scale = Vector3.One * 0.1f;
                modelObject.SceneNode.PositionX = -45;
                modelObject.SceneNode.RotationY = (float)System.Math.PI;
                modelObject.Parent = scene.Root;

            }

            // Added CPU-animated model
            {

                // create skinned mesh object
                GameObject modelObject = new GameObject("wolf");
                SkinnedModelRenderer model = new SkinnedModelRenderer("game/wolf/Wolf");
                model.SetClip("Wolf_Skeleton|Wolf_Walk_cycle_", transitionTime: 0f);

                // set model materials color and disable lighting
                foreach (var material in model.GetMaterials())
                {
                    material.DiffuseColor = Color.White;
                    material.AmbientLight = Color.Gray;
                    material.Alpha = 1f;
                }

                // fix something in wolf's model (problem with one of the mesh effects)
                var furMesh = model.GetMesh("Wolf_obj_fur");
                furMesh.RenderingQueue = GeonBit.Core.Graphics.RenderingQueue.Opacity;
                var furMaterial = furMesh.GetMaterial(0);
                furMaterial.Alpha = 1f;
                furMaterial.DiffuseColor = Color.White;
                furMaterial.SpecularPower = 10f;
                furMaterial.SpecularColor = Color.Black;
                furMaterial.Texture = Resources.GetTexture("game/wolf/textures/Wolf_Fur_Alpha");

                // add to gameobject and set its position and rotation
                modelObject.AddComponent(model, "model");
                modelObject.SceneNode.Scale = Vector3.One * 1.5f;
                modelObject.SceneNode.PositionX = 45;
                modelObject.SceneNode.RotationY = (float)System.Math.PI;
                modelObject.Parent = scene.Root;

            }

            // create skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);

            // create camera
            GameObject camera = new GameObject();
            camera.AddComponent(new CameraEditorController());
            Camera cameraComponent = new Camera();
            camera.AddComponent(cameraComponent);
            camera.SceneNode.Position = new Vector3(0, 0.8f, -0.75f) * 250f;
            camera.SceneNode.RotationX = -2.5f;
            camera.Parent = scene.Root;

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
