using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackScript : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void knockback(float attackerPosX, float knockbackStr)
    {
        m_Rigidbody2D.velocity = new Vector2(0f, 0f);
        if (transform.position.x > attackerPosX)
        {
            m_Rigidbody2D.AddForce(new Vector2(knockbackStr, knockbackStr));
        }
        else
        {
            m_Rigidbody2D.AddForce(new Vector2(-knockbackStr, knockbackStr));
        }
    }
}
