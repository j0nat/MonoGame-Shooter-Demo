using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BulletSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShooterEngine
{
    class CharacterController
    {
        private Physics physics;
        private PairCachingGhostObject ghostObject;
        private CapsuleShape capsuleShape;
        private KinematicCharacterController character;

        private Vector3 eye;
        private Vector3 target;
        private Vector3 up;
        private Vector3 mouseVector;

        private MouseState previousMouseState;
        private Vector3 mouseRotationBuffer;

        private Camera camera;
        private WorldRenderer worldRenderer;
        private GraphicsDevice graphicsDevice;

        public CharacterController(GraphicsDevice graphicsDevice, WorldRenderer worldRenderer, Physics physics, Camera camera)
        {
            this.physics = physics;
            this.eye = new Vector3(30, 20, 10);
            this.target = Vector3.UnitX;
            this.up = Vector3.UnitY;
            this.mouseVector = Vector3.Zero;
            this.mouseRotationBuffer = Vector3.Zero;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            this.worldRenderer = worldRenderer;
        }

        public void Initialize()
        {
            const float characterHeight = 1.75f;
            const float characterWidth = 1.75f;
            capsuleShape = new CapsuleShape(characterWidth, characterHeight);
            ghostObject = new PairCachingGhostObject()
            {
                CollisionShape = capsuleShape,
                CollisionFlags = CollisionFlags.CharacterObject,
                WorldTransform = Matrix.CreateTranslation(-2, 15, 0)
            };
            physics.World.AddCollisionObject(ghostObject, CollisionFilterGroups.CharacterFilter, CollisionFilterGroups.StaticFilter | CollisionFilterGroups.DefaultFilter);

            const float stepHeight = 0.35f;
            character = new KinematicCharacterController(ghostObject, capsuleShape, stepHeight);
            physics.World.AddAction(character);
        }

        public void Update(GameTime gameTime)
        {
            float FrameDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Matrix transform = ghostObject.WorldTransform;

            Vector3 position = transform.Translation;

            Vector3 forwardDir = new Vector3(transform.M31, transform.M32, transform.M33);
            forwardDir.Normalize();

            Vector3 upDir = new Vector3(transform.M21, transform.M22, transform.M23);
            upDir.Normalize();

            KeyboardState ks = Keyboard.GetState();
            const float walkVelocity = 1.1f * 4.0f;
            float walkSpeed = walkVelocity * FrameDelta * 5;// * 0.0001f;
            Vector3 walkDirection = Vector3.Zero;

            if (ks.IsKeyDown(Keys.A))
            {
                walkDirection -= Vector3.Cross(GetXZDirection(), Vector3.UnitY);
            }
            if (ks.IsKeyDown(Keys.D))
            {
                walkDirection += Vector3.Cross(GetXZDirection(), Vector3.UnitY);
            }

            if (ks.IsKeyDown(Keys.W))
            {
                walkDirection += GetXZDirection();
            }
            if (ks.IsKeyDown(Keys.S))
            {
                walkDirection -= GetXZDirection();
            }

            target = position;
            eye = position - forwardDir * 1 + upDir * 1;

            SetEyeTarget(ref eye, ref target);

            MouseUpdate(gameTime);

            Vector3 direction = GetXYZDirection();

            target = eye + (eye - target).Length() * direction;

            Matrix view = Matrix.CreateLookAt(eye, target, Vector3.UnitY);
            camera.View = view;

            character.SetWalkDirection(walkDirection * walkSpeed);

            if (ks.IsKeyDown(Keys.Space) && (character.OnGround || ShooterEngineGame.DEBUG))
            {
                character.Jump();
            }
        }

        public void SetEyeTarget(ref Vector3 eye, ref Vector3 target)
        {
            this.eye = eye;
            this.target = target;

            Matrix swapAxis = Matrix.CreateFromAxisAngle(Vector3.Cross(up, Vector3.UnitY), Angle(up, Vector3.UnitY));
            mouseVector = Vector3.Transform(Vector3.Normalize(eye - target), swapAxis);
        }

        private void MouseUpdate(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                // Out of service...
                // worldRenderer.ShootCube(target, GetCameraDirection());
            }

            float deltaX;
            float deltaY;
            if (ms != previousMouseState)
            {
                deltaX = ms.X - (graphicsDevice.Viewport.Width / 2);
                deltaY = ms.Y - (graphicsDevice.Viewport.Height / 2);

                if (Math.Abs(deltaX) > 3 || Math.Abs(deltaY) > 3)
                {
                    mouseRotationBuffer.X -= 0.1f * deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    mouseRotationBuffer.Y -= 0.1f * deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    {
                        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                    }

                    if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    {
                        mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));
                    }

                    Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
                }
            }

            previousMouseState = ms;

            SetByAngles(-mouseRotationBuffer.X, mouseRotationBuffer.Y);
        }

        private Vector3 GetCameraDirection()
        {
            Vector2 posCursor = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            Vector3 nearSource = new Vector3(posCursor, 0.0f);
            Vector3 farSource = new Vector3(posCursor, 1.0f);

            Vector3 nearPoint = graphicsDevice.Viewport.Unproject(nearSource, camera.Projection, camera.View, Matrix.Identity);
            Vector3 farPoint = graphicsDevice.Viewport.Unproject(farSource, camera.Projection, camera.View, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;

            return direction;
        }

        private Vector3 GetXZDirection()
        {
            Matrix swapAxis = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.UnitY, up), Angle(Vector3.UnitY, up));
            Vector3 direction = Vector3.Transform(mouseVector, swapAxis);
            direction.Y = 0;

            return direction;
        }

        private Vector3 GetXYZDirection()
        {
            Matrix swapAxis = Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.UnitY, up), Angle(Vector3.UnitY, up));
            Vector3 direction = Vector3.Transform(mouseVector, swapAxis);

            return direction;
        }

        float Angle(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Acos(Vector3.Dot(v1, v2));
        }

        public void SetByAngles(double horizontalAngle, double verticalAngle)
        {
            mouseVector = new Vector3(
                (float)(Math.Cos(horizontalAngle) * Math.Cos(verticalAngle)),
                (float)Math.Sin(verticalAngle),
                (float)(Math.Sin(horizontalAngle) * Math.Cos(verticalAngle)));
        }
    }
}
