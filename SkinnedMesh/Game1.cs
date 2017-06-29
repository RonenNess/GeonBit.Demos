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
            UiTheme = "hd";
            DebugMode = true;
            EnableVsync = true;
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

            // create skinned mesh object
            GameObject modelObject = new GameObject("soldier");
            SkinnedModelRenderer model = new SkinnedModelRenderer("game/mech/Mech");

            // set model materials color and disable lighting
            foreach (var material in model.GetMaterials())
            {
                material.LightingEnabled = false;
                material.DiffuseColor = Color.Gray;
                material.SpecularColor = Color.White;
                material.SpecularPower = 50f;
            }

            // add to gameobject and set its position and rotation
            modelObject.AddComponent(model, "model");
            modelObject.SceneNode.Scale = Vector3.One * 0.1f;
            modelObject.SceneNode.RotationY = (float)System.Math.PI;
            modelObject.Parent = scene.Root;

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

            // set scene
            GeonBitMain.Instance.Application.LoadScene(scene);
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
