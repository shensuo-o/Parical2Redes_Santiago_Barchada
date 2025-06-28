using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using System;

public class Player : NetworkBehaviour
{
    //stats
    [SerializeField] [Networked, OnChangedRender(nameof(UpdateLifeFeedback))] float HP { get; set; }
    [SerializeField] public float MaxHP;

#region Variables Movimiento

    //variables para el movimiento
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public float HorizontalInput;
    [SerializeField] private float Speed;

    //variables para el salto
    [SerializeField] private float jumpForce;
    [SerializeField] private float JumpStartTime;
    [SerializeField] private float jumpTime;
    [SerializeField] public bool isJumping;
    [SerializeField] private bool isGrounded;

    //variables para el dash
    [SerializeField] private bool canDash;
    [SerializeField] private bool startDash;
    [SerializeField] private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] private bool dashRight;

    #endregion

    public Animator bodyAnimator;
    public SpriteRenderer bodySpriteRenderer;

    public event Action OnDespawn;
    public event Action<float> OnLifeUpdate;
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyAnimator = GetComponentInChildren<Animator>();
        bodySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (HasStateAuthority)
        {
            Camera.main.GetComponent<CameraFollow>()?.SetTarget(transform);
        }
        GameManager.Instance.AddToList(this);
        HPbarManager.Instance.CreateLifeBar(this);
        HP = MaxHP;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        HorizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            startDash = true;
        }      
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (isDashing)
        {
            return;
        }

        Jump();

        if (HorizontalInput == 1)
        {
            dashRight = true;
            bodySpriteRenderer.flipX = false;
        }
        else if (HorizontalInput == -1)
        {
            dashRight = false;
            bodySpriteRenderer.flipX = true;
        }

        if (startDash && canDash)
        {
            StartCoroutine(Dash());
        }

        Movement(HorizontalInput);
        bodyAnimator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

#region Funciones Movimiento

    private void Movement(float dir)//Toma la variable de direccion y la usa para moverse con velocity del rigidbody.
    {
        var xVel = dir * Speed * 100 * Time.fixedDeltaTime;
        Vector2 targetVelocity = new Vector2(xVel, rb.velocity.y);
        rb.velocity = targetVelocity;
    }

    private void Jump()//Salto que se hace mas alto contra mas se sostiene apretado el boton.
    {
        if (isGrounded && isJumping)
        {
            isJumping = true;
            jumpTime = JumpStartTime;

            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTime > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTime -= Runner.DeltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if (dashRight)
        {
            //rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            rb.AddForce(new Vector2(transform.localScale.x * dashingPower, 0f));
        }
        else if (!dashRight)
        {
            //rb.velocity = new Vector2(-transform.localScale.x * dashingPower, 0f);
            rb.AddForce(new Vector2(-transform.localScale.x * dashingPower, 0f));
        }

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
        startDash = false;
    }

    #endregion

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(float dmg)
    {
        HP -= dmg;

        if (HP <= 0f)
        {
            Death();
        }
    }

    public void Death()
    {
        bodyAnimator.SetBool("isDead", true);
        GameManager.Instance.RPC_Defeat(Runner.LocalPlayer);
        Runner.Despawn(Object);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnDespawn?.Invoke();
    }

    void UpdateLifeFeedback()
    {
        OnLifeUpdate(HP / MaxHP);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isGrounded = false;
        }
    }
}
