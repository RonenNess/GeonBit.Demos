using Microsoft.Xna.Framework;
using GeonBit.ECS;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Misc;
using GeonBit.ECS.Components;
using System;

namespace SpaceshipGame
{
    /// <summary>
    /// Create the component to controls the asteroids.
    /// </summary>
    class AsteroidControls : BaseComponent
    {
        // create random instance
        Random rand = new Random();

        // asteroids challange factor, raises over time
        static float ChallangeLevel = 0f;

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned ship controller.</returns>
        public override BaseComponent Clone()
        {
            AsteroidControls ret = new AsteroidControls();
            ret._scaleFactor = _scaleFactor;
            ret._speedFactor = _speedFactor;
            ret._maxHp = _maxHp;
            ret._hp = _hp;
            return ret;
        }

        /// <summary>
        /// Create the asteroid controls.
        /// </summary>
        public AsteroidControls() : base()
        {
        }

        // asteroid scale and speed factor
        float _scaleFactor = 0f;
        float _speedFactor = 0f;

        // asteroid hp
        int _hp = 0;
        int _maxHp = 0;

        // if true, will skip on-spawn init
        bool _alreadyInit = false;

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // don't init if already init
            if (_alreadyInit)
            {
                return;
            }

            // increase challenge level
            ChallangeLevel += 0.025f;

            // get physical body
            PhysicalBody body = _GameObject.GetComponent<PhysicalBody>();

            // random size
            _scaleFactor = 1f + (float)rand.NextDouble() * (1.5f + ChallangeLevel);
            body.Scale = Vector3.One * _scaleFactor;

            // set hp based on scale
            _hp = (int)(_scaleFactor * 3f);
            _maxHp = _hp;

            // set speed factor
            _speedFactor = 1.35f + (float)rand.NextDouble() * ChallangeLevel;

            // random rotation speed
            Vector3 angular = new Vector3(
                (float)rand.NextDouble() - 0.5f,
                (float)rand.NextDouble() - 0.5f,
                (float)rand.NextDouble() - 0.5f);
            angular.Normalize();
            angular *= _speedFactor;
            body.ConstAngularVelocity = angular;

            // random movement speed
            body.ConstVelocity = Vector3.UnitZ * _speedFactor * 2.5f;

            // random position
            body.Position = new Vector3(((float)rand.NextDouble() - 0.5f) * 40f, 0, -15f - _scaleFactor);

        }

        /// <summary>
        /// Split this asteroid into two, or destroy it if too small.
        /// </summary>
        private void SplitMeteor()
        {
            // get base size
            PhysicalBody body = _GameObject.GetComponent<PhysicalBody>();
            float currSize = body.Scale.X;

            // create explosion effect
            GameObject explosion = Managers.Prototypes.Spawn("explosion");
            explosion.SceneNode.Position = _GameObject.SceneNode.WorldPosition;
            explosion.Parent = _GameObject.Parent;

            // too small? destroy
            if (currSize < 1.65f)
            {
                _GameObject.Destroy();
                return;
            }

            // split the meteor
            for (int i = -1; i <= 1; i += 2)
            {
                // create the meteor split
                GameObject split = _GameObject.Clone();

                // override some meteor controls properties before it spawns
                AsteroidControls splitControls = split.GetComponent<AsteroidControls>();
                PhysicalBody splitBody = split.GetComponent<PhysicalBody>();

                // set clonsed hp and scale factor
                splitControls._hp = _maxHp / 2;
                splitControls._maxHp = _hp;

                // set scale and velocity
                splitBody.Scale = body.Scale * 0.5f;
                splitBody.ConstVelocity = body.ConstVelocity.Value + new Vector3(i * 2f, 0, 0);

                splitControls._alreadyInit = true;

                // add to scene (this will invoke spawn event)
                split.Parent = _GameObject.Parent;
            }

            // destroy self
            _GameObject.Destroy();
        }

        /// <summary>
        /// Called when the parent Game Object start colliding with another object.
        /// </summary>
        /// <param name="other">The other object we collide with.</param>
        /// <param name="data">Collision data.</param>
        protected override void OnCollisionStart(GameObject other, GeonBit.Core.Physics.CollisionData data)
        {
            // if colliding player ship, split meteor
            if (other.Name == "player")
            {
                // apply force to push the ship further back
                float currSize = _GameObject.GetComponent<PhysicalBody>().Scale.X;
                float force = Math.Min(_speedFactor * currSize, 6.5f) * 3000f;
                Vector3 direction = other.SceneNode.WorldPosition - _GameObject.SceneNode.WorldPosition;
                direction.Normalize();
                other.GetComponent<PhysicalBody>().ApplyForce(direction * force);

                // create explosion effect
                GameObject explosion = Managers.Prototypes.Spawn("explosion");
                explosion.SceneNode.Position = data.Position + direction;
                explosion.Parent = _GameObject.Parent;

                // damage meteor
                Damage(1);

                // damage ship
                other.GetComponent<PlayerControls>().Damage();
            }
            // if colliding with a bullet
            else if (other.Name == "bullet")
            {
                // create explosion effect
                GameObject explosion = Managers.Prototypes.Spawn("explosion");
                explosion.SceneNode.Position = other.SceneNode.WorldPosition;
                explosion.Parent = _GameObject.Parent;

                // destroy the bullet and damage meteor
                other.Destroy();
                Damage(2);
            }
        }

        /// <summary>
        /// Damage this meteor, split it if out of life.
        /// </summary>
        /// <param name="amount">How much to damage the meteor.</param>
        protected void Damage(int amount = 1)
        {
            // damage
            _hp -= amount;

            // if out of life, split meteor
            if (_hp <= 0)
            {
                SplitMeteor();
            }
        }

        /// <summary>
        /// On heartbeat events
        /// </summary>
        protected override void OnHeartbeat()
        {
            // if out of screen, destroy self
            if (_GameObject.GetComponent<PhysicalBody>().Position.Z > 15f + _scaleFactor)
            {
                _GameObject.Destroy();
            }
        }
    }
}
