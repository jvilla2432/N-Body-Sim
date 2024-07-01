namespace N_Body_Sim_Backend
{
    public class Orthtree
    {
        // See http://arborjs.org/docs/barnes-hut

        public static double maxThreshold = 0.5;

        private static int dimensions = 3;

        private static int num_children = (int)Math.Pow(2, dimensions);

        private Orthtree?[] children = new Orthtree?[num_children];

        private double[] bounds;

        private double[] node;

        private double[] centerOfMass = new double[dimensions+1];


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
        public Orthtree(double[] octBounds, double[] startNode)
        {
            if(octBounds.Length != 2*dimensions || startNode.Length != (1+3*dimensions))
            {
                throw new ArgumentException("Invalidinput dimensions");
            }
            bounds = octBounds;
            node = startNode;
            for(int i = 0; i < dimensions + 1; i++)
            {
                centerOfMass[i] = node[i];
            }
        }

        static bool checkBounds(double[] bounds, double[] point)
        {
            for(int i = 0; i < dimensions; i++)
            {
                if(point[1+i] < bounds[i*2])
                {
                    return false;
                }
                if (point[1 + i] < bounds[1+(i*2)])
                {
                    return false;
                }
            }
            return true;
        }

        public int CheckSubdivision(double[] point)
        {
            uint result = 0;
            for(int i = 0; i < dimensions; i++)
            {
                double middle = bounds[i * 2] + bounds[1 + (i * 2)];
                middle = middle * 0.5;
                if(point[i+1] > middle)
                {
                    result = result & (uint)(1 << i);
                }
            }
            //result is now subdivison
            return (int)result;
        }


        private double[] subdivToBound(uint subdiv)
        {
            double[] subBounds = new double[dimensions * 2];
            for(int i = 0;i < dimensions; i++)
            {
                uint test = subdiv | (uint)(i << dimensions);
                if(test == 0)
                {
                    subBounds[i * 2] = bounds[i];
                    subBounds[i * 2 + 1] = bounds[i + 1]/2;
                }
                else
                {
                    subBounds[i * 2] = bounds[i + 1] / 2;
                    subBounds[i * 2 + 1] = bounds[i + 1];
                }
            }
            return subBounds;
        }

        /// <summary>
        /// Recalculates the center of mass of this node
        /// </summary>
        public void calculateMass()
        {
            double total_mass = centerOfMass[0]; 
            double[] masses = new double[dimensions];
            for(int i = 0; i < dimensions; i++)
            {
                masses[i] = centerOfMass[0] * centerOfMass[i + 1];
            }
            for (int i = 0; i < num_children; i++){
                Orthtree? child = children[i];
                if(child != null)
                {
                    total_mass += child.centerOfMass[0];
                    for(int j = 0; j < dimensions; j++)
                    {
                        masses[j] += child.centerOfMass[j + 1] * child.centerOfMass[0];
                    }
                }
            }
            centerOfMass[0] = total_mass;
            for (int i = 0; i < dimensions; i++)
            {
                centerOfMass[i + 1] = masses[i] / total_mass;
            }
        }

        public double[] calculateForce(double[] forceNode)
        {
            if(node != null)
            {
                return NBody.calculateForce(forceNode, node);
            }
            else
            {
                double sum = 0;
                for(int i = 0; i < dimensions; i++)
                {
                    sum += centerOfMass[i + 1] - forceNode[i + 1];
                }
                double dist = Math.Sqrt(sum);
                double threshold = (bounds[1]-bounds[0]) / dist;
                if(threshold < maxThreshold)
                {
                    //calculate...
                    return NBody.calculateForce(forceNode, centerOfMass);
                }
                else
                {
                    //recurse
                    double[] totalForce = new double[dimensions];
                    for(int i = 0; i < num_children; i++)
                    {
                        Orthtree? childNode = children[i];
                        if(childNode != null)
                        {
                            double[] childForce = childNode.calculateForce(forceNode);
                            for(int j = 0; j < childForce.Length; j++)
                            {
                                totalForce[j] += childForce[j];
                            }
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
            if (newNode.Length != dimensions)
            {
                throw new ArgumentException("Incorrect dimension");
            }
            //If current Octree has no node
            if (node == null)
            {
                node = newNode;
                calculateMass();
            }
            else
            {
                //insert node and node2
                int subdiv1 = CheckSubdivision(newNode);
                int subdiv2 = CheckSubdivision(node);
                if(children[subdiv1] == null)
                {
                    children[subdiv1] = new Orthtree(subdivToBound((uint)subdiv1), newNode);
                }
                else
                {
                    children[subdiv1].InsertNode(newNode);
                }
                if(children[subdiv2] == null)
                {
                    children[subdiv2] = new Orthtree(subdivToBound((uint)subdiv2), newNode);
                }
                else
                {
                    children[subdiv2].InsertNode(node);
                }
                calculateMass();
            }
        }
    }
}
