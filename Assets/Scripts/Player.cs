using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField][Networked, OnChangedRender(nameof(UpdateLifeFeedback))] float HP { get; set; }
    [SerializeField] public float MaxHP;

    [Header("Movimiento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float Speed;
    [SerializeField] private float jumpForce;

    [Header("Dash")]
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] private bool dashRight;

    private bool isGrounded;

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

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (isDashing) return;

        if (!GetInput(out NetworkInputData input)) return;

        // Movimiento horizontal
        float horizontalInput = input.movementInput;
        Move(horizontalInput);

        // Salto
        if (input.networkButtons.IsSet(MyButtons.Jump) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Dash
        if (input.networkButtons.IsSet(MyButtons.Dash) && canDash)
        {
            dashRight = horizontalInput >= 0;
            StartCoroutine(Dash());
        }

        // Flip sprite
        if (horizontalInput > 0.1f)
            bodySpriteRenderer.flipX = false;
        else if (horizontalInput < -0.1f)
            bodySpriteRenderer.flipX = true;

        bodyAnimator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    private void Move(float dir)
    {
        float xVel = dir * Speed * 100 * Time.fixedDeltaTime;
        Vector2 targetVelocity = new Vector2(xVel, rb.velocity.y);
        rb.velocity = targetVelocity;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        Vector2 force = dashRight ? Vector2.right : Vector2.left;
        rb.AddForce(force * dashingPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(float dmg)
    {
        HP -= dmg;
        if (HP <= 0f)
            Death();
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
        OnLifeUpdate?.Invoke(HP / MaxHP);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) // caída al vacío
        {
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) // suelo
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