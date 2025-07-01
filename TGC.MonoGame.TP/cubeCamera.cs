using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{
    /// <summary>
    /// A camera specialized for rendering into a cubemap: square FOV, one face at a time.
    /// </summary>
    public class CubeMapCamera
    {
        /// <summary>Position of the camera (center of the cubemap).</summary>
        public Vector3 Position { get; set; }
        
        /// <summary>View matrix (updated per face).</summary>
        public Matrix View { get; private set; }
        
        /// <summary>Projection matrix (90° FOV, 1:1 aspect).</summary>
        public Matrix Projection { get; private set; }

        /// <summary>
        /// Creates a CubeMapCamera at given position.
        /// </summary>
        /// <param name="position">Center point of the cubemap.</param>
        /// <param name="nearPlane">Near clipping plane distance.</param>
        /// <param name="farPlane">Far clipping plane distance.</param>
        public CubeMapCamera(Vector3 position, float nearPlane, float farPlane)
        {
            Position = position;
            // 90° FOV and square aspect for each face
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2, // 90 degrees
                1f,                  // aspect ratio 1:1
                nearPlane,
                farPlane);
        }

        /// <summary>
        /// Orient the camera to look along one cubemap face, then rebuild the View matrix.
        /// </summary>
        public void SetFace(CubeMapFace face)
        {
            Vector3 target, up;
            switch (face)
            {
                case CubeMapFace.PositiveX:
                    target = Position + Vector3.Right;
                    up     = Vector3.Down;
                    break;
                case CubeMapFace.NegativeX:
                    target = Position + Vector3.Left;
                    up     = Vector3.Down;
                    break;
                case CubeMapFace.PositiveY:
                    target = Position + Vector3.Up;
                    up     = Vector3.Forward;
                    break;
                case CubeMapFace.NegativeY:
                    target = Position + Vector3.Down;
                    up     = Vector3.Backward;
                    break;
                case CubeMapFace.PositiveZ:
                    target = Position + Vector3.Forward;
                    up     = Vector3.Down;
                    break;
                case CubeMapFace.NegativeZ:
                    target = Position + Vector3.Backward;
                    up     = Vector3.Down;
                    break;
                default:
                    // fallback
                    target = Position + Vector3.Forward;
                    up     = Vector3.Up;
                    break;
            }

            View = Matrix.CreateLookAt(Position, target, up);
        }
    }
}
