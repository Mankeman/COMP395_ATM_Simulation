using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//New as of Feb.25rd

public class ServiceProcess : MonoBehaviour
{
    public GameObject ATMService;
    public Transform ExitPlace;

    public float serviceRateAsPeoplePerHour = 25; // car/hour
    public float interServiceTimeInHours; // = 1.0 / ServiceRateAsCarsPerHour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    //public float ServiceRateAsCarsPerHour = 20; // car/hour
    public bool generateServices = false;

    public bool inService = false;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;
    //

    //New as Feb.25th
    //CarController carController;
    QueueManager queueManager; //=new QueueManager();

    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.UniformIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        interServiceTimeInHours = 1.0f / serviceRateAsPeoplePerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;
        //queueManager = this.GetComponent<QueueManager>();
        //queueManager = new QueueManager();
        //StartCoroutine(GenerateServices());
    }
    private void FixedUpdate()
    {
        if(inService == false)
        {
            GameObject[] peoples = GameObject.FindGameObjectsWithTag("person");
            foreach (GameObject people in peoples)
            {
                if (Vector3.Distance(people.transform.position, Waypoints.points[Waypoints.points.Length - 1].transform.position) <= 0.1f)
                {
                    people.GetComponent<PeopleController>().targetPeople = this.transform;
                    int waypoint = people.GetComponent<PeopleController>().wavepointIndex;
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_SP
        print("ServiceProcess.OnTriggerEnter:otherID=" + other.gameObject.GetInstanceID());
#endif
        if (inService)
        {
            return;
        }
        if (other.gameObject.tag == "people")
        {
            ATMService = other.gameObject;
            ATMService.GetComponent<PeopleController>().SetInService(true);

            //if (queueManager.Count() == 0)
            //{
            //    queueManager.Add(carInService);
            //}
            inService = true;
            generateServices = true;;
            //carController = carInService.GetComponent<CarController>();
            StartCoroutine(GenerateServices());
        }
    }

    IEnumerator GenerateServices()
    {
        while (generateServices)
        {
            //Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsPeoplePerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;

            }

            //New as of Feb.23rd
            //float timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds,maxInterServiceTimeInSeconds);
            generateServices = false;
            yield return new WaitForSeconds(timeToNextServiceInSec);

            //yield return new WaitForSeconds(interServiceTimeInSeconds);

        }
        ATMService.GetComponent<PeopleController>().ExitService(ExitPlace);

    }

}
