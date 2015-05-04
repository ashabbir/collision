using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionDetection
{
    public static class GJKAlgorithm
    {
        public static bool Process(Hull hull_first, Hull hull_second)
        {
            //initial point and direction
            Vector3 initial_vertex = SupportFunction(hull_first, hull_second, Vector3.One);
            Vector3 dir = Vector3.Negate(initial_vertex);

            List<Vector3> simplice = new List<Vector3>() { initial_vertex };

            int counter = 0;
            //http://www.dyn4j.org/2010/04/gjk-gilbert-johnson-keerthi/
           
            
            while(true)
            {
                //Get our next simplice point toward the origin.
                Vector3 next_vertex = SupportFunction(hull_first, hull_second, dir);

                //
                if (!DirectionTest(next_vertex, dir))
                {
                    return false;
                }
                else 
                {
                    //the thing gets stuck for ever have to break out of loop
                    counter++;
                    if (counter > 100)
                    {
                        return true;
                    }
                }

                simplice.Add(next_vertex);
              

                //start processing GJK
                if (simplice.Count == 2)
                {
                    #region 2 point test -- line
                    Vector3 a = simplice[1];
                    Vector3 b = simplice[0];
                    Vector3 ab = b - a;
                    

                    if (DirectionTest(ab, -a))
                    {
                        dir = trippleproduct(ab, -a, ab);
                    }
                    else
                    {
                        simplice.Remove(b);
                        dir = -a;
                    }
                    #endregion
                }
                else if (simplice.Count == 3)
                {
                    # region 3 point test -- triangle
                    Vector3 a = simplice[2];
                    Vector3 b = simplice[1];
                    Vector3 c = simplice[0];
                    
                    
                    Vector3 ab = b - a;
                    Vector3 ac = c - a;

                    

                    Vector3 abc = Vector3.Cross(ab, ac);
                    Vector3 acNormal = Vector3.Cross(abc, ac);
                    Vector3 abNormal = Vector3.Cross(ab, abc);

                    if (DirectionTest(acNormal, -a))
                    {
                        if (DirectionTest(ac, -a))
                        {
                            simplice.Remove(b);
                            dir = trippleproduct(ac, -a, ac);
                        }
                        else
                        {
                            if (DirectionTest(ab, -a))
                            {
                                simplice.Remove(c);
                                dir = trippleproduct(ab,-a,ab);
                            }
                            else
                            {
                                simplice.Remove(b);
                                simplice.Remove(c);
                                dir = -a;
                            }
                        }
                    }
                    else
                    {
                        if (DirectionTest(abNormal, -a))
                        {
                            if (DirectionTest(ab, -a))
                            {
                                simplice.Remove(c);
                                dir = trippleproduct(ab, -a, ab);
                            }
                            else
                            {
                                simplice.Remove(b);
                                simplice.Remove(c);
                                dir = -a;
                            }
                        }
                        else
                        {
                            if (DirectionTest(abc, -a))
                            {
                                dir = trippleproduct(abc, -a, abc);
                            }
                            else
                            {
                                dir = trippleproduct(-abc, -a, -abc);
                            }
                        }
                    }
                     

                    #endregion
                }
                else 
                {
                    #region 4 points test the final test
                    Vector3 a = simplice[3];
                    Vector3 b = simplice[2];
                    Vector3 c = simplice[1];
                    Vector3 d = simplice[0];
                    Vector3 ac = c - a;
                    Vector3 ad = d - a;
                    Vector3 ab = b - a;
                    Vector3 bc = c - b;
                    Vector3 bd = d - b;

                    Vector3 acd = Vector3.Cross(ad, ac);
                    Vector3 abd = Vector3.Cross(ab, ad);
                    Vector3 abc = Vector3.Cross(ac, ab);

                   
                    if (DirectionTest(abc, -a))
                    {
                        if (DirectionTest(Vector3.Cross(abc, ac), -a))
                        {
                            simplice.Remove(b);
                            simplice.Remove(d);
                            dir = trippleproduct(ac, -a, ac);
                        }
                        else if (DirectionTest(Vector3.Cross(ab, abc), -a))
                        {
                            simplice.Remove(c);
                            simplice.Remove(d);
                            dir = trippleproduct(ab, -a, ab);
                        }
                        else
                        {
                            simplice.Remove(d);
                            dir = abc;
                        }
                    }
                    else if (DirectionTest(acd, -a))
                    {
                        if (DirectionTest(Vector3.Cross(acd, ad), -a))
                        {
                            simplice.Remove(b);
                            simplice.Remove(c);
                            dir = trippleproduct(ad, -a, ad);
                        }
                        else if (DirectionTest(Vector3.Cross(ac, acd), -a))
                        {
                            simplice.Remove(b);
                            simplice.Remove(d);
                            dir = trippleproduct( ac, -a, ac);
                        }
                        else
                        {
                            simplice.Remove(b);
                            dir = acd;
                        }
                    }
                    else if (DirectionTest(abd, -a))
                    {
                        if (DirectionTest(Vector3.Cross(abd, ab), -a))
                        {
                            simplice.Remove(c);
                            simplice.Remove(d);
                            dir = trippleproduct(ab, -a, ab);
                        }
                        else if (DirectionTest(Vector3.Cross(ad, abd), -a))
                        {
                            simplice.Remove(b);
                            simplice.Remove(c);
                            dir = trippleproduct(ad, -a, ad);
                        }
                        else
                        {
                            simplice.Remove(c);
                            dir = abd;
                        }
                   
                    }
                    else
                    {
                        return true;
                    }
                    #endregion
                }
            }
        }




        public static Vector3 trippleproduct(Vector3 a, Vector3 b, Vector3 c) 
        {
            //(A * B) * C = B(C.dot(A)) - a(C.dot(B))
            return (b * Vector3.Dot(c, a)) - (a * Vector3.Dot(c, b));
        }


        public static Vector3 SupportFunction(Hull one, Hull two, Vector3 direction)
        {
            // get furthest point for each hull along direction
            // get minkowsi difference.

            Vector3 furthest_for_one = one.GetFurthestPoint(direction);
            Vector3 furthest_for_two = two.GetFurthestPoint(-direction);

            return furthest_for_one - furthest_for_two;
        }

       
         public static bool DirectionTest( Vector3 vector, Vector3 otherVector)
         {
             //if doc of vectors > 0 then same direction
             return Vector3.Dot(vector, otherVector) > 0;
         }

    }
}
