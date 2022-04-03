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

	[Header("Attacking")]
	[SerializeField] private bool canAttack;
	[SerializeField] private bool canAirAttack;
	[SerializeField] private float attackReset = 0.2f;
	[SerializeField] private GameObject attackArea;

	[Header("References")]
	[SerializeField] private Dependency<Rigidbody2D> _body;
	[SerializeField] private Dependency<Animator> _animator;

	private bool isGrounded;
	private bool attackReady;
	private bool isJumping;
	private bool facingRight = true;
	private bool jumpInput;
	private Vector2 movementInputs = Vector2.zero;
	private Vector3 currentVelocity = Vector3.zero;
	private Coroutine resettingAttack;
	private int waterBucketCount;
	private int bambooPackCount;
	private bool isDead;
	private int maxBucketWater;
	private int maxBambooPack;
	private bool airAttackDone;

	private Animator animator => _animator.Resolve(this);
	private Rigidbody2D body => _body.Resolve(this);

	public int MaxBucketWater
	{
		get => maxBucketWater;
		set
		{
			maxBucketWater = value;
			HUD.SetupBucketIcon(maxBucketWater);
		}
	}

	public int MaxBambooPack
	{
		get => maxBambooPack;
		set
		{
			maxBambooPack = value;
			HUD.SetupBambooIcon(maxBambooPack);
		}
	}

	public int BambooPackCount
	{
		get => bambooPackCount;
		set
		{
			bambooPackCount = Mathf.Max(0, Mathf.Min(value, MaxBambooPack));
			HUD.UpdateBambooUsage(bambooPackCount);
		}
	}

	public int WaterBucketCount
	{
		get => waterBucketCount;
		set
		{
			waterBucketCount = Mathf.Max(0, Mathf.Min(value, MaxBucketWater));
			HUD.UpdateBucketUsage(waterBucketCount);
		}
	}
	public bool FullOfBucketWater => WaterBucketCount == MaxBucketWater;
	public bool FullOfBambooPack => BambooPackCount == MaxBambooPack;
	public bool HasWaterBucket => WaterBucketCount > 0;
	public bool HasBambooPack => BambooPackCount > 0;

	private void Start()
	{
		WaterBucketCount = 0;
		BambooPackCount = 0;
		MaxBucketWater = Level.StartMaxBucketWater;
		MaxBambooPack = Level.StartMaxBambooPack;

		body.gravityScale = defaultGravityScale;
		attackReady = true;
	}

	private void Update()
	{
		if (isDead) return;

		// Attacking
		if (Input.GetButtonDown("Attack"))
		{
			if (HasWaterBucket)
			{
				animator.SetTrigger("Attack");
				this.TryStartCoroutine(ResetAttack(), ref resettingAttack);
			}
			else if (canAttack && attackReady && ((canAirAttack && !airAttackDone) || !isJumping))
			{
				animator.SetTrigger("Attack");
				this.TryStartCoroutine(ResetAttack(), ref resettingAttack);
				if (!isJumping)
				{
					body.AddForce(new Vector2((facingRight ? -1f : 1f) * attackMoveBackX.RandomValue, attackMoveBackY.RandomValue));
				}
				else
				{
					airAttackDone = true;
				}
				Level.GenerateImpulse();
			}
		}

		// Momvements
		if (!jumpInput)
		{
			jumpInput = Input.GetButtonDown("Jump");
		}

		movementInputs.x = Input.GetAxisRaw("Horizontal");
		movementInputs.y = Input.GetAxisRaw("Vertical");

		animator.SetBool("Walking", movementInputs.x != 0f);
		animator.SetBool("Jumping", isJumping);
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
				airAttackDone = false;
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
		attackReady = false;
		attackArea.gameObject.SetActive(true);

		yield return new WaitForSeconds(attackReset);

		WaterBucketCount--;
		attackArea.gameObject.SetActive(false);
		attackReady = true;
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
		WaterBucketCount++;
	}

	public void CollectBambooPack()
	{
		BambooPackCount++;
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
		animator.SetTrigger("WaterDeath");
		isDead = true;
		body.Freeze();
		Level.InverseColor();
		Level.GenerateImpulse();

		StartCoroutine(KilledByRiverCore());
	}

	private IEnumerator KilledByRiverCore()
	{
		yield return new WaitForSeconds(1f);
		Level.ReloadScene();
	}
}