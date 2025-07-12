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
    [SerializeField] private bool _isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] private bool dashRight;

    #endregion

    public Animator bodyAnimator;
    public SpriteRenderer bodySpriteRenderer;

    public event Action OnDespawn;
    public event Action<float> OnLifeUpdate;

    [Networked] private NetworkBool _canMove { get; set; } = false;

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
        HP = MaxHP;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!GetInput(out NetworkInputData data)) return;

        if (_isDashing)
        {

        }
        else
        {
            //Movement
            var xVel = data.inputDir * Speed * 100 * Time.fixedDeltaTime;
            Vector2 targetVelocity = new Vector2(xVel, rb.velocity.y);
            rb.velocity = targetVelocity;

            //Jump
            if (isGrounded && isJumping)
            {
                isJumping = true;
                jumpTime = JumpStartTime;

                rb.velocity = Vector2.up * jumpForce;
            }
        }

        if (data.inputDir == 1)
        {
            dashRight = true;
            bodySpriteRenderer.flipX = false;
        }
        else if (data.inputDir == -1)
        {
            dashRight = false;
            bodySpriteRenderer.flipX = true;
        }

        //Dash
        if (data.isDashing && canDash)
        {
            StartCoroutine(Dash());
        }

        bodyAnimator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

#region Funciones Movimiento

    private IEnumerator Dash()
    {
        canDash = false;
        _isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if (dashRight)
        {
            rb.AddForce(new Vector2(transform.localScale.x * dashingPower, 0f));
        }
        else if (!dashRight)
        {
            rb.AddForce(new Vector2(-transform.localScale.x * dashingPower, 0f));
        }

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        _isDashing = false;

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

    public void SetCanMove(bool can)
    {
        _canMove = can;
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
