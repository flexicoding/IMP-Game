using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class NewAiController : MonoBehaviour
{
    public float Speed = 1;
    public float WidthOffset = 5;
    public Transform PlayerTransform;
    public float ObstacleStepSize = 1;

    private RaycastHit prev_obstacle = new RaycastHit();
    private float prev_adj = 0;

    private void FixedUpdate()
    {
        if(transform.position == PlayerTransform.position) return;

        transform.LookAt(PlayerTransform);
        var obstacle_cast = Physics.Raycast(PlayerTransform.position, transform.position, out var obstacle_1);
        if(obstacle_1.collider == prev_obstacle.collider)
        {
            Debug.DrawRay(PlayerTransform.position + new Vector3(prev_adj, 0, 0), transform.position, Color.green, 1);
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
}