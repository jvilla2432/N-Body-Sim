namespace N_Body_Sim_Backend
{
    public class DynamicOrthtree
    {
        public static double maxThreshold = 0.5;

        private static int dimensions = 3;

        private Dictionary<uint, DynamicOrthtree> children = new();

        private double[] bounds;

        private double[] node;

        private double[] centerOfMass = new double[dimensions + 1];


        //Generalization to N-Dimensions Basically Im Binary
        // For an N dimensional Ntree 
        // Each dimension is one digit in a binary number
        // The first dimension is the first digit
        // EX: In 4 dimensions, 1010 is the Ntrant in the second half of the 1st, first half of the second ,etc...
        // To find quadrant from coords -> Modulo
        // To find coords from quadrant -> Trivial
        // Quadrants are indexed by this number. 
        // This allows for 2^N quadrants per Ntree

        /// <summary>
        /// Creates new Octree
        /// </summary>
        /// <param name="octBounds">Bounds of octTree. Format: xmin,xmax,ymin,ymax,zmin,zmax(inclusive)</param>
        /// <exception cref="ArgumentException"></exception>
        public DynamicOrthtree(double[] octBounds, double[] startNode)
        {
            if (octBounds.Length != 2 * dimensions || startNode.Length != (1 + 3 * dimensions))
            {
                throw new ArgumentException("Invalidinput dimensions");
            }
            bounds = octBounds;
            node = startNode;
            for (int i = 0; i < dimensions + 1; i++)
            {
                centerOfMass[i] = node[i];
            }
        }

        static bool checkBounds(double[] bounds, double[] point)
        {
            for (int i = 0; i < dimensions; i++)
            {
                if (point[1 + i] < bounds[i * 2])
                {
                    return false;
                }
                if (point[1 + i] < bounds[1 + (i * 2)])
                {
                    return false;
                }
            }
            return true;
        }

        public int CheckSubdivision(double[] point)
        {
            uint result = 0;
            for (int i = 0; i < dimensions; i++)
            {
                double middle = bounds[i * 2] + bounds[1 + (i * 2)];
                middle = middle * 0.5;
                if (point[i + 1] > middle)
                {
                    result = result & (uint)(1 << i);
                }
            }
            //result is now subdivison
            return (int)result;
            throw new ArgumentException("point not in bounds");
        }


        /// <summary>
        /// Recalculates the center of mass of this node
        /// </summary>
        public void calculateMass()
        {
            double total_mass = centerOfMass[3];
            double x_mass = centerOfMass[3] * centerOfMass[0];
            double y_mass = centerOfMass[3] * centerOfMass[1];
            double z_mass = centerOfMass[3] * centerOfMass[2];
            for (int i = 0; i < 8; i++)
            {
                if (children[i] != null)
                {
                    total_mass += children[i].centerOfMass[3];
                    x_mass += children[i].centerOfMass[0] * children[i].centerOfMass[3];
                    y_mass += children[i].centerOfMass[1] * children[i].centerOfMass[3];
                    z_mass += children[i].centerOfMass[2] * children[i].centerOfMass[3];
                }
            }
            centerOfMass[0] = x_mass / total_mass;
            centerOfMass[1] = y_mass / total_mass;
            centerOfMass[2] = z_mass / total_mass;
            centerOfMass[3] = total_mass;

        }

        public double[] calculateForce(double[] forceNode)
        {
            if (node != null)
            {
                return NBody.calculateForce(forceNode, node);
            }
            else
            {
                double dx = centerOfMass[0] - node[0];
                double dy = centerOfMass[1] - node[1];
                double dz = centerOfMass[2] - node[2];
                double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                double threshold = (bounds[1] - bounds[0]) / dist;
                if (threshold < maxThreshold)
                {
                    //calculate...
                    return NBody.calculateForce(forceNode, centerOfMass);
                }
                else
                {
                    //recurse
                    double[] totalForce = { 0, 0, 0, 0 };
                    for (int i = 0; i < 8; i++)
                    {
                        Orthtree? childNode = children[i];
                        if (childNode != null)
                        {
                            double[] childForce = childNode.calculateForce(forceNode);
                            totalForce[0] += childForce[0];
                            totalForce[1] += childForce[1];
                            totalForce[2] += childForce[2];
                            totalForce[3] += childForce[3];
                        }
                    }
                    return totalForce;
                }
            }
        }

        /// <summary>
        /// Adds a node to the Octtree
        /// </summary>
        /// <param name="newNode">Node to add</param>
        /// <exception cref="ArgumentException"></exception>
        public void InsertNode(double[] newNode)
        {
            if (newNode.Length != 3)
            {
                throw new ArgumentException("Requires x,y,z input");
            }
            //If current Octree has no node
            if (node == null)
            {
                node = newNode;
                calculateMass();
            }
            else
            {
                //If Octree has children
                if (node == null)
                {
                    //Insert into children
                    int octrant = CheckSubdivision(newNode);
                    if (children[octrant] == null)
                    {
                        children[octrant] = new Octree(subBounds[octrant]);
                    }
                    else
                    {
                        children[octrant].InsertNode(newNode);
                    }
                    calculateMass();
                }
                else //If octree does not have children
                {
                    //Subdivide
                    int octrant1 = CheckSubdivision(node);
                    int octrant2 = CheckSubdivision(newNode);
                    if (children[octrant1] == null)
                    {
                        children[octrant1] = new Octree(subBounds[octrant1]);
                    }
                    if (children[octrant2] == null)
                    {
                        children[octrant2] = new Octree(subBounds[octrant2]);
                    }
                    children[octrant1].InsertNode(newNode);
                    children[octrant2].InsertNode(newNode);
                    node = null;
                    calculateMass();
                }
            }
        }
    }
}
