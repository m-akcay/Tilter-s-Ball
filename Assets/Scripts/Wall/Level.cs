using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private class Limits
    {
        public static readonly float TOP_default = 0.0f;
        public static readonly float BOTTOM_default = -1.0f;
        public static readonly float LEFT_default = 0.0f;
        public static readonly float RIGHT_default = 1.0f;
    }

    public List<Wall> walls { get; private set; }
    public Wall largestWall { get; }
    public int levelNo { get; private set; }

    private GameObject hole;
    public Level init(int levelNo)
    {
        this.gameObject.layer = Wall.LAYER;

        float randX = Random.Range(0.25f, 0.75f);
        float randY = Random.Range(-0.25f, -0.75f);

        walls = new List<Wall>();

        walls.Add(createUpWall(randY, levelNo));
        walls.Add(createRightWall(randX, randY, levelNo));
        walls.Add(createBottomLeftWall(randX, randY, levelNo));
        walls.Add(createLeftMidWall(randX, randY, levelNo));

        setLargestWall(walls);

        walls.Add(createHole(randX, randY, levelNo));

        this.levelNo = levelNo;
        return this;
    }

    public void setColor(Color color)
    {
        this.walls.ForEach(wall => wall.setColor(color));
    }
    public void activate()
    {
        this.walls.ForEach(wall => wall.activate());
    }
    public void setColorAndActivateLevel(Color color)
    {
        this.walls.ForEach(wall =>
        {
            wall.setColor(color);
            wall.activate();
        });
    }
    private Wall createUpWall(float randY, int level)
    {
        float bottom = Limits.TOP_default + (randY + Wall.HOLE_SIZE);
        Wall wall = GameObject.CreatePrimitive(PrimitiveType.Cube)
            .AddComponent<UpWall>();
        wall.init(Limits.TOP_default, bottom, Limits.LEFT_default, Limits.RIGHT_default, level, this.gameObject.transform);
        return wall;
    }

    private Wall createRightWall(float randX, float randY, int level)
    {
        float left = randX + Wall.HOLE_SIZE;
        float top = randY + Wall.HOLE_SIZE;
        Wall wall = GameObject.CreatePrimitive(PrimitiveType.Cube)
            .AddComponent<RightWall>();
        wall.init(top, Limits.BOTTOM_default, left, Limits.RIGHT_default, level, this.gameObject.transform);
        return wall;
    }

    private Wall createBottomLeftWall(float randX, float randY, int level)
    {
        float top = randY - Wall.HOLE_SIZE;
        float right = randX + Wall.HOLE_SIZE;
        Wall wall = GameObject.CreatePrimitive(PrimitiveType.Cube)
             .AddComponent<BottomLeftWall>();
        wall.init(top, Limits.BOTTOM_default, Limits.LEFT_default, right, level, this.gameObject.transform);
        return wall;
    }

    private Wall createLeftMidWall(float randX, float randY, int level)
    {
        float top = randY + Wall.HOLE_SIZE;
        float bottom = randY - Wall.HOLE_SIZE;
        float right = randX - Wall.HOLE_SIZE;
        Wall wall = GameObject.CreatePrimitive(PrimitiveType.Cube)
             .AddComponent<MidLeftWall>();
        wall.init(top, bottom, Limits.LEFT_default, right, level, this.gameObject.transform);
        return wall;
    }

    private Wall createHole(float randX, float randY, int level)
    {
        float top = randY + Wall.HOLE_SIZE;
        float bottom = randY - Wall.HOLE_SIZE;
        float left = randX - Wall.HOLE_SIZE;
        float right = randX + Wall.HOLE_SIZE;
        Wall hole = GameObject.CreatePrimitive(PrimitiveType.Cube)
            .AddComponent<Hole>();
        hole.init(top, bottom, left, right, level, this.gameObject.transform);
        return hole;
    }
    private void setLargestWall(List<Wall> walls)
    {
        float maxSize = 0;
        int maxIdx = 0;
        for (int i = 0; i < 4; i++)
        {
            Wall wall = walls[i];
            if (wall.size > maxSize)
            {
                maxSize = wall.size;
                maxIdx = i;
            }
        }

        walls[maxIdx].showLevel = true;
    }

    public void removeWall(Wall wall)
    {
        this.walls.Remove(wall);
    }
    public void destroy()
    {
        this.walls.ForEach(wall =>
        {
            wall.destroyWall();
        });
    }

    private void Update()
    {
        if (this.walls.Count == 0)
            Destroy(this.gameObject);
    }
}
