using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearn
{
    public class Matrix<T> : IEnumerable<T> 
    {
        #region Fields
        protected readonly T[,] m_array; 
        #endregion

        #region Public Properties
        public int Height { get { return m_array.GetLength(0); } }

        public int Length { get { return m_array.GetLength(1); } } 
        #endregion

        #region Ctors
        public Matrix(int rows, int cols)
        {
            m_array = new T[rows, cols];
        }

        public Matrix(T[,] data)
            : this(data.GetLength(0), data.GetLength(1))
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    this[i, j] = data[i, j];
                }
            }
        }

        public Matrix(Matrix<T> matrix)
            : this(matrix.ToArray())
        {
        }

        public Matrix(T[][] data)
            : this(data.GetLength(0), data[0].Length)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Length; j++)
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
            get { return m_array[i, j]; }
            set { m_array[i, j] = value; }
        } 
        #endregion

        #region Enumerators
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_array.GetLength(0); i++)
            {
                for (int j = 0; j < m_array.GetLength(1); j++)
                {
                    yield return m_array[i, j];
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
                var newMat = new Matrix<T>(Length, Height);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Length; j++)
                    {
                        newMat[j, i] = this[i, j];
                    }
                }

                return newMat;
            }

        }

        public T[,] ToArray()
        {
            return m_array;
        }

        public T[][] ToJaggedArray()
        {
            var arr = new T[Height][];
            for (int i = 0; i < Height; i++)
            {
                arr[i] = new T[Length];
                for (int j = 0; j < Length; j++)
                {
                    arr[i][j] = this[i, j];
                }
            }

            return arr;
        }

        public virtual Matrix<T> Submatrix(int mStart, int nStart, int mSize = 0, int nSize = 0)
        {
            mSize = mSize == 0 ? Height - mStart : mSize;
            nSize = nSize == 0 ? Length - nStart : nSize;

            var x = new Matrix<T>(mSize, nSize);

            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < nSize; j++)
                {
                    x[i, j] = this[i + mStart, j + nStart];
                }
            }

            return x;
        }
 
        #endregion

        public static implicit operator T[][](Matrix<T> d)
        {
            return d.ToJaggedArray();
        }
    }


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
            var mat = new RealMatrix(c1.Height, c1.Length);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Length; j++)
                {
                    mat[i, j] = c1[i, j] + c2[i, j];
                }
            }
            return mat;
        }

        public static RealMatrix operator -(RealMatrix c1, RealMatrix c2)
        {
            var mat = new RealMatrix(c1.Height, c1.Length);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Length; j++)
                {
                    mat[i, j] = c1[i, j] - c2[i, j];
                }
            }
            return mat;
        }

        public static RealMatrix operator *(RealMatrix c1, RealMatrix c2)
        {
            if (c1.Length != c2.Height)
            {
                throw new ArithmeticException("Width of the first matrix must equal the height of the second matrix");

            }

            int height = c1.Height;
            int width = c2.Length;
            int n = c1.Length;

            var result = new RealMatrix(height, width);

            Parallel.For(0, height, i =>
                                        {
                                            for (int j = 0; j < width; j++)
                                            {
                                                result[i, j] = 0.0;

                                                for (int k = 0; k < n; k++)
                                                {
                                                    result[i, j] += c1[i, k]*c2[k, j];
                                                }
                                            }
                                        });

            return result;

        }

        //public static RealMatrix operator *(RealMatrix c1, Matrix<double> c2)
        //{
        //    return c1*(RealMatrix) c2;
        //}

        //public static RealMatrix operator *( Matrix<double> c2,RealMatrix c1)
        //{
        //    return (RealMatrix) c2*c1;
        //}

        public static RealMatrix operator >(RealMatrix c1, RealMatrix c2)
        {
            var result = new RealMatrix(c1.Height, c1.Length);
            
            Parallel.For(0, c1.Height, i =>
                                           {
                                               for (int j = 0; j < c1.Length; j++)
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

        #endregion

        #region Scalar Operations

        public static RealMatrix operator +(RealMatrix c1, double scalar)
        {
            var mat = new RealMatrix(c1.Height, c1.Length);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Length; j++)
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
            var mat = new RealMatrix(c1.Height, c1.Length);

            if (scalar != 2)
            {
                Parallel.For(0, c1.Height, i =>
                                               {
                                                   for (int j = 0; j < c1.Length; j++)
                                                   {
                                                       mat[i, j] = Math.Pow(c1[i, j], scalar);
                                                   }
                                               });
            }
            else
            {
                Parallel.For(0, c1.Height, i =>
                                               {
                                                   for (int j = 0; j < c1.Length; j++)
                                                   {
                                                       mat[i, j] = c1[i, j]*c1[i, j];
                                                   }
                                               });
            }
            return mat;
        }


        public static RealMatrix operator *(RealMatrix c1, double scalar)
        {
            var mat = new RealMatrix(c1.Height, c1.Length);

            for (int i = 0; i < c1.Height; i++)
            {
                for (int j = 0; j < c1.Length; j++)
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
                for (int j = 0; j < Length; j++)
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
                for (int j = 0; j < matrix.Length; j++)
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
                for (int j = 0; j < matrix.Length; j++)
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
                    A[i, j] = r.NextDouble();
                    A[j, i] = A[i, j];
                }

            return A;
        } 
        #endregion

        #region Polymorphism generics compatibility
        public new RealMatrix Submatrix(int mStart, int nStart, int mSize = 0, int nSize = 0)
        {
            return (RealMatrix)base.Submatrix(mStart, nStart, mSize, nSize);
        }

        public new RealMatrix Transpose
        {
            get
            {
                var newMat = new RealMatrix(Length, Height);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Length; j++)
                    {
                        newMat[j, i] = this[i, j];
                    }
                }

                return newMat;
            }

        }
        
        #endregion
    }

   

    public static class MatrixExtensions
    {
        public static RealMatrix InsertRow(this RealMatrix matrix, int value)
        {
            var mat = new RealMatrix(matrix.Height + 1, matrix.Length);

            
                Parallel.For(0, matrix.Height, i =>
                                               Parallel.For(0, matrix.Length, j =>
                                                                                  {
                                                                                      mat[i + 1, j] = matrix[i, j];
                                                                                  }));

            
            Parallel.For(0, mat.Length, i =>
                                            {
                                                mat[0, i] = value;
                                            });

            return mat;
        }

        public static RealMatrix InsertCol(this RealMatrix matrix, int value)
        {
            var mat = new RealMatrix(matrix.Height, matrix.Length + 1);

            Parallel.For(0, matrix.Height, i =>
                                           Parallel.For(0, matrix.Length, j =>
                                                                              {
                                                                                  mat[i, j + 1] = matrix[i, j];
                                                                              }));


            Parallel.For(0, mat.Height, i =>
                                            {
                                                mat[i, 0] = value;
                                            });

            return mat;
        }

        public static RealMatrix RemoveFirstCol(this RealMatrix matrix)
        {
            var mat = new RealMatrix(matrix.Height, matrix.Length - 1);

            Parallel.For(0, mat.Height, i =>
                                           Parallel.For(0, mat.Length, j =>
                                                                              {
                                                                                  mat[i, j] = matrix[i, j + 1];
                                                                              }));

            return mat;
        }

        public static RealMatrix Update(this RealMatrix matrix, int index, int value, int axis)
        {
            var mat = new RealMatrix(matrix);

            if (axis == 0)
            {
                Parallel.For(0, matrix.Length, i =>
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
        /// Q-R Decomposition
        /// </summary>
        /// <param name="A">Source matrix</param>
        /// <returns>{Q,R} in array form</returns>
        public static RealMatrix[] QR(RealMatrix A)
        {
            int m = A.Height;
            int n = A.Length;

            var Q = RealMatrix.I(m);

            var R = new RealMatrix(A);

            for (int k = 0; k < n; k++)
            {
                var x = new RVector(R.Submatrix(k, k, 0, 1));
                var e = RVector.Zeros(x.Length);
                e[0] = 1;
                var u = -Math.Sign(x[0]) * x.Norm2 * e + x;
                u = u / u.Norm2;
                var qn = RealMatrix.I(u.Length) - 2 * u.Outer(u);

                //var r = R.Submatrix(k, k) - 2 * u.Outer(u) * R.Submatrix(k, k);
                var q = RealMatrix.I(Q.Height);
                q.Update(k, k, qn);
                Q = Q * q;
                R = Q.Transpose * A;

            }

            return new[] { Q, R };
        }

        public static void Update(this RealMatrix matrix, int mPos, int nPos, RealMatrix src, int mSize = 0, int nSize = 0)
        {
            mSize = mSize == 0 ? matrix.Height - mPos : mSize;
            nSize = nSize == 0 ? matrix.Length - nPos : nSize;

            for (int i = 0; i < mSize; i++)
            {
                for (int j = 0; j < nSize; j++)
                {
                    matrix[i + mPos, j + nPos] = src[i, j];
                }
            }
        }
    }
   
}
