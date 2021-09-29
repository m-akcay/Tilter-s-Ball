using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wall : MonoBehaviour
{
    public static readonly float THICKNESS = 0.025f;
    public static readonly float HOLE_SIZE = 0.2f;
    public static readonly float SPAWN_Z_DEFAULT = 8;
    public static float SPAWN_Z = 8;
    public static int LAYER = 8;

    public static ushort destroyedCount = 0;

    public Vector3 center { get; protected set; }

    public float size { get; protected set; }
    public bool showLevel { get; set; }

    protected bool gettingDestroyed;
    public int levelNo { get; protected set; }
    protected Vector3 destroyDirection;

    protected bool isMoving;
    protected Rigidbody rb;
    protected Material mat;
    public bool activated;

    public virtual void init(float top, float bottom, float left, float right, int levelNo, Transform parent)
    {
        void setUpRigidbody()
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.back;
            rb.mass = 0.05f;
            rb.isKinematic = true;
        }

        float xAvg = Util.avg(left, right);
        float yAvg = Util.avg(top, bottom);
        
        center = new Vector3(xAvg, yAvg, SPAWN_Z);
        Vector3 scale = new Vector3(right - left, top - bottom, THICKNESS);

        this.size = scale.x * scale.y;
        this.levelNo = levelNo;

        setUpGameObject(center, scale);
        setUpRigidbody();
        setDestroyDirection();

        isMoving = false;
        showLevel = false;
        gettingDestroyed = false;
        this.transform.parent = parent;
    }

    public void activate()
    {
        this.activated = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.back * (1 + this.levelNo * 0.01f);
    }

    public void deactivate()
    {
        this.activated = false;
        rb.isKinematic = true;
    }

    public virtual void setColor(Color color)
    {
        if (!mat)
            mat = Instantiate(Resources.Load("Materials/WallMaterial") as Material);
        mat.SetColor("Color_6655885E", color);
        if (showLevel)
        {
            mat.SetInt("Boolean_A04ECFCA", 1);
            mat.SetInt("Vector1_785B10BC", this.levelNo);
        }
        this.GetComponent<MeshRenderer>().material = mat;
    }

    private void Start()
    {
        
    }

    protected virtual void Update()
    {
        if (!this.activated)
            return;

        if (gettingDestroyed && (this.transform.position.z < 0 || this.transform.position.y < -1))
        {
            destroyWall_pt2();
        }
    }

    protected void FixedUpdate()
    {
        if (!this.activated)
            return;

        if (gettingDestroyed)
            moveThisWall();
    }

    protected virtual void setUpGameObject(Vector3 center, Vector3 scale)
    {
        this.gameObject.layer = Wall.LAYER;
        this.gameObject.transform.position = center;
        this.gameObject.transform.localScale = scale;
        this.gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
        this.gameObject.AddComponent<BoxCollider>();
        this.gameObject.name = "WALL_" + levelNo;
        this.gameObject.tag = "wall";
        this.activated = false;
    }
    protected abstract void setDestroyDirection();
    private void moveThisWall()
    {
        rb.AddForce(destroyDirection);
    }

    public void destroyWall()
    {
        gettingDestroyed = true;
        moveThisWall();
    }

    private void destroyWall_pt2()
    {
        this.transform.parent.GetComponent<Level>().removeWall(this);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        // to avoid leak
        Destroy(mat);
    }

}
