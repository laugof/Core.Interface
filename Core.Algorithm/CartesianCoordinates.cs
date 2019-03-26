﻿using Core.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Author: Laurent Goffin
/// </summary>
namespace Core.Algorithm
{
    /// <summary>
    /// Properties computed from ICartesianCoordinates 
    /// </summary>
    public static class CartesianCoordinates
    {
        #region Norm

        /// <summary>
        /// Squared norm of v
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static double Norm2(IVector<double> v)
        {
            var norm2 = 0.0;
            for (int i = 0; i < v.Length; ++i)
                norm2 += v[i] * v[i];
            return norm2;
        }

        /// <summary>
        /// Norm of v
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static double Norm(IVector<double> v) => Math.Sqrt(Norm2(v));

        /// <summary>
        /// Return normalized vector of v multiplied by a scalar
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static I2dCartesianCoordinates<double> Normalize(I2dCartesianCoordinates<double> v, double scalar = 1.0)
        {
            var norm = Norm(v);
            var p = new Point2d();
            if (norm > 0) p.SetCartesian(scalar * v.X / norm, scalar * v.Y / norm);
            return p;
        }

        /// <summary>
        /// Return normalized vector of v multiplied by a scalar
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static I3dCartesianCoordinates<double> Normalize(I3dCartesianCoordinates<double> v, double scalar = 1.0)
        {
            var norm = Norm(v);
            var p = new Point3d();
            if (norm > 0) p.SetCartesian(scalar * v.X / norm, scalar * v.Y / norm, scalar * v.Z / norm);
            return p;
        }

        #endregion Norm

        #region Distance

        /// <summary>
        /// Squared distance between p1 and p2
        /// </summary>
        /// <param name="p1">Point1</param>
        /// <param name="p2">Point2</param>
        /// <returns></returns>
        public static double Distance2(IVector<double> p1, IVector<double> p2)
        {
            Debug.Assert(p1.Length == p2.Length);
            var distance = 0.0;
            var dimension = p1.Length;
            if (p2.Length < dimension) dimension = p2.Length; // FIXME >
            for (int i = 0; i < dimension; ++i)
            {
                var a1 = i < p1.Length ? p1[i] : 0.0;
                var a2 = i < p2.Length ? p2[i] : 0.0;
                var x = a1 - a2;
                distance += x * x;
            }
            return distance;
        }

        /// <summary>
        /// Distance between p1 and p2
        /// </summary>
        /// <param name="p1">Point1</param>
        /// <param name="p2">Point2</param>
        /// <returns></returns>
        public static double Distance(IVector<double> p1, IVector<double> p2) => Math.Sqrt(Distance2(p1, p2));

        #endregion Distance

        #region Barycenter

        internal class Point2d : IPoint2d
        {
            public double X { get; set; }
            public double Y { get; set; }

            public void SetCartesian(double x, double y) { X = x; Y = y; }
            public void SetCartesian(I2dCartesianCoordinates<double> p) { X = p.X; Y = p.Y; }

            public double this[int index]
            {
                get
                {
                    Debug.Assert(index >= 0 && index < Length);
                    switch (index)
                    {
                        case 0: return X;
                        case 1: return Y;
                        default: return double.NaN;
                    }
                }

                set
                {
                    Debug.Assert(index >= 0 && index < Length);
                    switch (index)
                    {
                        case 0: X = value; break;
                        case 1: Y = value; break;
                        default: break;
                    }
                }
            }

            public int Length { get { return 2; } }
        }

        /// <summary>
        /// Compute the center of gravity of the point cloud (Averaged point)
        /// </summary>
        /// <param name="pl">point list</param>
        /// <returns>Center of gravity</returns>
        public static I2dCartesianCoordinates<double> Barycenter(List<I2dCartesianCoordinates<double>> pl)
        {
            IPoint2d CenterOfGravity = new Point2d();
            foreach (var point in pl)
            {
                CenterOfGravity.X += point.X;
                CenterOfGravity.Y += point.Y;
            }
            if (pl.Count > 0)
            {
                var n = 1.0 / (double)pl.Count;
                CenterOfGravity.X *= n;
                CenterOfGravity.Y *= n;
            }
            return CenterOfGravity;
        }

        internal class Point3d : I3dCartesianCoordinates<double>
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public void SetCartesian(double x, double y, double z) { X = x; Y = y; Z = z; }
            public void SetCartesian(I2dCartesianCoordinates<double> p) { X = p.X; Y = p.Y; Z = 0.0; }
            public void SetCartesian(I3dCartesianCoordinates<double> p) { X = p.X; Y = p.Y; Z = p.Z; }

            public double this[int index]
            {
                get
                {
                    Debug.Assert(index >= 0 && index <= 2);
                    switch (index)
                    {
                        case 0: return X;
                        case 1: return Y;
                        case 2: return Z;
                        default: return double.NaN;
                    }
                }

                set
                {
                    Debug.Assert(index >= 0 && index <= 2);
                    switch (index)
                    {
                        case 0: X = value; break;
                        case 1: Y = value; break;
                        case 2: Z = value; break;
                        default: break;
                    }
                }
            }

            public int Length { get { return 3; } }
        }

        /// <summary>
        /// Compute the center of gravity of the point cloud (Averaged point)
        /// </summary>
        /// <param name="pl">point list</param>
        /// <returns>Center of gravity</returns>
        public static I3dCartesianCoordinates<double> Barycenter(List<I3dCartesianCoordinates<double>> pl)
        {
            I3dCartesianCoordinates<double> CenterOfGravity = new Point3d();
            foreach (var point in pl)
            {
                CenterOfGravity.X += point.X;
                CenterOfGravity.Y += point.Y;
                CenterOfGravity.Z += point.Z;
            }
            if (pl.Count > 0)
            {
                var n = 1.0 / (double)pl.Count;
                CenterOfGravity.X *= n;
                CenterOfGravity.Y *= n;
                CenterOfGravity.Z *= n;
            }
            return CenterOfGravity;
        }

        #endregion Barycenter

        /// <summary>
        /// Return if point is on a geometric shape
        /// </summary>
        /// <param name="shape">3d geometric shape</param>
        /// <param name="point">3d point</param>
        /// <param name="distanceTolerance">Is on the shape if distance is less than tolerance</param>
        public static bool IsOn(I3dPointDistance<double> shape, I3dCartesianCoordinates<double> point, double distanceTolerance = 1e-6)
        {
            var distance = shape.Distance(point);
            return distance < 0.0 ? false : distance <= distanceTolerance;
        }

        /// <summary>
        /// Return the addition: v1 + v2
        /// </summary>
        /// <param name="v1">3d vector</param>
        /// <param name="v2">3d vector</param>
        public static I3dCartesianCoordinates<double> Add(I3dCartesianCoordinates<double> v1, I3dCartesianCoordinates<double> v2)
        {
            var p = new Point3d();
            p.SetCartesian(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
            return p;
        }

        /// <summary>
        /// Mutiply scalar to Vector
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="v">Vector</param>
        /// <returns></returns>
        public static I3dCartesianCoordinates<double> Multiply(double scalar, I3dCartesianCoordinates<double> v)
        {
            var p = new Point3d();
            p.SetCartesian(scalar * v.X, scalar * v.Y, scalar * v.Z);
            return p;
        }

        public static I3dCartesianCoordinates<double> Multiply(I3dCartesianCoordinates<double> v, double scalar) => Multiply(scalar, v);

        /// <summary>
        /// Return the cross product: v1 x v2
        /// </summary>
        /// <param name="v1">3d vector</param>
        /// <param name="v2">3d vector</param>
        public static I3dCartesianCoordinates<double> CrossProduct(I3dCartesianCoordinates<double> v1, I3dCartesianCoordinates<double> v2)
        {
            var X = (v1.Y * v2.Z) - (v1.Z * v2.Y);
            var Y = (v1.Z * v2.X) - (v1.X * v2.Z);
            var Z = (v1.X * v2.Y) - (v1.Y * v2.X);
            var p = new Point3d();
            p.SetCartesian(X, Y, Z);
            return p;
        }

        /// <summary>
        /// Return the cross product: v1 x v2
        /// </summary>
        /// <param name="v1">2d vector</param>
        /// <param name="v2">2d vector</param>
        public static I3dCartesianCoordinates<double> CrossProduct(I2dCartesianCoordinates<double> v1, I2dCartesianCoordinates<double> v2)
        {
            var Z = (v1.X * v2.Y) - (v1.Y * v2.X);
            var p = new Point3d();
            p.SetCartesian(0.0, 0.0, Z);
            return p;
        }

        /// <summary>
        /// Dot product (scalar product)
        /// </summary>
        /// <param name="A">Vector</param>
        /// <param name="B">Vector</param>
        /// <returns>double</returns>
        public static double DotProduct(IVector<double> A, IVector<double> B)
        {
            var length = Math.Max(A.Length, B.Length);
            double s1 = 0.0;
            for (int i = 0; i < length; ++i)
            {
                var a = (i < A.Length) ? A[i] : 0;
                var b = (i < B.Length) ? B[i] : 0;
                s1 += a * b;
            }
#if DEBUG
            //var s2 = (A.GetRowMatrix * B.GetColMatrix)[0, 0];
            //var Zero = s1 - s2;
#endif //DEBUG
            return s1;
        }

        /// <summary>
        /// Dot product (scalar product)
        /// </summary>
        /// <param name="A">Vector</param>
        /// <param name="B">Vector</param>
        /// <returns>double</returns>
        public static double ScalarProduct(IVector<double> A, IVector<double> B) => DotProduct(A, B);

        /// <summary>
        /// Return a vector orthogonal to V
        /// </summary>
        /// <param name="V">Vector 3d</param>
        /// <param name="normalize">Normalize the orthogonal vector?</param>
        public static I3dCartesianCoordinates<double> Orthogonal(I3dCartesianCoordinates<double> V, bool normalize = true)
        {
            var O = new Point3d();
            var i = 0;
            if (Math.Abs(V[1]) < Math.Abs(V[0])) i = 1;
            if (Math.Abs(V[2]) < Math.Abs(V[i])) i = 2;
            switch (i)
            {
                case 0: O.SetCartesian(0.0, -V.Z, V.Y); break;
                case 1: O.SetCartesian(-V.Z, 0.0, V.X); break;
                case 2: O.SetCartesian(-V.Y, V.X, 0.0); break;
                default: break;
            }
            //Debug.Assert(Core.Mathematics.Utilities.Near(Vector3d.Angle(V, O).Get(Core.Angle.Unit.deg), 90.0, 1e-6));
            if (normalize) return Normalize(O);
            return O;
        }

        /// <summary>
        /// Are the two vectors orthogonal?
        /// </summary>
        /// <param name="A">3d vector</param>
        /// <param name="B">3d vector</param>
        /// <param name="epsilon">Tolerance</param>
        /// <returns></returns>
        public static bool Orthogonal(IVector<double> A, IVector<double> B, double epsilon = 1e-6)
        {
            return DotProduct(A, B) < epsilon;
        }
    }
}
