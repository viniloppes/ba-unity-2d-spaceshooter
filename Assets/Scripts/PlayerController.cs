using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    //Movement
    public float speed = 0.5f;
    public InputAction LeftAction;
    public InputAction MoveAction;
    private Rigidbody2D rigidbody2d;
    private Vector2 move;


    //Health variables control
    private int currentHealth = 0;
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }

    // Variables related to temporary invincibility
    public float timeInvincible = 2.0f;
    public bool isInvincible = false;
    public float damageCooldown;


    //Animation
    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);



    //Projectile
    public GameObject projectilePrefab;

    public InputAction talkAction;


    AudioSource audioSource;
    public AudioClip collectedClip;
    // Start is called before the first frame update
    void Start()
    {
        MoveAction.Enable();
        talkAction.Enable();
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;//currentHealth = 1;  // 

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
        move = MoveAction.ReadValue<Vector2>();


        //Animation
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        //animator.SetFloat("Look X", moveDirection.x);
        //animator.SetFloat("Look Y", moveDirection.y);
      //  animator.SetFloat("Speed", move.magnitude);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Try Talk with NPC");
            FindFriend();
        }

    }
    private void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);

       // animator.SetTrigger("Hit");
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(Vector2.up, 500);
       // animator.SetTrigger("Launch");
    }

    void FindFriend()
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {

            UIHandler.instance.DisplayDialogue();
        } else
        {

        }

    }
}
