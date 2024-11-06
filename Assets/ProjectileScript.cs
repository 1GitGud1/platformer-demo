using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Rigidbody2D m_Rigidbody2D;
    public Transform m_ArrowCheck;
    public LayerMask HitLayer;

    public int arrowDamage;
    private bool hasHit = false;

    // Start is called before the first frame update
    void Start()
    {
        //m_Rigidbody2D.AddForce(new Vector2(force*1000, 0f));
        //ApplyForce(force*1000, direction);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(m_ArrowCheck.position, .025f, HitLayer) && !hasHit)
		{
            //after a hit the arrow either stops where it lands or stick to an entity
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            hasHit = true;
            Collider2D hit = Physics2D.OverlapCircle(m_ArrowCheck.position, .025f, HitLayer);
            if (hit.gameObject.tag == "Player")
            {
                transform.parent = hit.gameObject.transform;
                hit.GetComponent<CharacterStats>().TakeDamage(arrowDamage, transform.position);
            }
            StartCoroutine(Expire());
		}
    }

    void FixedUpdate()
    {
        if (!hasHit)
        {
            float angleRad = Mathf.Atan2(m_Rigidbody2D.velocity.x, -m_Rigidbody2D.velocity.y);
            float angleDeg = (180f/Mathf.PI) * angleRad - 90;

            transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
    }

    private void OnCollisionEnter2D()
    {
        hasHit = true;
        //m_Rigidbody2D.velocity = Vector2.zero;
        // m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        // need to change collider to a wall checker or raycast. bouncing from collider is unnecessary.

        GetComponent<CircleCollider2D>().enabled = false;
        m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
    }

    private IEnumerator Expire()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
