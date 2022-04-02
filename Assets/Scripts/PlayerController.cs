using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Utils;
using Utils.Dependency;
using UnityEngine;
using UnityEngine.UI;
using static Facade;

public class PlayerController : Singleton<PlayerController>
{
	[Header("Movement")]
	[SerializeField] private float moveSpeed = 10f;
	[SerializeField] private float jumpForce = 400f;
	[SerializeField, FloatRangeSlider(0f, 500f)] private FloatRange attackMoveBackX = new FloatRange(350f, 450f);
	[SerializeField, FloatRangeSlider(0f, 500f)] private FloatRange attackMoveBackY = new FloatRange(250f, 350f);
	[SerializeField] private float defaultGravityScale = 8f;
	[SerializeField] private float fallingGravityScale = 12f;
	[SerializeField] private bool hasAirControl = false;
	[SerializeField, Range(0, .3f)] private float movementSmoothing = .05f;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCheck;

	[Header("Stats")]
	[SerializeField] private int maxBucketWater = 1;
	[SerializeField] private int maxBambooPack = 3;

	[Header("Attacking")]
	[SerializeField] private bool canAirAttack;
	[SerializeField] private float attackReset = 0.2f;
	[SerializeField] private GameObject attackArea;

	[Header("References")]
	[SerializeField] private Dependency<Rigidbody2D> _body;
	[SerializeField] private Dependency<Animator> _animator;

	private bool isGrounded;
	private bool canAttack;
	private bool isJumping;
	private bool facingRight = true;
	private bool jumpInput;
	private Vector2 movementInputs = Vector2.zero;
	private Vector3 currentVelocity = Vector3.zero;
	private Coroutine resettingAttack;
	private int waterBucketCount;
	private bool isDead;

	private Animator animator => _animator.Resolve(this);
	private Rigidbody2D body => _body.Resolve(this);

	public int BambooPackCount { get; set; }
	public bool FullOfBucketWater => waterBucketCount == maxBucketWater;
	public bool FullOfBambooPack => BambooPackCount == maxBambooPack;
	public bool HasWaterBucket => waterBucketCount > 0;
	public bool HasBambooPack => BambooPackCount > 0;

	private void Start()
	{
		body.gravityScale = defaultGravityScale;
		canAttack = true;
	}

	private void Update()
	{
		if (isDead) return;

		// Attacking
		if (canAttack && Input.GetButtonDown("Attack") && (canAirAttack || !isJumping))
		{
			if (!HasWaterBucket)
				body.AddForce(new Vector2((facingRight ? -1f : 1f) * attackMoveBackX.RandomValue, attackMoveBackY.RandomValue));

			this.TryStartCoroutine(ResetAttack(), ref resettingAttack);
		}

		// Momvements
		if (!jumpInput)
		{
			jumpInput = Input.GetButtonDown("Jump");
		}

		movementInputs.x = Input.GetAxisRaw("Horizontal");
		movementInputs.y = Input.GetAxisRaw("Vertical");
	}

	private void FixedUpdate()
	{
		if (isDead) return;

		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
				isJumping = false;
				body.gravityScale = defaultGravityScale;
			}
		}

		if (isJumping && body.velocity.y < 0f)
		{
			body.gravityScale = fallingGravityScale;
		}

		Move(movementInputs, jumpInput);
	}

	private IEnumerator ResetAttack()
	{
		canAttack = false;
		attackArea.gameObject.SetActive(true);

		yield return new WaitForSeconds(attackReset);

		waterBucketCount = Mathf.Max(0, waterBucketCount - 1);
		attackArea.gameObject.SetActive(false);
		canAttack = true;
	}

	public void Move(Vector2 direction, bool jump)
	{
		if (isGrounded || hasAirControl)
		{
			Vector3 targetVelocity = new Vector2(direction.x * moveSpeed, body.velocity.y);
			body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref currentVelocity, movementSmoothing);

			if (direction.x > 0 && !facingRight)
			{
				Flip();
			}
			else if (direction.x < 0 && facingRight)
			{
				Flip();
			}
		}
		if (isGrounded && jump)
		{
			jumpInput = false;
			isGrounded = false;
			isJumping = true;
			body.AddForce(new Vector2(0f, jumpForce));
		}
	}

	public void CollectWaterBucket()
	{
		waterBucketCount = Mathf.Min(maxBucketWater, waterBucketCount + 1);
	}

	public void CollectBambooPack()
	{
		BambooPackCount = Mathf.Min(maxBambooPack, BambooPackCount + 1);
	}

	private void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void KilledByRiver()
	{
		isDead = true;
		body.Freeze();
		Level.ReloadScene();
	}
}