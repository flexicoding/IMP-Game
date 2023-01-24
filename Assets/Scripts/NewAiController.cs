using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class NewAiController : MonoBehaviour
{
    public float Speed = 1;
    public float WidthOffset = 5;
    public Transform PlayerTransform;
    public float ObstacleStepSize = 1;
    
    public List<Transform> BulletOrigins;
    public TrailRenderer BulletTrail;
    public float ShootDelay = 0.2f;
    public float HealthRegenTime = 0.5f;
    public float ShieldRegenTime = 0.2f;
    public int Damage = 10;

    public GameObject Player;

    public int health = 100;

    private RaycastHit prev_obstacle = new RaycastHit();
    private float prev_adj = 0;
    private float shoot_time = 0;

    private void avoid_obstacles()
    {
        if(transform.position == PlayerTransform.position) return;

        transform.LookAt(PlayerTransform);
        var obstacle_cast = Physics.Raycast(PlayerTransform.position, transform.position, out var obstacle_1);
        if(obstacle_1.collider == prev_obstacle.collider)
        {
            Debug.DrawRay(PlayerTransform.position + new Vector3(prev_adj + WidthOffset, 0, 0), transform.position, Color.green, 1);
            transform.position = Vector3.MoveTowards(transform.position, PlayerTransform.position + new Vector3(prev_adj + WidthOffset, 0, 0), Time.deltaTime * Speed);
            return;
        }

        var positive_adj = obstacle_cast ? 0 : ObstacleStepSize;
        var obstacle_2 = obstacle_1;

        while(obstacle_cast)
        {
            obstacle_cast = Physics.Raycast(PlayerTransform.position + new Vector3(positive_adj + WidthOffset, 0, 0), transform.position, out obstacle_2);
            if(obstacle_1.collider != obstacle_2.collider) break;
            positive_adj += ObstacleStepSize;
        }

        obstacle_cast = Physics.Raycast(PlayerTransform.position, transform.position, out obstacle_1);
        var negative_adj = obstacle_cast ? 0 : -ObstacleStepSize;

        while(obstacle_cast)
        {
            obstacle_cast = Physics.Raycast(PlayerTransform.position + new Vector3(negative_adj + WidthOffset, 0, 0), transform.position, out obstacle_2);
            if(obstacle_1.collider != obstacle_2.collider) break;
            negative_adj -= ObstacleStepSize;
        }
        
        if(-negative_adj > positive_adj)
        {
            Debug.DrawRay(PlayerTransform.position + new Vector3(positive_adj + positive_adj != 0 ? WidthOffset : 0, 0, 0), transform.position, Color.red, 1);
            transform.position = Vector3.MoveTowards(transform.position, PlayerTransform.position + new Vector3(positive_adj + positive_adj != 0 ? WidthOffset : 0, 0, 0), Time.deltaTime * Speed);
            prev_obstacle = obstacle_1;
            prev_adj = positive_adj;
        }
        Debug.DrawRay(PlayerTransform.position + new Vector3(negative_adj + positive_adj != 0 ? WidthOffset : 0, 0, 0), transform.position, Color.red, 1);
        transform.position = Vector3.MoveTowards(transform.position, PlayerTransform.position + new Vector3(negative_adj + positive_adj != 0 ? WidthOffset : 0, 0, 0), Time.deltaTime * Speed);
        prev_obstacle = obstacle_1;
        prev_adj = negative_adj;
    }

    private void shoot()
    {
        if(Physics.Raycast(PlayerTransform.position, transform.position) || (shoot_time + ShootDelay > Time.time)) return;

        foreach(var o in BulletOrigins)
        {
            if (Physics.Raycast(o.position, o.forward, out RaycastHit hit, 100))
            {
                TrailRenderer trail = Instantiate(BulletTrail, o.position, Quaternion.identity);

                StartCoroutine(MoveTrail(trail, hit.point));

                shoot_time = Time.time;
            }
            else
            {
                TrailRenderer trail = Instantiate(BulletTrail, o.position, Quaternion.identity);
                StartCoroutine(MoveTrail(trail, new Ray(o.position, o.forward).GetPoint(100)));

                shoot_time = Time.time;
            }
        }
    }

    private void regen()
    {

    }

    private void FixedUpdate()
    {
        avoid_obstacles();
        shoot();
        regen();

        if (health <= 0) gameObject.SetActive(false);
    }

    private IEnumerator MoveTrail(TrailRenderer trail, Vector3 target)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, target, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = target;

        if (!Physics.Raycast(PlayerTransform.position, transform.position)) Player.GetComponent<PlayerController>().HP -= Damage;

        Destroy(trail.gameObject, trail.time);
    }
}