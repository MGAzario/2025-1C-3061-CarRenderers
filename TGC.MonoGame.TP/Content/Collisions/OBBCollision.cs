using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.TP.Content.Collisions
{

    public class OBB
    {
        
        public Vector3 Center { get; set; }
        public Vector3 Extents { get; set; }
        public Matrix Orientation { get; set;}
        
        
        public OBB(){}

        public OBB(Vector3 center, Vector3 extents)
        {
            Center = center;
            Extents = extents;
            Orientation = Matrix.Identity;
        }


        public void Rotate(Matrix roationMatrix)
        {
            Orientation += roationMatrix;
        }

        public void quaternionRotation(Quaternion quaternion)
        {
            Rotate(Matrix.CreateFromQuaternion(quaternion));
        }
        
        
        public static Vector3 GetExtents(BoundingBox box)
        {
            var max = box.Max;
            var min = box.Min;

            return (max - min) * 0.5f;            
        }
        
        public static Vector3 GetCenter(BoundingBox box)
        {
            return (box.Max + box.Min) * 0.5f;
        }

      
        public static float GetVolume(BoundingBox box)
        {
            var difference = box.Max - box.Min;
            return difference.X * difference.Y * difference.Z;
        }
        
        public static BoundingBox Scale(BoundingBox box, float scale)
        {
            var center = GetCenter(box);
            var extents = GetExtents(box);
            var scaledExtents = extents * scale;

            return new BoundingBox(center - scaledExtents, center + scaledExtents);
        }
        
        private float[] ToArray(Vector3 vector)
        {
            return new[] { vector.X, vector.Y, vector.Z };
        }

        
        private float[] ToFloatArray(Matrix matrix)
        {
            return new[]
            {
                matrix.M11, matrix.M21, matrix.M31,
                matrix.M12, matrix.M22, matrix.M32,
                matrix.M13, matrix.M23, matrix.M33,
            };
        }
        
        
        
        public PlaneIntersectionType Intersects(Plane plane)// OBB vs Plane
        {
            // Maximum extent in direction of plane normal 
            var normal = Vector3.Transform(plane.Normal, Orientation);

            // Maximum extent in direction of plane normal 
            var r = MathF.Abs(Extents.X * normal.X)
                    + MathF.Abs(Extents.Y * normal.Y)
                    + MathF.Abs(Extents.Z * normal.Z);

            // signed distance between box center and plane
            var d = Vector3.Dot(plane.Normal, Center) + plane.D;


            // Return signed distance
            if (MathF.Abs(d) < r)
                return PlaneIntersectionType.Intersecting;
            else if (d < 0.0f)
                return PlaneIntersectionType.Front;
            else
                return PlaneIntersectionType.Back;
        }
        
        public static OBB ComputeFromPoints(Vector3[] points)
        {
            return ComputeFromPointsRecursive(points, Vector3.Zero, new Vector3(360, 360, 360), 10f);
        }


        private static OBB ComputeFromPointsRecursive(Vector3[] points, Vector3 initValues, Vector3 endValues,
            float step)
        {
            var minObb = new OBB();
            var minimumVolume = float.MaxValue;
            var minInitValues = Vector3.Zero;
            var minEndValues = Vector3.Zero;
            var transformedPoints = new Vector3[points.Length];
            float y, z;

            var x = initValues.X;
            while (x <= endValues.X)
            {
                y = initValues.Y;
                var rotationX = MathHelper.ToRadians(x);
                while (y <= endValues.Y)
                {
                    z = initValues.Z;
                    var rotationY = MathHelper.ToRadians(y);
                    while (z <= endValues.Z)
                    {
            
                        var rotationZ = MathHelper.ToRadians(z);
                        var rotationMatrix = Matrix.CreateFromYawPitchRoll(rotationY, rotationX, rotationZ);
                        
                        for (var index = 0; index < transformedPoints.Length; index++)
                            transformedPoints[index] = Vector3.Transform(points[index], rotationMatrix);
         
                        var aabb = BoundingBox.CreateFromPoints(transformedPoints);
                
                        var volume = GetVolume(aabb);
                  
                        if (volume < minimumVolume)
                        {
                            minimumVolume = volume;
                            minInitValues = new Vector3(x, y, z);
                            minEndValues = new Vector3(x + step, y + step, z + step);

                            
                            var center = GetCenter(aabb);
                            center = Vector3.Transform(center, rotationMatrix);

                          
                            minObb = new OBB(center, GetExtents(aabb));
                            minObb.Orientation = rotationMatrix;
                        }

                        z += step;
                    }
                    y += step;
                }
                x += step;
            }
            
            if (step > 0.01f)
                minObb = ComputeFromPointsRecursive(points, minInitValues, minEndValues, step / 10f);

            return minObb;
        }
        
        
        public bool Intersects(OBB box)// OBB vs OBB
        {
            float ra;
            float rb;
            var R = new float[3, 3];
            var AbsR = new float[3, 3];
            var ae = ToArray(Extents);
            var be = ToArray(box.Extents);

            

            var result = ToFloatArray(Matrix.Multiply(Orientation, box.Orientation));

            for (var i = 0; i < 3; i++)
                 for (var j = 0; j < 3; j++)
                     R[i, j] = result[i * 3 + j];
            

           
            var tVec = box.Center - Center;

           

            var t = ToArray(Vector3.Transform(tVec, Orientation));
            

            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    AbsR[i, j] = MathF.Abs(R[i, j]) + float.Epsilon;

            // Test axes L = A0, L = A1, L = A2
            for (var i = 0; i < 3; i++)
            {
                ra = ae[i];
                rb = be[0] * AbsR[i, 0] + be[1] * AbsR[i, 1] + be[2] * AbsR[i, 2];
                if (MathF.Abs(t[i]) > ra + rb) return false;
            }

            // Test axes L = B0, L = B1, L = B2
            for (var i = 0; i < 3; i++)
            {
                ra = ae[0] * AbsR[0, i] + ae[1] * AbsR[1, i] + ae[2] * AbsR[2, i];
                rb = be[i];
                if (MathF.Abs(t[0] * R[0, i] + t[1] * R[1, i] + t[2] * R[2, i]) > ra + rb) return false;
            }

            // Test axis L = A0 x B0
            ra = ae[1] * AbsR[2, 0] + ae[2] * AbsR[1, 0];
            rb = be[1] * AbsR[0, 2] + be[2] * AbsR[0, 1];
            if (MathF.Abs(t[2] * R[1, 0] - t[1] * R[2, 0]) > ra + rb) return false;

            // Test axis L = A0 x B1
            ra = ae[1] * AbsR[2, 1] + ae[2] * AbsR[1, 1];
            rb = be[0] * AbsR[0, 2] + be[2] * AbsR[0, 0];
            if (MathF.Abs(t[2] * R[1, 1] - t[1] * R[2, 1]) > ra + rb) return false;

            // Test axis L = A0 x B2
            ra = ae[1] * AbsR[2, 2] + ae[2] * AbsR[1, 2];
            rb = be[0] * AbsR[0, 1] + be[1] * AbsR[0, 0];
            if (MathF.Abs(t[2] * R[1, 2] - t[1] * R[2, 2]) > ra + rb) return false;

            // Test axis L = A1 x B0
            ra = ae[0] * AbsR[2, 0] + ae[2] * AbsR[0, 0];
            rb = be[1] * AbsR[1, 2] + be[2] * AbsR[1, 1];
            if (MathF.Abs(t[0] * R[2, 0] - t[2] * R[0, 0]) > ra + rb) return false;

            // Test axis L = A1 x B1
            ra = ae[0] * AbsR[2, 1] + ae[2] * AbsR[0, 1];
            rb = be[0] * AbsR[1, 2] + be[2] * AbsR[1, 0];
            if (MathF.Abs(t[0] * R[2, 1] - t[2] * R[0, 1]) > ra + rb) return false;

            // Test axis L = A1 x B2
            ra = ae[0] * AbsR[2, 2] + ae[2] * AbsR[0, 2];
            rb = be[0] * AbsR[1, 1] + be[1] * AbsR[1, 0];
            if (MathF.Abs(t[0] * R[2, 2] - t[2] * R[0, 2]) > ra + rb) return false;

            // Test axis L = A2 x B0
            ra = ae[0] * AbsR[1, 0] + ae[1] * AbsR[0, 0];
            rb = be[1] * AbsR[2, 2] + be[2] * AbsR[2, 1];
            if (MathF.Abs(t[1] * R[0, 0] - t[0] * R[1, 0]) > ra + rb) return false;

            // Test axis L = A2 x B1
            ra = ae[0] * AbsR[1, 1] + ae[1] * AbsR[0, 1];
            rb = be[0] * AbsR[2, 2] + be[2] * AbsR[2, 0];
            if (MathF.Abs(t[1] * R[0, 1] - t[0] * R[1, 1]) > ra + rb) return false;

            // Test axis L = A2 x B2
            ra = ae[0] * AbsR[1, 2] + ae[1] * AbsR[0, 2];
            rb = be[0] * AbsR[2, 1] + be[1] * AbsR[2, 0];
            if (MathF.Abs(t[1] * R[0, 2] - t[0] * R[1, 2]) > ra + rb) return false;

            // Since no separating axis is found, the OBBs must be intersecting
            return true;
        }
        
        
        public bool Intersects(BoundingSphere sphere)
        {
            // Transform sphere to OBB-Space
            var obbSpaceSphere = new BoundingSphere(ToOBBSpace(sphere.Center), sphere.Radius);

            // Create AABB enclosing the OBB
            var aabb = new BoundingBox(-Extents, Extents);

            return aabb.Intersects(obbSpaceSphere);
        }
        public Vector3 ToOBBSpace(Vector3 point)
        {
            var difference = point - Center;
            return Vector3.Transform(difference, Orientation);
        }
        
        public static BoundingBox CreateAABBFrom(Model model)
        {
            var minPoint = Vector3.One * float.MaxValue;
            var maxPoint = Vector3.One * float.MinValue;

            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            var meshes = model.Meshes;
            for (int index = 0; index < meshes.Count; index++)
            {
                var meshParts = meshes[index].MeshParts;
                for (int subIndex = 0; subIndex < meshParts.Count; subIndex++)
                {
                    var vertexBuffer = meshParts[subIndex].VertexBuffer;
                    var declaration = vertexBuffer.VertexDeclaration;
                    var vertexSize = declaration.VertexStride / sizeof(float);

                    var rawVertexBuffer = new float[vertexBuffer.VertexCount * vertexSize];
                    vertexBuffer.GetData(rawVertexBuffer);

                    for (var vertexIndex = 0; vertexIndex < rawVertexBuffer.Length; vertexIndex += vertexSize)
                    {
                        var transform = transforms[meshes[index].ParentBone.Index];
                        var vertex = new Vector3(rawVertexBuffer[vertexIndex], rawVertexBuffer[vertexIndex + 1], rawVertexBuffer[vertexIndex + 2]);
                        vertex = Vector3.Transform(vertex, transform);
                        minPoint = Vector3.Min(minPoint, vertex);
                        maxPoint = Vector3.Max(maxPoint, vertex);
                    }
                }
            }
            return new BoundingBox(minPoint, maxPoint);
        }
        
        public static OBB FromAABB(BoundingBox box)
        {
            var center = OBB.GetCenter(box);
            var extents = OBB.GetExtents(box);
            return new OBB(center, extents);
        }
        
        public static BoundingSphere CreateSphereFrom(Model model)
        {
            var minPoint = Vector3.One * float.MaxValue;
            var maxPoint = Vector3.One * float.MinValue;

            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            var meshes = model.Meshes;
            for (var index = 0; index < meshes.Count; index++)
            {
                var meshParts = meshes[index].MeshParts;
                for (var subIndex = 0; subIndex < meshParts.Count; subIndex++)
                {
                    var vertexBuffer = meshParts[subIndex].VertexBuffer;
                    var declaration = vertexBuffer.VertexDeclaration;
                    int vertexSize = declaration.VertexStride / sizeof(float);

                    var rawVertexBuffer = new float[vertexBuffer.VertexCount * vertexSize];
                    vertexBuffer.GetData(rawVertexBuffer);

                    for (var vertexIndex = 0; vertexIndex < rawVertexBuffer.Length; vertexIndex += vertexSize)
                    {
                        var transform = transforms[meshes[index].ParentBone.Index];
                        var vertex = new Vector3(rawVertexBuffer[vertexIndex], rawVertexBuffer[vertexIndex + 1], rawVertexBuffer[vertexIndex + 2]);
                        vertex = Vector3.Transform(vertex, transform);
                        minPoint = Vector3.Min(minPoint, vertex);
                        maxPoint = Vector3.Max(maxPoint, vertex);
                    }
                }
            }
            var difference = (maxPoint - minPoint) * 0.5f;
            return new BoundingSphere(difference, difference.Length());
        }
        
        
    }
    
    
}

