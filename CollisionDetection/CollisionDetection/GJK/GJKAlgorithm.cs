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
            //initial point and direction
            Vector3 s = SupportFunction(regioneOne, regionTwo, Vector3.One);
            Vector3 dir = -s;

            Simplice simplice = new Simplice(s);

            //loop 30 times (could be higer) the algo converges but dont wana spend too much time
            for (int i = 0; i < 30; i++)
            {
                //Get our next simplice point toward the origin.
                Vector3 next_vertice = SupportFunction(regioneOne, regionTwo, dir);

                //If we move toward the origin and didn't pass it 
                //then we never will and there's no intersection.
                if (!SameDirectionTest(next_vertice, dir))
                {
                    return false;
                }

                simplice.Vertices.Add(next_vertice);
              

                //start processing GJK
                //if you have tow points (a line)
                if (simplice.Vertices.Count == 2)
                {
                    Vector3 a = simplice.Vertices.ElementAt(1);
                    Vector3 b = simplice.Vertices.ElementAt(0);
                    Vector3 ab = b - a;
                    Vector3 aO = -a;

                    if (SameDirectionTest(ab, aO))
                    {
                        float dot = Vector3.Dot(ab, aO);
                        float angle = (float)Math.Acos(dot / (ab.Length() * aO.Length()));
                        dir = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                    }
                    else
                    {
                        simplice.Vertices.Remove(b);
                        dir = aO;
                    }
                }
                else if (simplice.Vertices.Count == 3)
                {
                    //triangle test
                    Vector3 a = simplice.Vertices.ElementAt(2);
                    Vector3 b = simplice.Vertices.ElementAt(1);
                    Vector3 c = simplice.Vertices.ElementAt(0);
                    Vector3 ab = b - a;
                    Vector3 ac = c - a;
                    Vector3 abc = Vector3.Cross(ab, ac);
                    Vector3 aO = -a;
                    Vector3 acNormal = Vector3.Cross(abc, ac);
                    Vector3 abNormal = Vector3.Cross(ab, abc);

                    if (SameDirectionTest(acNormal, aO))
                    {
                        if (SameDirectionTest(ac, aO))
                        {
                            simplice.Vertices.Remove(b);
                            dir = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                        }
                        else
                        {
                            if (SameDirectionTest(ab, aO))
                            {
                                simplice.Vertices.Remove(c);
                                dir = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                            }
                            else
                            {
                                simplice.Vertices.Remove(b);
                                simplice.Vertices.Remove(c);
                                dir = aO;
                            }
                        }
                    }
                    else
                    {
                        if (SameDirectionTest(abNormal, aO))
                        {
                            if (SameDirectionTest(ab, aO))
                            {
                                simplice.Vertices.Remove(c);
                                dir = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                            }
                            else
                            {
                                simplice.Vertices.Remove(b);
                                simplice.Vertices.Remove(c);
                                dir = aO;
                            }
                        }
                        else
                        {
                            if (SameDirectionTest(abc, aO))
                            {
                                dir = Vector3.Cross(Vector3.Cross(abc, aO), abc);
                            }
                            else
                            {
                                dir = Vector3.Cross(Vector3.Cross(-abc, aO), -abc);
                            }
                        }
                    }

                }
                else //4 points test the final test
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

                    if (SameDirectionTest(abc, aO))
                    {
                        if (SameDirectionTest(Vector3.Cross(abc, ac), aO))
                        {
                            simplice.Vertices.Remove(b);
                            simplice.Vertices.Remove(d);
                            dir = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                        }
                        else if (SameDirectionTest(Vector3.Cross(ab, abc), aO))
                        {
                            simplice.Vertices.Remove(c);
                            simplice.Vertices.Remove(d);
                            dir = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                        }
                        else
                        {
                            simplice.Vertices.Remove(d);
                            dir = abc;
                        }
                    }
                    else if (SameDirectionTest(acd, aO))
                    {
                        if (SameDirectionTest(Vector3.Cross(acd, ad), aO))
                        {
                            simplice.Vertices.Remove(b);
                            simplice.Vertices.Remove(c);
                            dir = Vector3.Cross(Vector3.Cross(ad, aO), ad);
                        }
                        else if (SameDirectionTest(Vector3.Cross(ac, acd), aO))
                        {
                            simplice.Vertices.Remove(b);
                            simplice.Vertices.Remove(d);
                            dir = Vector3.Cross(Vector3.Cross(ac, aO), ac);
                        }
                        else
                        {
                            simplice.Vertices.Remove(b);
                            dir = acd;
                        }
                    }
                    else if (SameDirectionTest(abd, aO))
                    {
                        if (SameDirectionTest(Vector3.Cross(abd, ab), aO))
                        {
                            simplice.Vertices.Remove(c);
                            simplice.Vertices.Remove(d);
                            dir = Vector3.Cross(Vector3.Cross(ab, aO), ab);
                        }
                        else if (SameDirectionTest(Vector3.Cross(ad, abd), aO))
                        {
                            simplice.Vertices.Remove(b);
                            simplice.Vertices.Remove(c);
                            dir = Vector3.Cross(Vector3.Cross(ad, aO), ad);
                        }
                        else
                        {
                            simplice.Vertices.Remove(c);
                            dir = abd;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

   

      



        public static Vector3 SupportFunction(Hull one, Hull two, Vector3 direction)
        {
            // get furthest point on the  
            Vector3 furthest_for_one = one.GetFurthestPoint(direction);
            Vector3 furthest_for_two = two.GetFurthestPoint(-direction);

            // get minkowsi difference along a given direction.
            return furthest_for_one - furthest_for_two;
        }

       
         public static bool SameDirectionTest( Vector3 vector, Vector3 otherVector)
         {
             return Vector3.Dot(vector, otherVector) > 0;
         }

    }
}
