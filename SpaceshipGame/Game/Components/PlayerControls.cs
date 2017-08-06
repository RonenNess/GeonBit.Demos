using Microsoft.Xna.Framework;
using GeonBit.ECS;
using GeonBit.ECS.Components.Physics;
using GeonBit.ECS.Components.Graphics;
using GeonBit.ECS.Components;
using System;

namespace SpaceshipGame
{
    /// <summary>
    /// Create the component to controls the player's ship.
    /// </summary>
    class PlayerControls : BaseComponent
    {
        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned ship controller.</returns>
        public override BaseComponent Clone()
        {
            return new PlayerControls();
        }

        // weapon cooldown, to control fire rate
        float _weaponCooldown = 0f;

        // time until we generate the next ammo point
        float _timeForNextAmmoRegen = 1f;

        // how much does it take to regenerate a single bullet
        static readonly float TimeToRegenerateAmmo = 2.0f;

        /// <summary>
        /// Create the ship controls.
        /// </summary>
        public PlayerControls() : base()
        {
        }

        // create random instance
        Random rand = new Random();

        /// <summary>
        /// Return if the player is currently dead.
        /// </summary>
        bool IsDead
        {
            get { return PlayerStatus.Hp <= 0; }
        }

        /// <summary>
        /// Damage the player.
        /// </summary>
        /// <param name="amount">By how much to damage the player.</param>
        public void Damage(int amount = 1)
        {
            // if player is already dead, skip
            if (IsDead)
            {
                return;
            }

            // decrease hp
            PlayerStatus.Hp -= amount;

            // check if player just died
            if (IsDead)
            {
                // make the ship model black and turn physics ethereal so the ship will fall out of the screen
                _GameObject.Find("model").GetComponent<ModelRenderer>().MaterialOverride.DiffuseColor = Color.Black;
                _GameObject.GetComponent<RigidBody>().IsEthereal = true;

                // stop music
                Managers.SoundManager.StopMusic();

                // add explosions effect
                GameObject deathExplosions = Managers.Prototypes.Spawn("explosions-set");
                deathExplosions.Parent = _GameObject;
            }
        }

        /// <summary>
        /// Do on frame update.
        /// </summary>
        protected override void OnUpdate()
        {
            // if dead - disable controls
            if (IsDead)
            {
                return;
            }

            // decrease weapon cooldown
            if (_weaponCooldown > 0f)
            {
                _weaponCooldown -= Managers.TimeManager.TimeFactor;
            }

            // if hit fire key, fire weapon
            if (Managers.GameInput.IsKeyDown(GeonBit.Input.GameKeys.Fire))
            {
                // make sure we have enough ammo and weapon is not currently cooling down.
                if (_weaponCooldown <= 0f && PlayerStatus.Ammo > 0)
                {
                    // set cooldown and decrease ammo
                    _weaponCooldown = 0.25f;
                    _timeForNextAmmoRegen = TimeToRegenerateAmmo;
                    PlayerStatus.Ammo--;

                    // spawn bullets from player's position
                    for (int i = -1; i <= 1; i += 2)
                    {
                        // create new bullet
                        GameObject bullet = Managers.Prototypes.Spawn("bullet");

                        // update position and copy transformations from node to physical body
                        bullet.SceneNode.Position = _GameObject.SceneNode.WorldPosition + Vector3.Left * 1.5f * i;
                        bullet.GetComponent<RigidBody>().CopyNodeWorldMatrix(true);
                        bullet.Parent = _GameObject.Parent;
                    }
                }
            }

            // get physical body
            RigidBody body = _GameObject.GetComponent<RigidBody>();

            // set movement speed factor
            float moveSpeed = 750f;

            // if currently moving ship, apply force in desired direction
            if (Managers.GameInput.MovementVector != Vector3.Zero)
            {
                body.ApplyForce(Managers.GameInput.MovementVector * moveSpeed);
            }

            // tilt spaceship left / right based on speed on X axis
            float tilt = body.LinearVelocity.X * 0.035f;
            if (tilt < -0.5f) tilt = -0.5f;
            if (tilt > 0.5f) tilt = 0.5f;
            _GameObject.Find("model", false).SceneNode.RotationZ = tilt;

            // Make sure player can't escape screen boundaries

            // get screen boundaries based on camera frustum
            BoundingFrustum frustum = Managers.ActiveScene.ActiveCamera.ViewFrustum;

            // get world position
            Vector3 worldPos = _GameObject.SceneNode.WorldPosition;

            // if try to escape from left side, apply force to bring the ship back to screen
            if (worldPos.X < frustum.Left.D)
            {
                body.ApplyForce(Vector3.Right * moveSpeed * 2f);
            }
            // if try to escape from right side, apply force to bring the ship back to screen
            if (worldPos.X > -frustum.Right.D)
            {
                body.ApplyForce(Vector3.Left * moveSpeed * 2f);
            }
            // if try to escape from top side, apply force to bring the ship back to screen
            if (worldPos.Z < frustum.Top.D)
            {
                body.ApplyForce(Vector3.Backward * moveSpeed * 2f);
            }
            // if try to escape from bottom side, apply force to bring the ship back to screen
            if (worldPos.Z > -frustum.Bottom.D)
            {
                body.ApplyForce(Vector3.Forward * moveSpeed * 2f);
            }

        }

        /// <summary>
        /// Called every heartbeat.
        /// </summary>
        protected override void OnHeartbeat()
        {
            // if dead, skip
            if (IsDead)
            {
                return;
            }

            // regenerate ammo
            if (PlayerStatus.Ammo < PlayerStatus.MaxAmmo)
            {
                _timeForNextAmmoRegen -= _GameObject.HeartbeatInterval;
                if (_timeForNextAmmoRegen <= 0f)
                {
                    _timeForNextAmmoRegen = TimeToRegenerateAmmo;
                    PlayerStatus.Ammo++;
                }
            }

            // increase score
            PlayerStatus.Score++;
        }
    }
}
