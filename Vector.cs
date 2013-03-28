using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    /// <summary>
    /// Generic Vector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vector<T>
    {

        protected T[] m_array;

        public int Length
        {
            get { return m_array.Length; }
        }

        public Vector(int size)
        {
            m_array = new T[size];
        }

        public Vector(Matrix<T> mat)
        {
            int size = Math.Max(mat.Width, mat.Height);
            m_array = new T[size];

            for (int i = 0; i < size; i++)
            {
                m_array[i] = mat.Height > mat.Width ? mat[i, 0] : mat[0, i];
            }
        }

        public T this[int pos]
        {
            get { return m_array[pos]; }
            set { m_array[pos] = value; }
        }

    }

    /// <summary>
    /// Vector of real numbers
    /// </summary>
    public class RVector : Vector<double>
    {
        #region Public properties
        /// <summary>
        /// Vector Euclidean norm p2
        /// </summary>
        public double Norm2
        {
            get
            {
                double n = 0;
                Parallel.For(0, m_array.Length, i =>
                                                    {
                                                        n += this[i] * this[i];
                                                    });

                return n == 0.0 ? 0 : Math.Sqrt(n);
            }
        } 
        #endregion

        #region Ctors
        public RVector(int size)
            : base(size)
        {

        }

        public RVector(RealMatrix mat)
            : base(mat)
        {
        }
        
        #endregion

        #region Operator Overloadings
        public static RVector operator *(double scalar, RVector v)
        {
            var newVector = new RVector(v.Length);

            Parallel.For(0, v.Length, i =>
                                          {
                                              newVector[i] = v[i] * scalar;
                                          });

            return newVector;
        }

        public static RVector operator *(RVector v, double scalar)
        {
            return scalar * v;
        }

        public static RVector operator /(RVector v, double scalar)
        {
            return scalar == 0.0 ? new RVector(v.Length) : v * (1 / scalar);
        }

        public static double operator *(RVector u, RVector v)
        {
            return u.Dot(v);
        }

        public static RVector operator +(RVector u, RVector v)
        {
            var x = new RVector(u.Length);
            Parallel.For(0, u.Length, i =>
                                          {
                                              x[i] = u[i] + v[i];
                                          });
            return x;
        }

        public static RVector operator -(RVector u, RVector v)
        {
            var x = new RVector(u.Length);
            Parallel.For(0, u.Length, i =>
                                          {
                                              x[i] = u[i] - v[i];
                                          });
            return x;
        }

        public static RVector operator >(RVector c1, RVector c2)
        {
            var result = new RVector(c1.Length);

            Parallel.For(0, c1.Length, i =>
            {

                result[i] = Convert.ToDouble(c1[i] > c2[i]);

            });
            return result;
        }

        public static RVector operator <(RVector c1, RVector c2)
        {
            return c2 > c1;
        } 
        #endregion

        #region Static Methods
        public static RVector Zeros(int size)
        {
            return new RVector(size);
        }

        public static RVector Ones(int size)
        {
            var v = new RVector(size);

            for (int i = 0; i < size; i++)
            {
                v[i] = 1;
            }

            return v;
        }

        public static RVector Random(int size)
        {
            return Distributions.UniformRandromVector(size);
        } 
        #endregion

        #region Public Methods
        public void Normalize()
        {

            double distance = Norm2;

            Parallel.For(0, Length, i =>
                                        {
                                            this[i] = this[i] / distance;
                                        });

        } 
        #endregion
    }

    /// <summary>
    /// Vector extension methdos
    /// </summary>
    public static class VectorEx
    {
        public static double Dot(this RVector v1, RVector v2)
        {
            double r = 0;
            Parallel.For(0, v1.Length, i =>
                                           {
                                               r += v1[i]*v2[i];
                                           });
            return r;
        }

        public static RealMatrix Outer(this RVector u, RVector v)
        {
            var A = new RealMatrix(u.Length, v.Length);
            Parallel.For(0, u.Length, i =>
                                      Parallel.For(0, v.Length, j =>
                                                                    {
                                                                        A[i, j] = u[i]*v[j];
                                                                    }));
            return A;
        }
    }
}
