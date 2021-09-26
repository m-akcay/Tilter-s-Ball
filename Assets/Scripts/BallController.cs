using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    private Material mat;
    private PostFXBinder postFXBinder;
    private LevelManager lvlManager;
    private bool activated;

    public bool prefTest;
    public Text scoreText;

    private void Start()
    {
        Physics.gravity *= 0.5f;
        rb = this.GetComponent<Rigidbody>();
        jumping = false;
        goingLeft = false;
        goingRight = false;
        collidedWithHole = false;
        lvlManager = GameObject.Find("GameManagerObject").GetComponent<LevelManager>();
        mat = this.GetComponent<Renderer>().material;
        postFXBinder = this.GetComponent<PostFXBinder>();
        activated = false;
    }

    private void Update()
    {
        if (!this.activated)
            return;
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
        if (!this.activated)
            return;

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
                hole.transform.parent.GetComponent<Level>().destroy();

                lvlManager.levelUp();

                collidedWithHole = true;
                Debug.Log(this.score);
                postFXBinder.disableFX();
            }
        }
        
        if (collisionObj.tag == "wall")
        {
            postFXBinder.bindBlurPosAndSpd(collision.GetContact(0).point, 
                                            collision.relativeVelocity.magnitude);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "finisher")
        {
            lvlManager.endGame();

            if (PlayerPrefs.GetInt("highscore") < this.score)
            {
                scoreText.text = "new high score -> " + this.score + ("\nlevel -> ") + (lvlManager.currentLevel - 1);
                PlayerPrefs.SetInt("highscore", this.score);
                PlayerPrefs.SetInt("highscore_level", lvlManager.currentLevel - 1);
            }
            else
            {
                scoreText.text = "score -> " + this.score + "\nlevel -> " + (lvlManager.currentLevel - 1) + "\n" +
                                 "highest score-> " + PlayerPrefs.GetInt("highscore") + " - level " + PlayerPrefs.GetInt("highscore_level");
            }
        }
    }
    public void activate()
    {
        this.activated = true;
        this.rb.isKinematic = false;
    }
    public void deactivate()
    {
        this.activated = false;
        this.rb.isKinematic = true;
    }
    private void OnDestroy()
    {
        Destroy(this.mat);
    }
}
