using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    private CharacterController character;
    private Vector3 direction;
    public float jumpForce = 8f;
    public float gravity = 9.81f * 2f;
    private bool neverJumped = true;

    public override void Initialize()
    {
        character = GetComponent<CharacterController>();
    }

    public override void OnEpisodeBegin()
    {
        direction = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        GameObject closestObstacle = getClosestObstacle();

        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);

        sensor.AddObservation(getObstacleXPosition(closestObstacle));
        sensor.AddObservation(getObstacleYPosition(closestObstacle));

        sensor.AddObservation(getObstacleWidth(closestObstacle));
        sensor.AddObservation(getObstacleHeight(closestObstacle));

        sensor.AddObservation(character.isGrounded);
        sensor.AddObservation(GameManagerAgent.Instance.gameSpeed);
    }

    private GameObject getClosestObstacle()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        float closestDistance = Mathf.Infinity;
        GameObject closestObstacle = null;

        foreach (GameObject obstacle in obstacles)
        {
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObstacle = obstacle;
            }
        }

        return closestObstacle;
    }

    private float getObstacleYPosition(GameObject obstacle)
    {
        if (obstacle == null) {
            return -1f;
        } else {
            return obstacle.transform.position.y;
        }
    }

    private float getObstacleXPosition(GameObject obstacle)
    {
        if (obstacle == null) {
            return -1f;
        } else {
            return obstacle.transform.position.x;
        }
    }

    private float getObstacleWidth(GameObject obstacle)
    {
        if (obstacle == null) {
            return -1f;
        } else {
            return obstacle.GetComponent<BoxCollider>().size.x;
        }
    }

    private float getObstacleHeight(GameObject obstacle)
    {
        if (obstacle == null) {
            return -1f;
        } else {
            return obstacle.GetComponent<BoxCollider>().size.y;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0];
        direction += Vector3.down * gravity * Time.deltaTime;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            if (jumpAction == 1 && neverJumped) {
                direction = Vector3.up * jumpForce;
                neverJumped = false;
            } else if (jumpAction == 0) {
                neverJumped = true;
            }
        }

        character.Move(direction * Time.deltaTime);
        AddReward(.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.Space)) {
            actions[0] = 1;
        } else {
            actions[0] = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) {
            AddReward(-1f);
            GameManagerAgent.Instance.GameOver();
        } else if (other.CompareTag("playerPassChecker")) {
            // AddReward(.5f);
        }
    }
}
