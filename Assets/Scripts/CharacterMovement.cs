using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    public CharacterController2D controller; 
    public Animator animator;
    public AttackScript attackScript;
    public InventoryScript inventoryScript;
    public GameObject inventory;
    public FieldOfViewScript fieldOfViewScript;
    SpriteRenderer weaponSpriteRenderer;

    float horizontalMove = 0f;
    public float runSpeed = 6f;
    float moveSpeed;
    float attackMoveSpeed;
    bool jump = false;
    bool crouch = false;
    
    bool inventoryToggle = false;
    int slotSelector = 0;
    int slotSelectorColumn = 0;
    int slotSelectorRow = 0;

    public LayerMask itemLayer;
    

    void Start()
    {
        moveSpeed = runSpeed;
        attackMoveSpeed = runSpeed/4;
        inventory.SetActive(false);
        weaponSpriteRenderer = transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fieldOfViewScript.SetOrigin(transform.position);
        if (inventoryToggle == false)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            animator.SetFloat("speed", Mathf.Abs(horizontalMove));

            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("isJumping", true);
            }
            else if (Input.GetButtonUp("Jump"))
            {
                controller.jumpCutScript();
            }

            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
                animator.SetBool("isCrouching", crouch);

                TryPickUpItem();
            } 
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
                animator.SetBool("isCrouching", crouch);
            }

            if(horizontalMove != 0 && controller.velocityY <= 0)
            {
                controller.LedgeGrab();
            }

            if (!controller.attackTrue)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    animator.speed = attackScript.attackRate;
                    controller.attackTrue = true;
                    //if (!animator.GetBool("isJumping"))
                    runSpeed = attackMoveSpeed;
                    animator.SetBool("Attacking", true);
                }
            }
        }



        else {
            if (Input.GetKeyDown(KeyCode.D))
            {
                slotSelectorColumn = (slotSelectorColumn + 1) % 7;
                slotSelector = slotSelectorColumn + (slotSelectorRow * 7);
                inventoryScript.UpdateSelectedSlot(slotSelector);
                
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                slotSelectorColumn = (slotSelectorColumn + 6) % 7;
                slotSelector = slotSelectorColumn + (slotSelectorRow * 7);
                inventoryScript.UpdateSelectedSlot(slotSelector);
                
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                slotSelectorRow = (slotSelectorRow + 1) % 5;
                slotSelector = slotSelectorColumn + (slotSelectorRow * 7);
                inventoryScript.UpdateSelectedSlot(slotSelector);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                slotSelectorRow = (slotSelectorRow + 4) % 5;
                slotSelector = slotSelectorColumn + (slotSelectorRow * 7);
                inventoryScript.UpdateSelectedSlot(slotSelector);
            }

            if (Input.GetKeyDown(KeyCode.L))
                {
                    if (inventoryScript.isFull[slotSelector] && !(inventoryScript.lastEquippedSlot == slotSelector && inventoryScript.weaponEquipped))
                    {
                        inventoryScript.SOslots[slotSelector].DropItem(transform.position);
                        inventoryScript.slots[slotSelector].GetComponent<SlotScript>().RemoveItem();
                        inventoryScript.isFull[slotSelector] = false;
                    }
                }

            if (Input.GetKeyDown(KeyCode.I))
                {
                    if (inventoryScript.isFull[slotSelector])
                    {
                        if(!(inventoryScript.weaponEquipped && slotSelector == inventoryScript.lastEquippedSlot))
                        {
                            inventoryScript.SOslots[slotSelector].Use();
                            inventoryScript.slots[slotSelector].GetComponent<SlotScript>().RemoveItem();
                            inventoryScript.isFull[slotSelector] = false;
                        }
                    }
                }

            if (Input.GetKeyDown(KeyCode.J))
                {
                    if (inventoryScript.isFull[slotSelector])
                    {
                        if (!inventoryScript.weaponEquipped)
                        {
                            inventoryScript.EquipWeapon(slotSelector);
                            weaponSpriteRenderer.sprite = inventoryScript.SOslots[slotSelector].ItemSprite;
                        }
                        else if(slotSelector == inventoryScript.lastEquippedSlot)
                        {
                            inventoryScript.UnequipWeapon(slotSelector);
                            weaponSpriteRenderer.sprite = null;
                        }
                        else
                        {
                            inventoryScript.SwitchWeapon(slotSelector);
                            weaponSpriteRenderer.sprite = inventoryScript.SOslots[slotSelector].ItemSprite;
                        }
                    }
                }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {   
            inventory.SetActive(!inventoryToggle);
            if (inventoryToggle == true){
            }
            else {
                horizontalMove = 0;
                animator.SetFloat("speed", Mathf.Abs(horizontalMove));
                controller.jumpCutScript();
                crouch = false;
                animator.SetBool("isCrouching", crouch);
            }
            
            inventoryToggle = !inventoryToggle;
        }

    }

    public void onLanding()
    {
        //animator.SetBool("isJumping", false);

        controller.m_AirControl = true;
    }

    void FixedUpdate()
    {
        if (controller.grab)
        {
            horizontalMove = 0f;
            crouch = false;
        }
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void attackEndMoveSpeed()
    {
        animator.speed = 1f;
        controller.attackTrue = false;
        animator.SetBool("Attacking", false);
        runSpeed = moveSpeed;
    }

    private void TryPickUpItem()
    {
        Vector2 playerPosition = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, Vector2.down, 1f, itemLayer);

        if (hit.collider != null)
        {
            // Item is detected beneath the player
            GameObject item = hit.collider.gameObject;
            // Implement the logic to pick up the item
            // You might want to disable the item's renderer, collider, etc.
            item.GetComponent<itemPickup>().PickUpItem();
        }
    }

}
