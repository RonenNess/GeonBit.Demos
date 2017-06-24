using Microsoft.Xna.Framework;
using GeonBit.ECS.Components;
using GeonBit.ECS.Components.Graphics;


namespace SpaceshipGame
{
    /// <summary>
    /// A script component to animate the scene background.
    /// </summary>
    class BackgroundAnimator : BaseComponent
    {
        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned animator.</returns>
        public override BaseComponent Clone()
        {
            return new BackgroundAnimator();
        }

        // current background position
        float pos = 0f;

        // increase background movement speed over time
        float _speedFac = 0f;

        /// <summary>
        /// Do on frame update.
        /// </summary>
        protected override void OnUpdate()
        {
            // move background position
            pos -= Managers.TimeManager.TimeFactor * (25f + _speedFac);
            _speedFac += Managers.TimeManager.TimeFactor;

            // take player X position to add movement effect on X axis for background
            float posX = 0f; // Managers.ActiveScene.Root.Find("player").SceneNode.WorldPosition.X;

            // update background
            _GameObject.GetComponent<SceneBackground>().TileOffset = new Point((int)posX, (int)pos);
        }
    }
}
