using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    /// <summary>
    /// Generic Matrix
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Matrix<T> : IEnumerable<T> 
    {
        #region Fields
        protected readonly T[][] m_array; 
        #endregion

        #region Public Properties
        public int Height { get { return m_array.Length; } }

        public int Width { get { return m_array[0].Length; } } 
        #endregion

        #region Ctors
        public Matrix(int rows, int cols)
        {
            m_array = new T[rows][];

            //init
            for (int i = 0; i < rows; i++)
            {
                m_array[i] = new T[cols];
            }
        }

        public Matrix(T[,] data)
            : this(data.GetLength(0), data.GetLength(1))
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = data[i, j];
                }
            }
        }

        public Matrix(Matrix<T> matrix)
            : this(matrix.m_array)
        {
        }

        public Matrix(T[][] data)
            : this(data.GetLength(0), data[0].Length)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = data[i][j];
                }
            }
        }

        public Matrix(Vector<T> v)
            : this(v.Length, 1)
        {
            for (int i = 0; i < v.Length; i++)
            {
                this[i, 0] = v[i];
            }
        } 
        #endregion

        #region Indexers
        public T this[int i, int j]
        {
            get { return m_array[i][j]; }
            set { m_array[i][j] = value; }
        } 
        #endregion

        #region Enumerators
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    yield return m_array[i][j];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 
        #endregion

        #region Public Properties
        
        public Matrix<T> Transpose
        {
            get
            {
                var newMat = new Matrix<T>(Width, Height);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        newMat[j, i] = this[i, j];
                    }
                }

                return newMat;
            }

        }
        
        public T[][] ToArray()
        {
            return m_array;
        }

        
 
        #endregion

        public static implicit operator T[][](Matrix<T> d)
        {
            return d.ToArray();
        }
    }

    /// <summary>
    /// Matrix of real numbers
    /// </summary>
    public class RealMatrix : Matrix<double>
    {
        #region Ctors

        public RealMatrix(int rows, int cols)
            : base(rows, cols)
        {
        }

        public RealMatrix(double[,] data)
            : base(data)
        {
        }

        public RealMatrix(double[][] data)
            : base(data)
        {
        }

        public RealMatrix(Vector<double> v)
            : base(v)
        {
        }

        public RealMatrix(RealMatrix matrix ) : base(matrix)
        {
        }

        #endregion

        #region Matrix Operations

        public static RealMatrix operator +(RealMatrix c1, RealMatrix c2)
        {
            var mat = new RealMatrix(c1.Height, c1.Width);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Width; j++)
                {
                    mat[i, j] = c1[i, j] + c2[i, j];
                }
            }
            return mat;
        }

        public static RealMatrix operator -(RealMatrix c1, RealMatrix c2)
        {
            var mat = new RealMatrix(c1.Height, c1.Width);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Width; j++)
                {
                    mat[i, j] = c1[i, j] - c2[i, j];
                }
            }
            return mat;
        }

        public static RealMatrix operator *(RealMatrix c1, RealMatrix c2)
        {
#if DEBUG
            return Product(c1, c2);
#else
            return UnsafeMultiplication(c1, c2);
#endif
        }

        public static RealMatrix operator >(RealMatrix c1, RealMatrix c2)
        {
            var result = new RealMatrix(c1.Height, c1.Width);
            
            Parallel.For(0, c1.Height, i =>
                                           {
                                               for (int j = 0; j < c1.Width; j++)
                                               {
                                                   result[i, j] = Convert.ToDouble(c1[i, j] > c2[i, j]);
                                               }
                                           });
            return result;
        }

        public static RealMatrix operator <(RealMatrix c1, RealMatrix c2)
        {
            return c2 > c1;
        }

        public static RealMatrix Product(RealMatrix matrixA, RealMatrix matrixB)
        {
            int aRows = matrixA.Height;
            int aCols = matrixA.Width;
            int bRows = matrixB.Height;
            int bCols = matrixB.Width;
            if (aCols != bRows)
                throw new Exception("xxxx");

            var result = new RealMatrix(aRows, bCols);

            Parallel.For(0, aRows, i =>
                                                                                           {
                                                                                               for (int j = 0;
                                                                                                    j < bCols;
                                                                                                    ++j)
                                                                                                   // each col of B
                                                                                                   for (int k = 0;
                                                                                                        k < aCols;
                                                                                                        ++k)
                                                                                                       // could use k < bRows
                                                                                                   {
                                                                                                       result[i, j] +=
                                                                                                           matrixA[i, k]*
                                                                                                           matrixB[k, j];
                                                                                                   }
                                                                                           }

                );

            return result;
        }

#if !DEBUG
        public unsafe static RealMatrix UnsafeMultiplication(RealMatrix m1, RealMatrix m2)
        {
            int h = m1.Height;
            int w = m2.Width;
            int l = m1.Width;
            RealMatrix resultMatrix = new RealMatrix(h, w);

            Parallel.For(0, h, i =>
                               //for (int i = 0; i < h; i++)
                                   {
                                       fixed (double* pm = resultMatrix.m_array[i], pm1 = m1.m_array[i])
                                       {
                                           for (int j = 0; j < w; j++)
                                           {
                                               double res = 0;
                                               for (int k = 0; k < l; k++)
                                               {
                                                   fixed (double* pm2 = m2.m_array[k])
                                                       res += pm1[k]*pm2[j];
                                               }
                                               pm[j] = res;

                                           }
                                       }
                                   });

            return resultMatrix;
        }
#endif
        #endregion

        #region Scalar Operations

        public static RealMatrix operator +(RealMatrix c1, double scalar)
        {
            var mat = new RealMatrix(c1.Height, c1.Width);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Width; j++)
                {
                    mat[i, j] = c1[i, j] + scalar;
                }
            }
            return mat;
        }

        public static RealMatrix operator /(RealMatrix c1, double scalar)
        {
            return scalar == 0.0 ? c1 : c1*(1/scalar);
        }

        public static RealMatrix operator ^(RealMatrix c1, double scalar)
        {
            var mat = new RealMatrix(c1.Height, c1.Width);

            if (scalar != 2)
            {
                Parallel.For(0, c1.Height, i =>
                                               {
                                                   for (int j = 0; j < c1.Width; j++)
                                                   {
                                                       mat[i, j] = Math.Pow(c1[i, j], scalar);
                                                   }
                                               });
            }
            else
            {
                Parallel.For(0, c1.Height, i =>
                                               {
                                                   for (int j = 0; j < c1.Width; j++)
                                                   {
                                                       mat[i, j] = c1[i, j]*c1[i, j];
                                                   }
                                               });
            }
            return mat;
        }


        public static RealMatrix operator *(RealMatrix c1, double scalar)
        {
            var mat = new RealMatrix(c1.Height, c1.Width);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Width; j++)
                {
                    mat[i, j] = c1[i, j]*scalar;
                }
            }
            return mat;
        }

        public static RealMatrix operator *(double scalar, RealMatrix c1)
        {
            return c1*scalar;
        }

        #endregion

        #region Vector Operations
        public static RealMatrix operator *(RealMatrix m, RVector v)
        {
            return m * new RealMatrix(v);
        }

        public static RealMatrix operator *(RVector v, RealMatrix m)
        {
            return new RealMatrix(v).Transpose * m;
        } 
        #endregion
        
        public override string ToString()
        {
            var str = new StringBuilder();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    str.Append(this[i, j].ToString("N2"));
                    str.Append(", ");
                }
                str.Append("\n");
            }
            return str.ToString();
        }
        
        #region Static methods
        public static RealMatrix Ones(int rows, int cols)
        {
            var matrix = new RealMatrix(rows, cols);

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    matrix[i, j] = 1;
                }
            }

            return matrix;
        }

        public static RealMatrix Zeros(int rows, int cols)
        {
            var matrix = new RealMatrix(rows, cols);

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    matrix[i, j] = 0;
                }
            }

            return matrix;
        }
        
        public static RealMatrix I(int m)
        {
            var x = new RealMatrix(m, m);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    x[i, j] = i == j ? 1 : 0;
                }
            }

            return x;
        }

        public static RealMatrix SymetricRandom(int m)
        {
            var A = new RealMatrix(m, m);
            var r = new Random();
            for (int i = 0; i < m; i++)
                for (int j = 0; j < i; j++)
                {
                    A[i, j] = Distributions.GetRandomDouble();
                    A[j, i] = A[i, j];
                }

            return A;
        } 
        #endregion

        #region Polymorphism generics compatibility
        public  RealMatrix Submatrix(int mStart, int nStart, int mSize = 0, int nSize = 0)
        {
            mSize = mSize == 0 ? Height - mStart : mSize;
            nSize = nSize == 0 ? Width - nStart : nSize;

            var x = new RealMatrix(mSize, nSize);

            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < nSize; j++)
                {
                    x[i, j] = this[i + mStart, j + nStart];
                }
            }

            return x;
        }

        public new RealMatrix Transpose
        {
            get
            {
                var newMat = new RealMatrix(Width, Height);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        newMat[j, i] = this[i, j];
                    }
                }

                return newMat;
            }

        }
        
        #endregion

        public RVector ToVector()
        {
            if(Width == 1)
            {
                return new RVector(Submatrix(0, 0, 0, 1));
            }
            else if(Height == 1)
            {
                return new RVector(Submatrix(0, 0, 1));
            }
            else
            {
                throw new InvalidOperationException("Matrix is not a one liner");
            }
        }
    }


    /// <summary>
    /// Real matrix extension methods
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Insert a new row at a first row in a matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RealMatrix InsertRow(this RealMatrix matrix, int value)
        {
            var mat = new RealMatrix(matrix.Height + 1, matrix.Width);


            Parallel.For(0, matrix.Height, i =>
                                           Parallel.For(0, matrix.Width, j =>
                                                                              {
                                                                                  mat[i + 1, j] = matrix[i, j];
                                                                              }));


            Parallel.For(0, mat.Width, i =>
                                            {
                                                mat[0, i] = value;
                                            });

            return mat;
        }

        /// <summary>
        /// Insert a new column as a first column in a matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RealMatrix InsertCol(this RealMatrix matrix, int value)
        {
            var mat = new RealMatrix(matrix.Height, matrix.Width + 1);

            Parallel.For(0, matrix.Height, i =>
                                           Parallel.For(0, matrix.Width, j =>
                                                                              {
                                                                                  mat[i, j + 1] = matrix[i, j];
                                                                              }));


            Parallel.For(0, mat.Height, i =>
                                            {
                                                mat[i, 0] = value;
                                            });

            return mat;
        }

        /// <summary>
        /// Remove first Column
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static RealMatrix RemoveFirstCol(this RealMatrix matrix)
        {
            var mat = new RealMatrix(matrix.Height, matrix.Width - 1);

            Parallel.For(0, mat.Height, i =>
                                        Parallel.For(0, mat.Width, j =>
                                                                        {
                                                                            mat[i, j] = matrix[i, j + 1];
                                                                        }));

            return mat;
        }

        /// <summary>
        /// Update a certain axis in a matrix with a specific value
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="index">Offset point</param>
        /// <param name="value"></param>
        /// <param name="axis">0 -> left to right, 1 -> Up and down</param>
        /// <returns></returns>
        public static RealMatrix Update(this RealMatrix matrix, int index, int value, int axis)
        {
            var mat = new RealMatrix(matrix);

            if (axis == 0)
            {
                Parallel.For(0, matrix.Width, i =>
                                                   {
                                                       mat[index, i] = value;
                                                   });
            }
            else
            {
                Parallel.For(0, matrix.Height, i =>
                                                   {
                                                       mat[i, index] = value;
                                                   });
            }

            return mat;
        }

        /// <summary>
        /// Update a selection range in a matrix with another matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="mPos"></param>
        /// <param name="nPos"></param>
        /// <param name="src"></param>
        /// <param name="mSize"></param>
        /// <param name="nSize"></param>
        public static void Update(this RealMatrix matrix, int mPos, int nPos, RealMatrix src, int mSize = 0,
                                  int nSize = 0)
        {
            mSize = mSize == 0 ? matrix.Height - mPos : mSize;
            nSize = nSize == 0 ? matrix.Width - nPos : nSize;

            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < nSize; j++)
                {
                    matrix[i + mPos, j + nPos] = src[i, j];
                }
            }
        }

        /// <summary>
        /// Update a selection with a source vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="src"></param>
        /// <param name="length"></param>
        /// <param name="isVertical"></param>
        /// <param name="mPos"></param>
        /// <param name="nPos"></param>
        public static void Update(this RealMatrix matrix, RVector src, int length = 0, bool isVertical = true,
                                  int mPos = 0, int nPos = 0)
        {
            length = length == 0 ? src.Length : length;


            for (int i = 0; i < length; i++)
            {
                if (isVertical)
                {
                    matrix[mPos + i, nPos] = src[i];
                }
                else
                {
                    matrix[mPos, nPos + i] = src[i];
                }
            }

        }


        /// <summary>
        /// Q-R Decomposition
        /// </summary>
        /// <param name="A">Source matrix</param>
        /// <returns>{Q,R} in array form</returns>
        public static RealMatrix[] QR(RealMatrix A)
        {
            int m = A.Height;
            int n = A.Width;

            var Q = RealMatrix.I(m);

            var R = new RealMatrix(A);

            for (int k = 0; k < n; k++)
            {
                var x = new RVector(R.Submatrix(k, k, 0, 1));
                var e = RVector.Zeros(x.Length);
                e[0] = 1;
                var u = -Math.Sign(x[0])*x.Norm2*e + x;
                u = u/u.Norm2;
                var qn = RealMatrix.I(u.Length) - 2*u.Outer(u);

                //var r = R.Submatrix(k, k) - 2 * u.Outer(u) * R.Submatrix(k, k);
                var q = RealMatrix.I(Q.Height);
                q.Update(k, k, qn);
                Q = Q*q;
                R = Q.Transpose*A;

            }

            return new[] {Q, R};
        }

    }

}
