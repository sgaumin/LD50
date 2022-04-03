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
	[Header("Stats")]
	[SerializeField] private bool forceFlipAtStartup;
	[SerializeField] private int startMaxBucketWater = 1;
	[SerializeField] private int startMaxBambooPack = 3;

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

	[Header("Audio")]
	[SerializeField] private AudioExpress attackingSound;
	[SerializeField] private AudioExpress bucketWaterSound;
	[SerializeField] private AudioExpress collectSound;
	[SerializeField] private AudioExpress deathSound;
	[SerializeField] private AudioExpress jumpSound;
	[SerializeField] private AudioExpress walkSound;

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
	private Coroutine attacking;
	private int waterBucketCount;
	private int bambooPackCount;
	private int maxBucketWater;
	private int maxBambooPack;
	private bool airAttackDone;
	private bool cancelInteraction;
	private bool canBeKickBack;

	private Animator animator => _animator.Resolve(this);
	private Rigidbody2D body => _body.Resolve(this);

	public bool CancelInteraction
	{
		get => cancelInteraction; set
		{
			cancelInteraction = value;
			if (cancelInteraction)
			{
				animator.SetBool("Walking", false);
				animator.SetBool("Jumping", false);
				movementInputs = Vector2.zero;
				body.velocity = Vector2.zero;
				body.angularVelocity = 0;
			}
		}
	}

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
		if (forceFlipAtStartup)
		{
			Flip();
		}

		WaterBucketCount = 0;
		BambooPackCount = 0;
		MaxBucketWater = startMaxBucketWater;
		MaxBambooPack = startMaxBambooPack;
		canBeKickBack = true;

		body.gravityScale = defaultGravityScale;
		attackReady = true;
	}

	public void ForceAllowAttack()
	{
		canAttack = true;
	}

	public void ForceDisallowAttack()
	{
		canAttack = false;
	}

	public void ForceAllowAirAttack()
	{
		canAirAttack = true;
	}

	private void Update()
	{
		if (CancelInteraction) return;

		// Attacking
		if (Input.GetButtonDown("Attack") && attackReady && ((canAirAttack && !airAttackDone) || (!isJumping && isGrounded)))
		{
			this.TryStartCoroutine(DoAttack(), ref attacking);
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

	public void DoWaterBucketAttack(Action callback)
	{
		WaterBucketCount--;
		bucketWaterSound.Play();
		animator.SetTrigger("Attack");
		Level.GenerateImpulse();

		callback?.Invoke();
	}

	public void DoNormalAttack(Action callback)
	{
		if (!canAttack) return;

		attackingSound.Play();
		animator.SetTrigger("Attack");
		Level.GenerateImpulse();
		Level.FreezeTime();

		if (!isJumping && canBeKickBack)
		{
			body.AddForce(new Vector2((facingRight ? -1f : 1f) * attackMoveBackX.RandomValue, attackMoveBackY.RandomValue));
			StartCoroutine(KickBackresetCore());
		}
		else
		{
			airAttackDone = true;
		}

		callback?.Invoke();
	}

	private IEnumerator KickBackresetCore()
	{
		canBeKickBack = false;
		yield return new WaitForSeconds(attackReset);
		canBeKickBack = true;
	}

	private void FixedUpdate()
	{
		if (CancelInteraction) return;

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

	public void ForceMovement(Vector2 direction)
	{
		movementInputs = direction;
		Move(movementInputs, false);
		animator.SetBool("Walking", true);
	}

	private IEnumerator DoAttack()
	{
		attackReady = false;
		attackArea.gameObject.SetActive(true);

		yield return new WaitForSeconds(attackReset);

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
			jumpSound.Play();
		}
		else
		{
			jumpInput = false;
		}
	}

	public void PlayWalkSound()
	{
		walkSound.Play();
	}

	public void CollectWaterBucket()
	{
		WaterBucketCount++;
		collectSound.Play();
	}

	public void CollectBambooPack()
	{
		BambooPackCount++;
		collectSound.Play();
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
		deathSound.Play();
		CancelInteraction = true;
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