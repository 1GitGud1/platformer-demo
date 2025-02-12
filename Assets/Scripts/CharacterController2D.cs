using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] public bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings

	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .06f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	private Vector2 WallCheck;	// A position marking where to check if the player is against a wall
	const float k_WallCheckRadius = .025f;
	public bool grab = false;
	public bool attackTrue = false;
	private float startingGrav;
	public float velocityY;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		
		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		startingGrav = m_Rigidbody2D.gravityScale;
	}

	private void FixedUpdate()
	{
		velocityY = m_Rigidbody2D.velocity.y;
		animator.SetFloat("speedY", velocityY);
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		animator.SetBool("isJumping", !m_Grounded);

	}
	
	public void LedgeGrab()
	{
		WallCheck = m_Rigidbody2D.position + (new Vector2(0.058f * transform.localScale.x * 17, 0.09f));
		
		if (!m_Grounded && !grab && !attackTrue && Physics2D.OverlapCircle(WallCheck, k_WallCheckRadius, m_WhatIsGround) && !Physics2D.OverlapCircle((WallCheck + new Vector2(0f, 0.05f)), k_WallCheckRadius, m_WhatIsGround))
		{
			m_Rigidbody2D.velocity = new Vector2(0f, 0f);
			m_Rigidbody2D.gravityScale = 0f;

			grab = true;
			animator.SetBool("isLedgeGrabbing", true);
			m_AirControl = true;
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight && !attackTrue)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight && !attackTrue)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if ((m_Grounded || grab) && jump)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.gravityScale = startingGrav;
			grab = false;
			animator.SetBool("isLedgeGrabbing", false);
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce), ForceMode2D.Impulse);
		}
	}

	public void jumpCutScript()
	{
		if (!m_Grounded && m_Rigidbody2D.velocity.y > 0)
		{
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetComponent<Rigidbody2D>().position + (new Vector2(0.058f * transform.localScale.x * 17, 0.09f)), k_WallCheckRadius);
		Gizmos.color = Color.red;
        Gizmos.DrawSphere((GetComponent<Rigidbody2D>().position + (new Vector2(0.058f * transform.localScale.x * 17, 0.09f)) + new Vector2(0f, 0.05f)), k_WallCheckRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(m_GroundCheck.position, k_GroundedRadius);
    }
}
