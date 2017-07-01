using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Particles;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Sound;
using GeonBit.Core.Graphics.Materials;
using GeonBit.Core.Graphics;

namespace EmptyGeonBitProject
{
    /// <summary>
    /// Your main game class!
    /// </summary>
    internal class Game1 : GeonBitGame
    {
        // camera object
        GameObject camera;

        /// <summary>
        /// Initialize your GeonBitGame properties here.
        /// </summary>
        public Game1()
        {
            InitParams.UiTheme = "hd";
            InitParams.DebugMode = true;
            InitParams.EnableVsync = true;
            InitParams.Title = "Isometric Game";
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

            // move camera
            camera.SceneNode.PositionX += Managers.GameInput.MovementVector.X;
            camera.SceneNode.PositionZ += Managers.GameInput.MovementVector.Z;
        }

        /// <summary>
        /// Initialize to implement per main type.
        /// </summary>
        override public void Initialize()
        {
            // create the scene
            GameScene scene = new GameScene();

            // create camera object
            camera = new GameObject();
            Camera cameraComponent = new Camera();
            cameraComponent.CameraType = CameraType.Orthographic;
            Point screenSize = Managers.GraphicsManager.ViewportSize;
            float ratio = (float)screenSize.X / (float)screenSize.Y;
            cameraComponent.ForceScreenSize = new Point((int)(50f * ratio), (int)(25f * ratio));
            camera.AddComponent(cameraComponent);
            camera.SceneNode.Position = new Vector3(50, 100, 150);
            camera.SceneNode.RotationX = -(float)System.Math.PI * 0.25f;
            camera.Parent = scene.Root;

            // create a tilemap fsor the floor
            TileMap tilemap = scene.Root.AddComponent(new TileMap(Vector2.One * 10f, 10)) as TileMap;

            // create floor material 
            BasicMaterial tilesMaterial = new BasicMaterial();
            tilesMaterial.Texture = Resources.GetTexture("game/floor");
            tilesMaterial.TextureEnabled = true;
            tilesMaterial.SpecularColor = Color.Black;

            // create some floor tiles
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    GameObject tile = tilemap.GetTile(new Point(i, j));
                    ShapeRenderer tileModel = tile.AddComponent(new ShapeRenderer(ShapeMeshes.Plane)) as ShapeRenderer;
                    tileModel.SetMaterial(tilesMaterial);
                    tile.SceneNode.Scale = Vector3.One * 5f;
                    tile.SceneNode.RotationX = (float)System.Math.PI * -0.5f;
                }
            }

            // create random grass sprites
            System.Random rand = new System.Random();
            for (int i = 0; i < 100; ++i)
            {
                GameObject grassObj = new GameObject("grass");
                grassObj.SceneNode.Scale = Vector3.One * 5;
                grassObj.SceneNode.Position = new Vector3((float)rand.NextDouble() * 90f, 2.5f, (float)rand.NextDouble() * 90f);
                grassObj.AddComponent(new BillboardRenderer("game/grass", faceCamera: false));
                grassObj.Parent = scene.Root;
            }

            // create a tree
            GameObject treeObj = new GameObject("tree");
            treeObj.SceneNode.Scale = Vector3.One * 30;
            treeObj.SceneNode.Position = new Vector3(50, 15f, 25);
            treeObj.AddComponent(new BillboardRenderer("game/tree", faceCamera: false));
            treeObj.Parent = scene.Root;

            // create a spritesheet with animations (8 steps on X axis, 4 directions on Y axis)
            SpriteSheet sp = new SpriteSheet(new Point(8, 4));

            // add character sprite
            GameObject spriteObj = new GameObject("player");
            spriteObj.SceneNode.Scale = Vector3.One * 10;
            spriteObj.SceneNode.Position = new Vector3(50, 5, 50);
            spriteObj.AddComponent(new SpriteRenderer(sp, "game/rpg_sprite_walk", faceCamera: false));
            spriteObj.Parent = scene.Root;

            // define walking animation and play on character
            SpriteAnimationClip animationClip = new SpriteAnimationClip(startStep: 0, endStep: 7, speed: 10f, loop: true);
            spriteObj.GetComponent<SpriteRenderer>().PlayAnimation(animationClip);

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
