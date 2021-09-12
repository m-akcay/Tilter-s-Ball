using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class BallController : MonoBehaviour
{
    private int score = 0;
    private int scoreFromThisLevel = 0;
    private Rigidbody rb;
    private bool jumping;
    private bool goingRight;
    private bool goingLeft;

    private bool collidedWithHole;
    private bool vfxRunning;
    private VisualEffect vfx;
    private Material mat;
    private Material postProcessMaterial;

    private LevelManager lvlManager;
    private void Start()
    {
        Physics.gravity *= 0.5f;
        rb = this.GetComponent<Rigidbody>();
        jumping = false;
        goingLeft = false;
        goingRight = false;
        collidedWithHole = false;
        vfxRunning = false;
        lvlManager = GameObject.Find("GameManagerObject").GetComponent<LevelManager>();
        mat = this.GetComponent<Renderer>().material;
        postProcessMaterial = GameObject.Find("RenderedQuad").GetComponent<Renderer>().material;
        //vfx = this.gameObject.GetComponent<VisualEffect>();
    }

    private void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumping = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            goingLeft = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            goingRight = true;
        }

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                jumping = true;
            }
        }
        
        if (Input.acceleration.x > 0)
        {
            goingRight = true;
        }
        else
        {
            goingLeft = true;
        }
#endif
        if (collidedWithHole)
        {
            collidedWithHole = false;
            vfxRunning = true;

            switch (scoreFromThisLevel)
            {
                default:
                    break;
                case 1:
                    mat.color = Color.white;
                    break;
                case 2:
                    mat.color = Color.green;
                    break;
                case 3:
                    mat.color = new Color(0.719f, 0, 1);
                    break;
            }

        }

    }

    private void FixedUpdate()
    {
        if (jumping)
        {
            jumping = false;
            rb.AddForce(Vector3.up * 1000 * Time.deltaTime);
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (goingLeft)
        {
            goingLeft = false;
            rb.AddForce(Vector3.left * 50 * Time.deltaTime);
        }
        else if (goingRight)
        {
            goingRight = false;
            rb.AddForce(Vector3.right * 50 * Time.deltaTime);
        }
#elif UNITY_ANDROID
        if (goingLeft)
        {
            rb.AddForce(Vector3.right * 75 * Time.deltaTime * Input.acceleration.x);
        }

#endif
        //if (Input.GetKeyDown(KeyCode.K))
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObj = collision.gameObject;
        if (collisionObj.tag == "hole")
        {
            Hole hole = collisionObj.GetComponent<Hole>();
           
            // avoiding multiple collisions to the same hole
            if (hole.levelNo == lvlManager.currentLevel)
            {
                Vector2 contactPt = collision.GetContact(0).point;
                this.scoreFromThisLevel = hole.calculateScore(contactPt);
                this.score += scoreFromThisLevel;
                Level level = hole.transform.parent.GetComponent<Level>();
                level.destroy();
                lvlManager.levelUp();

                collidedWithHole = true;
                Debug.Log(this.score);
                postProcessMaterial.SetInt("_ShouldBlur", 0);
            }
        }
        
        if (collisionObj.tag == "wall")
        {
            Vector4 contactPt = collision.GetContact(0).point;
            contactPt.w = collision.relativeVelocity.magnitude;
            postProcessMaterial.SetInt("_ShouldBlur", 1);
            postProcessMaterial.SetVector("_CollisionWithWall_WS", contactPt);
        }
    }

    private void OnDestroy()
    {
        Destroy(this.mat);
        Destroy(this.postProcessMaterial);
    }
}
