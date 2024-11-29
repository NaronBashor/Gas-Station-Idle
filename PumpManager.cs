using System.Collections;
using UnityEngine;

public class PumpManager : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private float pumpingTime = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Car")) {
            // Stop the car's movement
            CarController carController = collision.GetComponent<CarController>();
            if (carController != null) {
                carController.StopCar();
                SoundManager.Instance.PlaySound("carFill", 0.25f, false);
                StartCoroutine(StartPumpingRoutine(carController));
            }
        }
    }

    private IEnumerator StartPumpingRoutine(CarController carController)
    {
        // Wait until the car is fully stopped
        yield return new WaitUntil(() => carController.IsStopped());

        // Set canPump to true once the car is stopped
        parentObject.GetComponent<GasStationManager>().vehiclesFueling.Add(1);

        // Wait for the pumping time
        float time = Random.Range(2, pumpingTime);
        yield return new WaitForSeconds(time);

        // Resume the car's movement and set canPump to false
        carController.MarkVehicleAsFueled(time);
        //carController.StartCar();
        parentObject.GetComponent<GasStationManager>().vehiclesFueling.RemoveAt(0);
    }
}
