using System.Collections.Generic;

using BulletSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShooterEngine.Debug;

namespace ShooterEngine
{
    class Physics
    {
        public CollisionConfiguration CollisionConf { get; set; }
        public CollisionDispatcher Dispatcher { get; set; }
        public BroadphaseInterface Broadphase { get; set; }
        public DynamicsWorld World { get; set; }
        public ConstraintSolver ConstraintSolver { get; set; }

        private PhysicsDebugDrawer debugDrawer;
        private BasicEffect debugEffect;
        private GraphicsDevice graphicsDevice;

        public Physics(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Initialize()
        {
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);
            Broadphase = new AxisSweep3(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000));
            Broadphase.OverlappingPairCache.SetInternalGhostPairCallback(new GhostPairCallback());
            ConstraintSolver = new SequentialImpulseConstraintSolver();

            debugDrawer = new PhysicsDebugDrawer(graphicsDevice);
            debugDrawer.DebugMode = DebugDrawModes.DrawAabb;

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, ConstraintSolver, CollisionConf);
            World.DispatchInfo.AllowedCcdPenetration = 0.0001f;
            World.Gravity = new Vector3(0, -10, 0);
            World.DebugDrawer = debugDrawer;

            debugEffect = new BasicEffect(graphicsDevice);
        }

        public void DrawDebug(Camera camera)
        {
            foreach (EffectPass pass in debugEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            debugEffect.View = camera.View;
            debugEffect.Projection = camera.Projection;

            debugDrawer.DrawDebugWorld(World);
        }

        public void LoadWorldCollisions(List<Mesh> meshes)
        {
            foreach (Mesh mesh in meshes)
            {
                List<Vector3> vertices = mesh.GetVertices();

                for (int i = 0; i < vertices.Count; i += 3)
                {
                    CollisionShape collisionShape = new TriangleShape(vertices[i], vertices[i + 1], vertices[i + 2]);

                    LocalCreateRigidBody(0, Matrix.Identity, collisionShape);
                }
            }
        }

        public RigidBody LocalCreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            using (var rbInfo = new RigidBodyConstructionInfo(mass, null, shape))
            {
                var body = new RigidBody(rbInfo);
                World.AddRigidBody(body);
                return body;
            }
        }

        public void ExitPhysics()
        {
            for (int i = World.NumConstraints - 1; i >= 0; i--)
            {
                var constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            for (int i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                var obj = World.CollisionObjectArray[i];
                var body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            World.Dispose();
            Broadphase.Dispose();
            if (Dispatcher != null)
            {
                Dispatcher.Dispose();
            }
            CollisionConf.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            World.StepSimulation((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
