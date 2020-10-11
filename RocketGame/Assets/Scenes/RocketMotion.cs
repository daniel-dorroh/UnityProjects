using UnityEngine;

public class RocketMotion : MonoBehaviour
{
    public float ThrustForce = 20.0f;
    public float ManeuverAngle = 50.0f;
    public Rigidbody Rocket;
    public AudioSource RocketPulse;
    public AudioSource ManeuverBlast;
    private bool _hasManeuveredLeft = false;
    private bool _hasManeuveredRight = false;

    public void Start()
    {
        if (Rocket == default(Rigidbody))
        {
            Rocket = GetComponent<Rigidbody>();
        }
        if (RocketPulse == null)
        {
            RocketPulse = GetComponent<AudioSource>();
        }
        if (ManeuverBlast == null)
        {
            ManeuverBlast = GetComponent<AudioSource>();
        }
    }

    public void Update()
    {
        Thrust();
        Maneuver();
    }

    private void Thrust()
    { 
        if (Input.GetKey(KeyCode.Space))
        {
            Rocket.AddRelativeForce(new Vector3(0, ThrustForce, 0));

            if (!RocketPulse.isPlaying)
            {
                RocketPulse.time = Random.value * 2.0f;
                RocketPulse.loop = true;
                RocketPulse.Play();
            }
        }
        else
        {
            RocketPulse.Stop();
        }
    }

    private void Maneuver()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _hasManeuveredRight = false;
            transform.Rotate(Vector3.forward, ManeuverAngle * Time.deltaTime);
            _hasManeuveredLeft = PlayManeuverSound(_hasManeuveredLeft);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _hasManeuveredLeft = false;
            transform.Rotate(Vector3.back, ManeuverAngle * Time.deltaTime);
            _hasManeuveredRight = PlayManeuverSound(_hasManeuveredRight);
        }
        else
        {
            ManeuverBlast.Stop();
            _hasManeuveredRight = false;
            _hasManeuveredLeft = false;
        }
    }

    private bool PlayManeuverSound(bool hasManeuvered)
    {
        if (!hasManeuvered && !ManeuverBlast.isPlaying)
        {
            hasManeuvered = true;
            ManeuverBlast.time = 0.4f;
            ManeuverBlast.loop = false;
            ManeuverBlast.Play();
        }
        return hasManeuvered;
    }
}
