using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class CarController : MonoBehaviour
{
    Animator anim;
    AvailablePump selectedPump;

    [SerializeField] private float minimumStopDistance = 1.5f;
    [SerializeField] private float raycastStartOffset = 0.2f;

    [SerializeField] private float stuckTimeThreshold = 3f; // Threshold for stuck detection
    private float stuckTimer = 0f; // Timer to track how long the vehicle has been stuck

    public Transform target;
    public Transform exit;
    public Transform carGraphic;
    public float nextWaypointDistance;
    public float rayDistance;
    public LayerMask layerMask;
    public Transform intersectionCheckPoint;

    public int currentWaypoint;
    private float carWaitTime;
    bool canMove;
    public bool isFueled;

    private bool isStopped = false;
    [SerializeField] private float moveSpeed;

    public List<Transform> vehiclePath = new List<Transform>();

    [SerializeField] private GameObject moneyIcon;
    [SerializeField] private TextMeshProUGUI moneyAmountPopup;

    private UnityEngine.Vector3 previousPosition;
    private UnityEngine.Vector3 currentPosition;

    public float detectionRadius = 5f;          // Range of the cone
    public float detectionAngle = 60f;          // Angle of the cone in degrees
    public LayerMask targetLayerMask;           // Layer of objects we want to detect

    private Collider2D myCollider;

    public class AvailablePump
    {
        public GasStationManager Station;
        public int PumpIndex;

        public AvailablePump(GasStationManager station, int pumpIndex)
        {
            Station = station;
            PumpIndex = pumpIndex;
        }
    }

    private void Start()
    {
        SoundManager.Instance.PlaySound("vehicle", 0.25f, true);
        moneyIcon.SetActive(false);
        moneyAmountPopup.enabled = false;
        moveSpeed = 0.5f;
        currentWaypoint = 0;

        // Get the car's own collider component
        myCollider = GetComponent<Collider2D>();

        previousPosition = transform.position;

        exit = GameObject.Find("Exit Waypoint").GetComponent<Transform>();

        anim = GetComponent<Animator>();

        // Determine which pumps are available and set a path
        GasStationManager[] gasStations = FindObjectsOfType<GasStationManager>();

        List<AvailablePump> availablePumps = new List<AvailablePump>();

        // Check availability at all gas stations
        for (int i = 0; i < gasStations.Length; i++) {
            GasStationManager currentStation = gasStations[i];

            for (int j = 0; j < currentStation.instanceData.pumpOccupied.Count; j++) {
                if (!currentStation.instanceData.pumpOccupied[j]) {
                    availablePumps.Add(new AvailablePump(currentStation, j));
                }
            }
        }

        // If pumps are available, select a random one
        if (availablePumps.Count > 0) {
            selectedPump = availablePumps[UnityEngine.Random.Range(0, availablePumps.Count)];
            selectedPump.Station.instanceData.pumpOccupied[selectedPump.PumpIndex] = true;

            switch (selectedPump.PumpIndex) {
                case 0:
                    vehiclePath = selectedPump.Station.pumpOneWaypoints;
                    vehiclePath.Add(exit);
                    break;
                case 1:
                    vehiclePath = selectedPump.Station.pumpTwoWaypoints;
                    vehiclePath.Add(exit);
                    break;
                case 2:
                    vehiclePath = selectedPump.Station.pumpThreeWaypoints;
                    vehiclePath.Add(exit);
                    break;
                case 3:
                    vehiclePath = selectedPump.Station.pumpFourWaypoints;
                    vehiclePath.Add(exit);
                    break;
                case 4:
                    vehiclePath = selectedPump.Station.pumpFiveWaypoints;
                    vehiclePath.Add(exit);
                    break;
            }
        } else {
            // No available pumps, proceed to exit
            vehiclePath.Add(exit);
        }

        isFueled = false;

        StartCar();
    }

    private void FixedUpdate()
    {
        // Always check if there’s a car too close in front
        if (vehiclePath.Count > 0) {
            target = vehiclePath[currentWaypoint];

            // Calculate the direction of movement based on the target position
            UnityEngine.Vector3 directionOfMovement = (target.position - transform.position).normalized;

            // Perform the raycast check to see if the car can move
            bool carInFront = IsCarTooCloseToStop(directionOfMovement);

            // Update `canMove` based on the presence of a car in front
            if (carInFront) {
                stuckTimer += Time.deltaTime;

                // Check if the vehicle has been stuck for too long
                if (stuckTimer >= stuckTimeThreshold) {
                    // Allow the vehicle to ignore the obstacle and move forward
                    canMove = true;
                    stuckTimer = 0; // Reset the stuck timer
                } else {
                    canMove = false; // Keep waiting if not past the threshold
                }
            } else {
                // No car in front, reset the stuck timer and allow movement
                stuckTimer = 0;
                canMove = true;
            }

            // If the car is allowed to move, proceed towards the target waypoint
            if (canMove && !isStopped) {
                // Move towards target
                transform.position = UnityEngine.Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                // Check if the car has reached the target waypoint
                if (UnityEngine.Vector3.Distance(transform.position, target.position) < 0.01f) {
                    currentWaypoint++;
                }
            }
        }

        currentPosition = transform.position;

        // Handle animation based on movement
        UnityEngine.Vector3 movement = currentPosition - previousPosition;
        HandleAnimation(movement);

        previousPosition = currentPosition;
    }

    private bool IsCarTooCloseToStop(UnityEngine.Vector3 directionOfMovement)
    {
        // Snap the raycast direction to the closest cardinal direction (up, down, left, right)
        UnityEngine.Vector3 rayDirection = SnapToCardinalDirection(directionOfMovement);

        // Calculate the collider bounds
        Bounds colliderBounds = myCollider.bounds;

        // Adjust the ray start position based on the movement direction
        UnityEngine.Vector3 rayStartPosition = colliderBounds.center + rayDirection * (colliderBounds.extents.y + raycastStartOffset);

        // Center the ray horizontally when moving up or down, and vertically when moving left or right
        if (rayDirection == UnityEngine.Vector3.up || rayDirection == UnityEngine.Vector3.down) {
            rayStartPosition.x = colliderBounds.center.x;  // Set x to center when moving up/down
        } else if (rayDirection == UnityEngine.Vector3.left || rayDirection == UnityEngine.Vector3.right) {
            rayStartPosition.y = colliderBounds.center.y;  // Set y to center when moving left/right
        }

        // Determine ray length based on movement direction
        float rayLength = minimumStopDistance;
        if (rayDirection == UnityEngine.Vector3.up || rayDirection == UnityEngine.Vector3.down) {
            rayLength *= 2;  // Double the length if moving vertically
        }

        // Cast a ray from the offset position in the snapped cardinal direction
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, rayDirection, rayLength, targetLayerMask);

        if (hit.collider != null && hit.collider != myCollider && hit.collider.CompareTag("Car")) {
            // Draw the ray as red if it hits another car within the minimum distance
            Debug.DrawRay(rayStartPosition, rayDirection * rayLength, Color.red);
            return true;  // Car in front
        }

        // Draw the ray as green if there's no hit or it's only detecting itself
        Debug.DrawRay(rayStartPosition, rayDirection * rayLength, Color.green);
        return false;  // No car in front
    }

    private UnityEngine.Vector3 SnapToCardinalDirection(UnityEngine.Vector3 direction)
    {
        // Compare the x and y magnitudes to snap to the closest cardinal direction
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            return direction.x > 0 ? UnityEngine.Vector3.right : UnityEngine.Vector3.left;
        } else {
            return direction.y > 0 ? UnityEngine.Vector3.up : UnityEngine.Vector3.down;
        }
    }

    private void HandleAnimation(UnityEngine.Vector3 movement)
    {
        // Determine if car is moving horizontally or vertically
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y)) {
            if (movement.x < 0) {
                GetComponent<SpriteRenderer>().flipX = true;
            } else {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            anim.SetBool("front", false);
            anim.SetBool("side", true);
        } else if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x)) {
            anim.SetBool("front", true);
            anim.SetBool("side", false);
        }
    }

    public void MarkVehicleAsFueled(float time)
    {
        carWaitTime = time;

        if (selectedPump.Station.HasWorker) {
            // Auto-collect if the station has a worker
            OnMoneyIconButtonPressed();
        } else {
            // Show the money icon for manual collection if no worker
            SoundManager.Instance.PlaySound("moneyIcon", 0.5f, false);
            ShowMoneyIcon();
        }
    }

    private void ShowMoneyIcon()
    {
        moneyIcon.SetActive(true);
    }

    public void OnMoneyIconButtonPressed()
    {
        // Hide the money icon
        moneyIcon.SetActive(false);
        SoundManager.Instance.PlaySound("buttonClick", 1f, false);

        // Show the income popup and calculate income
        moneyAmountPopup.enabled = true;

        // Retrieve income per second from the GasStationManager
        float incomePerSecond = selectedPump.Station.instanceData.incomePerSecond;
        //Debug.Log("Income Per Second: " + incomePerSecond);
        //Debug.Log("Car Wait Time: " + carWaitTime);

        // Calculate income generated for this car
        BigInteger incomeGenerated = new BigInteger(Math.Ceiling(incomePerSecond * carWaitTime));


        // Display income generated in popup
        moneyAmountPopup.text = "$ " + incomeGenerated.ToString();

        // Hide the popup after a short delay
        StartCoroutine(PopupDelay());
    }



    private IEnumerator PopupDelay()
    {
        isFueled = true;
        selectedPump.Station.instanceData.pumpOccupied[selectedPump.PumpIndex] = false;
        StartCar();

        yield return new WaitForSeconds(2f);
        moneyAmountPopup.enabled = false;
    }

    public void StopCar()
    {
        isStopped = true;
        canMove = false;
    }

    public bool IsStopped()
    {
        return isStopped;
    }

    public void StartCar()
    {
        isStopped = false;
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Delete")) {
            Destroy(gameObject);
        }
    }
}
