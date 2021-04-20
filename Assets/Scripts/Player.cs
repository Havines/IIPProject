using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public CinemachineVirtualCamera camera;
    
    [SerializeField] private float speed = 3f;
    
    private Vector2 motionVector;
    

    private void Awake()
    {
        camera = FindObjectOfType<CinemachineVirtualCamera>();
        camera.Follow = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        motionVector = new Vector2(horizontal, vertical);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rigidbody2D.velocity = motionVector * speed;
    }
}