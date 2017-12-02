﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    [Header("Settings")]
    public bool isActuallyAPerson = true;
    public AudioClip m_barkSound;
    public float m_speed;
    public float m_attentionSpan;
    public float m_alertRadius;
    public string m_name;
    float offset = 0;
    DogState m_state = DogState.following;

    enum DogState
    {
        waiting,
        following,
        running,
        snoofing
    }

    [Header("References")]
    public LineRenderer m_lead;
    public Rigidbody2D m_rigidbody;
    public DistanceJoint2D m_joint;
    public PointMaker m_route;
    public GameObject m_owner;

    //airhorns
    [SerializeField]
    Transform targetthing;

    void Awake()
    {
        if (m_owner)
        {
            m_joint = m_owner.AddComponent<DistanceJoint2D>();
            m_joint.maxDistanceOnly = true;
            m_joint.autoConfigureDistance = false;
            m_joint.distance = Player.m_leadLength;
            m_joint.connectedBody = m_rigidbody;

            m_attentionSpan = Random.Range(1.5f, 15);

            float r = Random.Range(0.75f, 1.5f);
            transform.localScale = Vector3.one * r;
            m_rigidbody.mass *= r;
            m_speed *= r;
            {
                offset = Random.value * 6;
            }
        }
    }

    void Update()
    {
        if (!isActuallyAPerson)
        {
            if (m_owner.tag == "Player")
                m_joint.distance = Player.m_leadLength;
            else
                m_joint.distance = 2.5f;
            Vector3[] newPos =
                {
                    transform.position,
                    m_owner.transform.position
                };
            m_lead.SetPositions(newPos);
        }
        Vector3 target = Vector3.zero;

        switch (m_state)
        {
            case DogState.waiting:
                break;
            case DogState.following:
                target = m_route.WhereGo(transform.position, offset, !isActuallyAPerson);
                break;
            case DogState.running:
                break;
            case DogState.snoofing:
                break;
        }

        targetthing.transform.position = target - Vector3.back * 10;

        float n = .0f;
        Vector3 noise = new Vector3(Random.Range(-n, n), Random.Range(-n, n), Random.Range(-n, n));
        m_rigidbody.AddForce((target - transform.position + noise).normalized * m_speed);

        //Vector3 pos = Vector3.Lerp(transform.position, target, Time.deltaTime);
        //pos.z = 0;
        //transform.position = pos;
        Vector3 diff = target - transform.position + noise;
        diff.Normalize();
        float z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(0, 0, z + 180), Time.deltaTime);

        // MoveCamera();
    }

    Vector3 targetPos;

    void MoveCamera()
    {
        Vector3 p = Camera.main.WorldToViewportPoint(transform.position);
        if (p.x > 0.6)
        {
            targetPos += Vector3.right * Time.deltaTime * 5;
        }
        if (p.x < 0.4)
        {
            targetPos += Vector3.left * Time.deltaTime * 5;
        }
        if (p.y > 0.6)
        {
            targetPos += Vector3.up * Time.deltaTime * 5;
        }
        if (p.y < 0.4)
        {
            targetPos += Vector3.down * Time.deltaTime * 5;
        }
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * 15);
    }
}