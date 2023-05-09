using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using System.Runtime.ConstrainedExecution;
using UnityEngine.UIElements;

public class BasePlayer : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 0f;
    [SerializeField] private float _speedRotation = 0f;

    private Vector3 Direction { get; set; }

    void Update()
    {
        DefineDirection();

        Rotate();
        Move();
    }

    private void DefineDirection()
    {
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
    }

    private void Rotate()
    {
        var inputDirection = Direction;
        var directionRotation = new Vector3(inputDirection.x, 0f, inputDirection.z);
        if (directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speedRotation);

            transform.rotation = targetRotation;
        }
    }

    private void Move()
    {
        transform.position += Direction * _movementSpeed * Time.deltaTime;
    }
}
