using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BallController : MonoBehaviour
{
    private int score;
    private int scoreFromThisLevel = 0;
    private Rigidbody rb;
    private bool jumping;
    private bool goingRight;
    private bool goingLeft;

    public bool collidedWithHole { get; private set; }
    private Material mat;
    private PostFXBinder postFXBinder;
    [SerializeField]
    private LevelManager lvlManager = null;
    private bool activated;

    public bool prefTest;
    [SerializeField]
    private TextMeshProUGUI scoreText = null;

    [SerializeField] private TextMeshProUGUI levelText = null;

    public int currentLevel { get { return lvlManager.currentLevel - 1; } }
    public bool updateLevelText { get; set; }

    public void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        jumping = false;
        goingLeft = false;
        goingRight = false;
        collidedWithHole = false;
        //lvlManager = GameObject.Find("GameManagerObject").GetComponent<LevelManager>();
        mat = this.GetComponent<Renderer>().material;
        postFXBinder = this.GetComponent<PostFXBinder>();
        activated = false;
        score = 0;
        scoreText.text = "";
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

            Shader.SetGlobalVector("_BallColor", mat.color.zeroAlpha());
            //Shader.SetGlobalColor("_Color", mat.color);

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (LevelManager.gameIsStopped)
            return;
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

                this.updateLevelText = true;
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
            postFXBinder.disableFX();

            this.levelText.text = "";
            this.levelText.gameObject.SetActive(false);

            StartCoroutine(lvlManager.endGame());

            if (PlayerPrefs.GetInt("highscore") < this.score)
            {
                scoreText.text = "NEW HIGH SCORE -> " + this.score + ("\nLEVEL -> ") + (lvlManager.currentLevel - 1);
                PlayerPrefs.SetInt("highscore", this.score);
                PlayerPrefs.SetInt("highscore_level", lvlManager.currentLevel - 1);
            }
            else
            {
                scoreText.text = "SCORE -> " + this.score + "\nlevel -> " + (lvlManager.currentLevel - 1) + "\n" +
                                 "HIGHEST SCORE -> " + PlayerPrefs.GetInt("highscore") + " - LEVEL " + PlayerPrefs.GetInt("highscore_level");
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
