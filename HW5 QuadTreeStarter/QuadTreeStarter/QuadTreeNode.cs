using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace QuadTreeStarter
{
    class QuadTreeNode
    {
        #region Constants
        // The maximum number of objects in a quad
        // before a subdivision occurs
        private const int MAX_OBJECTS_BEFORE_SUBDIVIDE = 3;
        #endregion

        #region Variables
        // The game objects held at this level of the tree
        private List<GameObject> _objects;

        // This quad's rectangle area
        private Rectangle _rect;

        // This quad's divisions
        private QuadTreeNode[] _divisions;
        #endregion

        #region Properties
        /// <summary>
        /// The divisions of this quad
        /// </summary>
        public QuadTreeNode[] Divisions { get { return _divisions; } }

        /// <summary>
        /// This quad's rectangle
        /// </summary>
        public Rectangle Rectangle { get { return _rect; } }

        /// <summary>
        /// The game objects inside this quad
        /// </summary>
        public List<GameObject> GameObjects { get { return _objects; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Quad Tree
        /// </summary>
        /// <param name="x">This quad's x position</param>
        /// <param name="y">This quad's y position</param>
        /// <param name="width">This quad's width</param>
        /// <param name="height">This quad's height</param>
        public QuadTreeNode(int x, int y, int width, int height)
        {
            // Save the rectangle
            _rect = new Rectangle(x, y, width, height);

            // Create the object list
            _objects = new List<GameObject>();

            // No divisions yet
            _divisions = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a game object to the quad.  If the quad has too many
        /// objects in it, and hasn't been divided already, it should
        /// be divided
        /// </summary>
        /// <param name="gameObj">The object to add</param>
        public void AddObject(GameObject gameObj)
        {
            // ADD YOUR CODE FOR ACTIVITY ONE HERE

            //If this node's rectangle contains the game object...
            if(Rectangle.Contains(gameObj.Rectangle) == true)
            {
                //...if GameObjects is less than the max AND this node hasn't been divided yet...
                if (GameObjects.Count < MAX_OBJECTS_BEFORE_SUBDIVIDE && Divisions == null)
                {
                    //...add the game object to this node's list of GameObjects (and produce a debugging message)
                    GameObjects.Add(gameObj);
                    Console.WriteLine($"A game object at {gameObj.Rectangle.X},{gameObj.Rectangle.Y} was added to a quad.");
                }
                //Otherwise...
                else
                {
                    //If this node hasn't been divided yet...
                    if (Divisions == null)
                    {
                        //...divide this node
                        Divide();
                    }

                    //Keep track of if the game object has been given a loving, quad-shaped home yet
                    bool hasBeenAdded = false;

                    //For every subdivision of this node...
                    foreach(QuadTreeNode q in Divisions)
                    {
                        //...check if the current subdivision contains the game object...
                        if (q.Rectangle.Contains(gameObj.Rectangle) == true)
                        {
                            //...and give it a loving quadrant-shaped home. Also, mark that the game object now has a home.
                            q.AddObject(gameObj);
                            hasBeenAdded = true;
                        }
                    }
                    //If the game object doesn't fit into any of the subdivisions (as indicated by the bool)...
                    if(hasBeenAdded == false)
                    {
                        //...add it to this node's GameObjects, because it still fits in this one! Also, produce a debugging message.
                        GameObjects.Add(gameObj);
                        Console.WriteLine($"A game object at {gameObj.Rectangle.X},{gameObj.Rectangle.Y} was added to a quad.");
                    }
                }
            }
        }

        /// <summary>
        /// Divides this quad into 4 smaller quads.  Moves any game objects
        /// that are completely contained within the new smaller quads into
        /// those quads and removes them from this one.
        /// </summary>
        public void Divide()
        {
            // ADD YOUR CODE FOR ACTIVITY ONE HERE

            //Even though the code block where this method is called is not executed unless this node hasn't been divided, check it again anyway
            //Can't be too careful, right?
            if(Divisions == null)
            {
                //Create a container to keep track of the subdivisions
                _divisions = new QuadTreeNode[4];

                //Generate the new subdivisions and add them into the array in one fell swoop
                _divisions[0] = new QuadTreeNode(Rectangle.X, Rectangle.Y, Rectangle.Width / 2, Rectangle.Height / 2);
                _divisions[1] = new QuadTreeNode(Rectangle.X + Rectangle.Width / 2, Rectangle.Y, Rectangle.Width / 2, Rectangle.Height / 2);
                _divisions[2] = new QuadTreeNode(Rectangle.X, Rectangle.Y + Rectangle.Height / 2, Rectangle.Width / 2, Rectangle.Height / 2);
                _divisions[3] = new QuadTreeNode(Rectangle.X + Rectangle.Width / 2, Rectangle.Y + Rectangle.Height / 2, Rectangle.Width / 2, Rectangle.Height / 2);

                //Also produce a particularly nonspecific debugging message
                Console.WriteLine("A quad was divided.");

                //For every subdivision of this node...
                foreach(QuadTreeNode q in Divisions)
                {
                    //...start with a clean, temporary list to hold game objects that need to be moved out of this node's list of GameObjects
                    //This allows us to still use a foreach loop on this node's list of GameObjects, since you can't modify a list while a foreach is being run on it
                    List<GameObject> toBeRemoved = new List<GameObject>();

                    //...for every GameObject currently contained in this node...
                    foreach (GameObject g in GameObjects)
                    {
                        //...check if the current subdivision contains the current GameObject...
                        if(q.Rectangle.Contains(g.Rectangle) == true)
                        {
                            //...and add the GameObject to the subdivisions list if true
                            q.GameObjects.Add(g);

                            //Also mark that GameObject to be removed from this node's list and produce a debugging message
                            toBeRemoved.Add(g);
                            Console.WriteLine($"A {g.Color} GameObject at {q.Rectangle.X},{q.Rectangle.Y} was moved into a subdivision.");
                        }
                    }

                    //...for every GameObject that was marked in the temporary list...
                    foreach(GameObject gR in toBeRemoved)
                    {
                        //...remove that GameObject from this node's list of GameObjects
                        GameObjects.Remove(gR);
                    }
                    //Afterwards, clean out the temporary list just to be safe.
                    toBeRemoved.Clear();
                }
            }
        }

        /// <summary>
        /// Recursively populates a list with all of the rectangles in this
        /// quad and any subdivision quads.  Use the "AddRange" method of
        /// the list class to add the elements from one list to another.
        /// </summary>
        /// <returns>A list of rectangles</returns>
        public List<Rectangle> GetAllRectangles()
        {
            List<Rectangle> rects = new List<Rectangle>();
            // ADD YOUR CODE FOR ACTIVITY TWO HERE

            //Start by adding this node's rectangle to the list
            rects.Add(this.Rectangle);

            //Now, if this node has been divided...
            if (Divisions != null)
            {
                //...for every subdivision of this node...
                foreach (QuadTreeNode q in Divisions)
                {
                    //...fetch the rectangle of the current subdivision and all the subdivision's subdivisions, then add it on to this node's list of Rectangles
                    //Recursion be like "Don't talk to me or my son or my son's son or my son's son's son or my O(n)*son's son ever again"
                    rects.AddRange(q.GetAllRectangles());
                }
            }

            //Pass this iteration's list of rectangles back to the previous iteration or the original method call
            return rects;
        }

        /// <summary>
        /// A possibly recursive method that returns the
        /// smallest quad that contains the specified rectangle
        /// </summary>
        /// <param name="rect">The rectangle to check</param>
        /// <returns>The smallest quad that contains the rectangle</returns>
        public QuadTreeNode GetContainingQuad(Rectangle rect)
        {
            // ADD YOUR CODE FOR ACTIVITY TWO HERE

            //If this node's rectangle contains the passed-in rectangle...
            if (this.Rectangle.Contains(rect) == true)
            {
                //...and if this node has been divided...
                if(Divisions != null)
                {
                    //...for every subdivision of this node...
                    foreach(QuadTreeNode q in Divisions)
                    {
                        //...check if the current subdivision contains the passed-in rectangle...
                        if(q.Rectangle.Contains(rect) == true)
                        {
                            //...and recursively fetch the next iteration
                            return q.GetContainingQuad(rect);
                        }
                    }
                    //Should none of the subdivisions contain the passed-in rectangle, return this node
                    return this;
                }
                //...otherwise...
                else
                {
                    //return this node
                    return this;
                }
            }
            // Return null if this quad doesn't completely contain
            // the rectangle that was passed in
            else
            {
                return null;
            }
        }
        #endregion
    }
}
