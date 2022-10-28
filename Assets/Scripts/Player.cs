using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Delegate for an event to be raised when player dies
public delegate void KillPlayerEvent();


/// <summary>
/// Class responsible for Player mechanism and logic
/// such as: Player movement, player health, physics, and speed 
/// </summary>

public class Player : MonoBehaviour
{

    [Header("Player stats and Camera follow")]
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float health;

    private float maxHealth;

    //System event invoked when player dies
    public event KillPlayerEvent m_Kill;

    private Rigidbody m_Rigidbody;


    private Slider healthBar;
    private Vector3 cameraForwardVector;
    private Vector3 cameraRightVector;

    private Material playerMat;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        healthBar = FindObjectOfType<Slider>();
        healthBar.minValue = 0; 
        healthBar.maxValue = health;
        healthBar.value = health;
        m_Rigidbody = GetComponent<Rigidbody>();
        playerMat = GetComponent<MeshRenderer>().material;
        ResetMaterial();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ProcessMovement();
    }


    //Deal Damage when projectile hit player
    internal void DealDamage(float damage)
    {
        health -= damage;
        float GB_Value = fromHealthtoRGB(health);
        playerMat.color = new Color(1, GB_Value / 255, GB_Value / 255, 1/(GB_Value/255));
        healthBar.value = health;
        if (health <= 0)
        {
            m_Kill?.Invoke();
            Destroy(gameObject);
            return;
        }
        return;
    }


    /// <summary>
    /// Camera relative-based movement that moves forward on the camera's direction not the world forward vector
    /// </summary>
    private void ProcessMovement()
    {

        //Axis value for player Input
        float verticalAxis = Input.GetAxis("Vertical");
        float HorizontalAxis = Input.GetAxis("Horizontal");


        //Camera's forward and right vectors
        cameraForwardVector = m_Camera.transform.forward;
        cameraRightVector = m_Camera.transform.right; 


        //Clearing the y-axis on the forward and right vectors to nullify up and down force
        cameraForwardVector.y = 0;
        cameraRightVector.y = 0;

        //Normalizing the vectors for constant speed based on camera angle and direction
        cameraForwardVector = cameraForwardVector.normalized;
        cameraRightVector = cameraRightVector.normalized;


        //Force vectors used for the rigidBody's AddForce function to move the player
        Vector3 forwardRelativeMovement = verticalAxis * cameraForwardVector;
        Vector3 rightRelativeMovement = HorizontalAxis * cameraRightVector;



        m_Rigidbody.AddForce(rightRelativeMovement.normalized * m_Speed * Time.deltaTime, ForceMode.Force);
        m_Rigidbody.AddForce(forwardRelativeMovement.normalized * m_Speed * Time.deltaTime, ForceMode.Force);

        //Clamping the magnitude of the rigidBody's velocity so the player doesn't over-speed the canon bullets.
        if (m_Rigidbody.velocity.magnitude > m_Speed)
        {
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, m_Speed);
        }

        //Debug.Log(m_Rigidbody.velocity);
    }


    private float fromHealthtoRGB(float currHealth){
        return (currHealth / maxHealth) * 255;
    }

    private void ResetMaterial(){
        playerMat.color = Color.white;
    }
}
