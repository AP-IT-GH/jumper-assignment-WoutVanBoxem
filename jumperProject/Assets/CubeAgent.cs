using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    public GameObject cuboid;
    private Rigidbody cuboidRb;
    private bool shouldHitCuboid;
    public float maxCuboidXPosition = 12f;
    public float jumpCooldown = 5f;
    private float timeSinceLastJump;

public void Start(){
    cuboidRb = cuboid.GetComponent<Rigidbody>();
    cuboidRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    Rigidbody cubeRb = GetComponent<Rigidbody>();
    cubeRb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
    timeSinceLastJump = jumpCooldown;
}
    public override void OnEpisodeBegin()
{
    transform.position = new Vector3(7.77f, 0.5f, 0);
    shouldHitCuboid = Random.value > 0.5f;

    // Move cuboid to a fixed position with a random speed
    cuboidRb.velocity = Vector3.right * Random.Range(0.5f, 1f);
    cuboid.transform.position = new Vector3(-8.55f, 0.5f, 0f);
}

public override void CollectObservations(VectorSensor sensor)
{
    // Observe cube position
    sensor.AddObservation(transform.position);

    // Observe cuboid position
    sensor.AddObservation(cuboid.transform.position);

    // Observe cuboid velocity
    sensor.AddObservation(cuboidRb.velocity.x);

    // Observe whether should hit cuboid
    sensor.AddObservation(shouldHitCuboid);
}

public override void OnActionReceived(ActionBuffers actions)
{
    // Jump action
    if (actions.DiscreteActions[0] == 1 && transform.position.y <= 1f)
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 39.24f);
        timeSinceLastJump = 0f;
        // Reward for jumping when should not hit cuboid
        if (!shouldHitCuboid)
        {
            AddReward(0.5f);
        }
        // Penalty for jumping when should hit cuboid
        else if (shouldHitCuboid)
        {
            AddReward(-1.0f);
        }
    }
}

private void OnCollisionEnter(Collision collision)
{
    // Reward for hitting cuboid when should hit cuboid
    if (shouldHitCuboid && collision.gameObject.CompareTag("Cuboid"))
    {
        AddReward(1.0f);
        EndEpisode();
    }
    // Penalty for hitting cuboid when should not hit cuboid
    else if (!shouldHitCuboid && collision.gameObject.CompareTag("Cuboid"))
    {
        AddReward(-1.0f);
        EndEpisode();
    }
}
private void FixedUpdate()
{
    // Check if cuboid is below a certain height and the CubeAgent is above it
    if (!shouldHitCuboid && cuboid.transform.position.y < 1f && transform.position.y > 1f)
    {
        AddReward(1.0f);
        EndEpisode();
    }
    // Penalty for not jumping when should not hit cuboid
    else if (shouldHitCuboid && cuboid.transform.position.x < transform.position.x && transform.position.y <= 0.5f)
    {
        AddReward(-1.0f);
        EndEpisode();
    }
    if (cuboid.transform.position.x > maxCuboidXPosition)
    {
        EndEpisode();
    }
    timeSinceLastJump += Time.fixedDeltaTime;
}
}

