using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    public static class GJKAlgorithm
    {
        public static bool Intersects(Hull regioneOne, Hull regionTwo)
        {
            //Get an initial point on the Minkowski difference.
            Vector3 s = Support(regioneOne, regionTwo, Vector3.One);

            //Create our initial simplice.
            Simplice simplice = new Simplice(s);

            //Choose an initial direction toward the origin.
            Vector3 d = -s;

            //Choose a maximim number of iterations to avoid an 
            //infinite loop during a non-convergent search.
            int maxIterations = 50;

            for (int i = 0; i < maxIterations; i++)
            {
                //Get our next simplice point toward the origin.
                Vector3 a = Support(regioneOne, regionTwo, d);

                //If we move toward the origin and didn't pass it 
                //then we never will and there's no intersection.
                if (!SameDirectionTest(a, d))
                {
                    return false;
                }
                //otherwise we add the new
                //point to the simplice and
                //process it.
                simplice.Vertices.Add(a);
                //Here we either find a collision or we find the closest feature of
                //the simplice to the origin, make that the new simplice and update the direction
                //to move toward the origin from that feature.
                if (ProcessSimplex(ref simplice, ref d))
                {
                    return true;
                }
            }
            //If we still couldn't find a simplice 
            //that contains the origin then we
            //"probably" have an intersection.
            return true;
        }

        /// <summary>
        ///Either finds a collision or the closest feature of the simplice to the origin, 
        ///and updates the simplice and direction.
        /// </summary>
        static bool ProcessSimplex(ref Simplice simplice, ref Vector3 direction)
        {
            if (simplice.Vertices.Count == 2)
            {
                return ProcessLine(ref simplice, ref direction);
            }
            else if (simplice.Vertices.Count == 3)
            {
                return ProcessTriangle(ref simplice, ref direction);
            }
            else
            {
                return ProcessTetrehedron(ref simplice, ref direction);
            }
        }

        /// <summary>
        /// Determines which Veronoi region of a line segment 
        /// the origin is in, utilizing the preserved winding
        /// of the simplice to eliminate certain regions.
        /// </summary>
        static bool ProcessLine(ref Simplice simplice, ref Vector3 direction)
        {
            Vector3 a = simplice.Vertices.ElementAt(1);
            Vector3 b = simplice.Vertices.ElementAt(0);
            Vector3 ab = b - a;
            Vector3 aO = -a;

            if (SameDirectionTest(ab,aO))
            {
                float dot = Vector3.Dot(ab, aO);
                float angle = (float)Math.Acos(dot / (ab.Length() * aO.Length()));
                direction = Vector3.Cross(Vector3.Cross(ab, aO), ab);
            }
            else
            {
                simplice.Vertices.Remove(b);
                direction = aO;
            }
            return false;
        }

        /// <summary>
        /// Determines which Veronoi region of a triangle 
        /// the origin is in, utilizing the preserved winding
        /// of the simplice to eliminate certain regions.
        /// </summary>
        static bool ProcessTriangle(ref Simplice simplice, ref Vector3 direction)
        {
            Vector3 a = simplice.Vertices.ElementAt(2);
            Vector3 b = simplice.Vertices.ElementAt(1);
            Vector3 c = simplice.Vertices.ElementAt(0);
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 abc = Vector3.Cross(ab, ac);
            Vector3 aO = -a;
            Vector3 acNormal = Vector3.Cross(abc, ac);
            Vector3 abNormal = Vector3.Cross(ab, abc);

            if (SameDirectionTest(acNormal,aO))
            {
                if (SameDirectionTest(ac,aO))
                {
                    simplice.Vertices.Remove(b);
                    direction = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                }
                else
                {
                    if (SameDirectionTest(ab,aO))
                    {
                        simplice.Vertices.Remove(c);
                        direction = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                    }
                    else
                    {
                        simplice.Vertices.Remove(b);
                        simplice.Vertices.Remove(c);
                        direction = aO;
                    }
                }
            }
            else
            {
                if ( SameDirectionTest(abNormal,aO))
                {
                    if (SameDirectionTest(ab,aO))
                    {
                        simplice.Vertices.Remove(c);
                        direction = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                    }
                    else
                    {
                        simplice.Vertices.Remove(b);
                        simplice.Vertices.Remove(c);
                        direction = aO;
                    }
                }
                else
                {
                    if (SameDirectionTest(abc,aO))
                    {
                        direction = Vector3.Cross(Vector3.Cross(abc, aO), abc);
                    }
                    else
                    {
                        direction = Vector3.Cross(Vector3.Cross(-abc, aO), -abc);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines which Veronoi region of a tetrahedron
        /// the origin is in, utilizing the preserved winding
        /// of the simplice to eliminate certain regions.
        /// </summary>
        static bool ProcessTetrehedron(ref Simplice simplice, ref Vector3 direction)
        {
            Vector3 a = simplice.Vertices.ElementAt(3);
            Vector3 b = simplice.Vertices.ElementAt(2);
            Vector3 c = simplice.Vertices.ElementAt(1);
            Vector3 d = simplice.Vertices.ElementAt(0);
            Vector3 ac = c - a;
            Vector3 ad = d - a;
            Vector3 ab = b - a;
            Vector3 bc = c - b;
            Vector3 bd = d - b;

            Vector3 acd = Vector3.Cross(ad, ac);
            Vector3 abd = Vector3.Cross(ab, ad);
            Vector3 abc = Vector3.Cross(ac, ab);

            Vector3 aO = -a;

            if (SameDirectionTest(abc,aO))
            {
                if (SameDirectionTest(Vector3.Cross(abc, ac),aO))
                {
                    simplice.Vertices.Remove(b);
                    simplice.Vertices.Remove(d);
                    direction = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                }
                else if (SameDirectionTest(Vector3.Cross(ab, abc),aO))
                {
                    simplice.Vertices.Remove(c);
                    simplice.Vertices.Remove(d);
                    direction = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                }
                else
                {
                    simplice.Vertices.Remove(d);
                    direction = abc;
                }
            }
            else if (SameDirectionTest(acd,aO))
            {
                if (SameDirectionTest(Vector3.Cross(acd, ad),aO))
                {
                    simplice.Vertices.Remove(b);
                    simplice.Vertices.Remove(c);
                    direction = Vector3.Cross(Vector3.Cross(ad, aO), ad);
                }
                else if (SameDirectionTest(Vector3.Cross(ac, acd),aO))
                {
                    simplice.Vertices.Remove(b);
                    simplice.Vertices.Remove(d);
                    direction = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                }
                else
                {
                    simplice.Vertices.Remove(b);
                    direction = acd;
                }
            }
            else if (SameDirectionTest(abd,aO))
            {
                if (SameDirectionTest(Vector3.Cross(abd, ab),aO))
                {
                    simplice.Vertices.Remove(c);
                    simplice.Vertices.Remove(d);
                    direction = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                }
                else if (SameDirectionTest(Vector3.Cross(ad, abd),aO))
                {
                    simplice.Vertices.Remove(b);
                    simplice.Vertices.Remove(c);
                    direction = Vector3.Cross(Vector3.Cross(ad, aO), ad);
                }
                else
                {
                    simplice.Vertices.Remove(c);
                    direction = abd;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the furthest point on the Minkowski 
        /// difference along a given direction.
        /// </summary>
       public static Vector3 Support(
            Hull one,
            Hull two,
            Vector3 direction)
        {
            return one.GetFurthestPoint(direction) -
                two.GetFurthestPoint(-direction);
        }

       
         public static bool SameDirectionTest( Vector3 vector, Vector3 otherVector)
         {
             return Vector3.Dot(vector, otherVector) > 0;
         }

    }
}
