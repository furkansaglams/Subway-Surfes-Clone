using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


[System.Serializable]

public enum SIDE
{
    LEFT = -18,
    MID = 0,
    RIGHT = 18
}

public enum HitX { LEFT, MID, RIGHT, NONE }
public enum HitY { UP, MID, DOWN, LOW, NONE }
public enum HitZ { FORWARD, MID, BACKWARD, NONE }
public class PlayerController : MonoBehaviour
{
    public SIDE m_Side;
    public SIDE lastSide;
    public HitX hitX;
    public HitY hitY;
    public HitZ hitZ;
    public float jumpPower;
    private float x;
    private float y;
    public float frwdSpeed;
    public float smoothSpeed;
    private float colHeight;
    private float colCenterY;
    public float stumbleTolerance = 10f;
    public float stumbletime;


    [HideInInspector]
    public bool swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool inJump;
    public bool inRoll;
    public bool stopAllState = false;
    public bool canInput = true;

    public Collider collisionCol;
    private CharacterController m_char;
    private Animator m_Animator;

    public Queue<GameObject> tileObjQueue = new Queue<GameObject>();

    public static PlayerController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    private void Start()
    {
        stumbletime = stumbleTolerance;

        m_Side = SIDE.MID;
        hitX = HitX.NONE;
        hitY = HitY.NONE;
        hitZ = HitZ.NONE;

        AddQueue();

        m_char = GetComponent<CharacterController>();
        colHeight = m_char.height;
        colCenterY = m_char.center.y;
        m_Animator = GetComponent<Animator>();

    }


    private void Update()
    {
        collisionCol.isTrigger = !canInput;
        if (!canInput)
        {
            m_char.Move(Vector3.down * 10f * Time.deltaTime);
            return;
        }

        swipeLeft = (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && canInput;
        swipeRight = (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && canInput;
        swipeUp = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canInput;
        swipeDown = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && canInput;

        if (swipeLeft)
        {
            if (m_Side == SIDE.MID)
            {
                lastSide = m_Side;
                m_Side = SIDE.LEFT;
                PlayAnimation("dodgeLeft");
            }
            else if (m_Side == SIDE.RIGHT)
            {
                lastSide = m_Side;
                m_Side = SIDE.MID;
                PlayAnimation("dodgeLeft");


            }
            else if (m_Side != lastSide)
            {
                lastSide = m_Side;
                PlayAnimation("stumbleOffLeft");
            }

        }
        else if (swipeRight)
        {
            if (m_Side == SIDE.MID)
            {
                lastSide = m_Side;
                m_Side = SIDE.RIGHT;
                PlayAnimation("dodgeRight");



            }
            else if (m_Side == SIDE.LEFT)
            {
                lastSide = m_Side;
                m_Side = SIDE.MID;
                PlayAnimation("dodgeRight");


            }
            else if (m_Side != lastSide)
            {
                lastSide = m_Side;
                PlayAnimation("stumbleOffRight");
            }

        }
        if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {

            m_Animator.SetLayerWeight(1, 0);
            stopAllState = false;
        }
        stumbletime = Mathf.MoveTowards(stumbletime, stumbleTolerance, Time.deltaTime);
        x = Mathf.Lerp(x, (int)m_Side, Time.deltaTime * smoothSpeed);
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, frwdSpeed * Time.deltaTime);
        m_char.Move(moveVector);
        Jump();
        Roll();

    }
    public void Jump()
    {

        if (m_char.isGrounded)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                PlayAnimation("Landing");
                inJump = false;
            }

            if (swipeUp)
            {
                y = jumpPower;
                PlayAnimation("Jump");
                inJump = true;

            }
        }
        else
        {
            y -= jumpPower * 2 * Time.deltaTime;
            if (m_char.velocity.y < -0.1f)
                PlayAnimation("Falling");
        }
    }

    internal float rollCounter;
    void Roll()
    {
        rollCounter -= Time.deltaTime;
        if (rollCounter <= 0f)
        {
            rollCounter = 0f;
            m_char.center = new Vector3(0, colCenterY, 0);
            m_char.height = colHeight;
            inRoll = false;
        }
        if (swipeDown)
        {
            rollCounter = 0.3f;
            y -= 10f;
            m_char.center = new Vector3(0, colCenterY / 2f, 0);
            m_char.height = colHeight / 2f;
            PlayAnimation("roll");
            inRoll = true;
            inJump = false;
        }
    }
    public IEnumerator DeathPlayer(string animName)
    {
        stopAllState = true;
        m_Animator.SetLayerWeight(1, 0);
        m_Animator.Play(animName);
        yield return new WaitForSeconds(0.2f);
        canInput = false;
        UIManager.instance.GameOver();

    }
    public void PlayAnimation(string animName)
    {
        if (stopAllState)
        {
            return;
        }
        m_Animator.Play(animName);
    }
    public void Stumble(string animName)
    {
        m_Animator.ForceStateNormalizedTime(0.0f);
        stopAllState = true;
        m_Animator.Play(animName);
        if (stumbletime < stumbleTolerance / 2f)
        {
            StartCoroutine(DeathPlayer("death_bounce"));
            return;
        }
        stumbletime -= 6f;
        ResetCollision();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("TileTrigger"))
        {
            AddQueue();
        }
        else if (other.CompareTag("DestroyTile"))
        {
            RemoveQueue();
        }
        else if (other.CompareTag("TileGenerate"))
        {
            other.GetComponent<TriggerBasedActivation>().GenerateTile();
        }
        else if (other.CompareTag("StartTile"))
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Coin"))
        {
            CoinManager.instance.AddCoin();
            other.gameObject.SetActive(false);
        }

    }

    public void OnCharacterColliderHit(Collider col)
    {
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);
        Debug.Log(hitX.ToString() + hitY.ToString() + hitZ.ToString());
        if (hitZ == HitZ.FORWARD && hitX == HitX.MID)
        {
            if (hitY == HitY.LOW)
            {
                Stumble("stumble_low");
            }
            else if (hitY == HitY.DOWN)
            {

                StartCoroutine(DeathPlayer("death_lower"));
            }
            else if (hitY == HitY.MID)
            {
                if (col.tag == "MovingObstacle")
                {
                    StartCoroutine(DeathPlayer("death_movingTrain"));

                }
                else if (col.tag != "RampObstacle")
                {
                    StartCoroutine(DeathPlayer("death_bounce"));
                }
            }
            else if (hitY == HitY.UP && !inRoll)
            {
                StartCoroutine(DeathPlayer("death_upper"));

            }
        }
        else if (hitZ == HitZ.MID)
        {
            if (hitX == HitX.RIGHT)
            {
                m_Side = lastSide;
                Stumble("stumbleSideRight");


            }
            else if (hitX == HitX.LEFT)
            {
                m_Side = lastSide;
                Stumble("stumbleSideLeft");

            }
        }
        else
        {
            if (hitX == HitX.RIGHT)
            {

                m_Animator.SetLayerWeight(1, 1);
                Stumble("stumbleCornerRight");

            }
            else if (hitX == HitX.LEFT)
            {
                m_Animator.SetLayerWeight(1, 1);
                Stumble("stumbleCornerLeft");

            }
        }

    }
    private void ResetCollision()
    {
        hitX = HitX.NONE;
        hitY = HitY.NONE;
        hitZ = HitZ.NONE;
    }
    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col_bounds.max.x, char_bounds.max.x);
        float avarage = (max_x + min_x) / 2f - col_bounds.min.x;
        HitX hit;
        if (avarage > col_bounds.size.x - 0.33f)
        {
            hit = HitX.RIGHT;
        }
        else if (avarage < 0.33f)
        {
            hit = HitX.LEFT;
        }
        else
        {
            hit = HitX.MID;
        }
        return hit;
    }
    public HitY GetHitY(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col_bounds.max.y, char_bounds.max.y);
        float avarage = ((min_y + max_y) / 2f - char_bounds.min.y) / char_bounds.size.y;
        HitY hit;
        if (avarage < 0.17f)
        {
            hit = HitY.LOW;
        }
        else if (avarage < 0.33f)
        {
            hit = HitY.DOWN;
        }
        else if (avarage < 0.66f)
        {
            hit = HitY.MID;
        }
        else
        {
            hit = HitY.UP;
        }
        return hit;
    }
    public HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = m_char.bounds;
        Bounds col_bounds = col.bounds;
        float min_z = Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Min(col_bounds.max.z, char_bounds.max.z);
        float avarage = ((min_z + max_z) / 2f - char_bounds.min.z) / char_bounds.size.z;
        HitZ hit;
        if (avarage < 0.33f)
        {
            hit = HitZ.BACKWARD;
        }
        else if (avarage < 0.66f)
        {
            hit = HitZ.MID;
        }
        else
        {
            hit = HitZ.FORWARD;
        }
        return hit;
    }

    public void AddQueue()
    {
        var tileObj = TileSpawnManager.instance.SpawnTile();
        tileObjQueue.Enqueue(tileObj);
    }
    public void RemoveQueue()
    {
        if (tileObjQueue.Count == 0) return;
        var tileObj = tileObjQueue.Dequeue();
        ObjectPool.instance.SetPoolObject(tileObj, 0);
        tileObj.GetComponent<TriggerBasedActivation>().UnGenerateTile();
    }



}


