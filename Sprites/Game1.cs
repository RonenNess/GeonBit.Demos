using Microsoft.Xna.Framework;
using GeonBit;
using GeonBit.ECS;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components.Physics;
using GeonBit.Core.Graphics;
using GeonBit.Core.Graphics.Materials;

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
            UiTheme = "editor";
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
            // make fullscreen but with frame
            //MakeFullscreen(true);

            // create the scene
            GameScene scene = new GameScene();

            // create skybox
            Managers.GraphicsManager.CreateSkybox(null, scene.Root);

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
                grassObj.AddComponent(new BillboardRenderer("game/grass"));
                grassObj.Parent = scene.Root;
            }

            // create a tree
            GameObject treeObj = new GameObject("tree");
            treeObj.SceneNode.Scale = Vector3.One * 30;
            treeObj.SceneNode.Position = new Vector3(50, 15f, 75);
            treeObj.AddComponent(new BillboardRenderer("game/tree"));
            treeObj.Parent = scene.Root;

            // create a spritesheet with animations (8 steps on X axis, 4 directions on Y axis)
            SpriteSheet sp = new SpriteSheet(new Point(8, 4));

            // add character sprite
            GameObject spriteObj = new GameObject("player");
            spriteObj.SceneNode.Scale = Vector3.One * 10;
            spriteObj.SceneNode.Position = new Vector3(50, 5, 50);
            SpriteRenderer sprite = spriteObj.AddComponent(new SpriteRenderer(sp, "game/rpg_sprite_walk")) as SpriteRenderer;
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
