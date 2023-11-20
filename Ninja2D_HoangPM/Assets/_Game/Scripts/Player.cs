using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;

    private bool isGrounded = true;
    private bool isJumping = false ;
    private bool isAttack = false;
    private bool isDead = false;
    private bool isCanMove = true;

    private float horizontal;

    private int Coins = 0;

    private Vector3 savePoint;

    //private void Awake()
    //{
    //    Coins = PlayerPrefs.GetInt("Coins", 0);
    //}

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

        isGrounded = CheckGrounded();
       
        //horizontal = Input.GetAxisRaw("Horizontal"); // -1 -> 0 -> 1
        //verticle = Input.GetAxisRaw("verticle"); di chuyen chieu doc (len, xuong)

        if (isAttack )
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.K))
            {
                Jump();
            }

            //change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.J) && isGrounded)
            {
                Attack();
            }
            //throw
            if (Input.GetKeyDown(KeyCode.L) && isGrounded)
            {
                Throw();
            }

        }
        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }

        //Moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            //horizontal > 0 -> tra ve 0, neu horizontal <= 0 -> tra ve 180
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        //idle
        else if (isGrounded)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;
        isDead = false;
        transform.position = savePoint;
        ChangeAnim("idle");
        DeActiveAttack();
        SavePoint();
        UIManager.instance.SetCoins(Coins);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        
        base.OnDeath();
        isDead = true;
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

        //return hit.collider != null;
    }

    public void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);
    }

    public void Throw()
    {
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);

        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }

    public void ResetAttack()
    {
        isAttack = false;
        ChangeAnim("idle");
    }

    public void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;
            ChangeAnim("jump");
            rb.AddForce(jumpForce * Vector2.up);
        }
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coins")
        {
            Coins++;
            //PlayerPrefs.SetInt("Coins", Coins);
            UIManager.instance.SetCoins(Coins);

            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }
}
