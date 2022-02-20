using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PeopleController : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform targetWindow;
    public Transform targetPeople=null;
    public Transform targetExit = null;

    //Entering and exiting the ATM
    private int atmServiceChoice = 1;
    public float timer = 60f;
    private bool timerOn = false;
    private bool reachedATM = false;

    //Which ATM? and where's the lineup?
    public int wavepointIndex = 0;
    public bool reachedEndOfLine = false;
    public bool InService { get; set; }
    public GameObject ATM;
    public QueueManager queueManager;

    public enum PersonState
    {
        None=-1,
        Entered,  //going towards the ATM
        InService,
        Serviced
    }
    public PersonState personState = PersonState.None;
    // Start is called before the first frame update
    void Start()
    {
        ATM = GameObject.FindGameObjectWithTag("ATM");
        targetWindow = Waypoints.points[0];
        targetExit = GameObject.FindGameObjectWithTag("Exit").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
#if DEBUG_CC
        print("Start: this.GO.ID=" + this.gameObject.GetInstanceID());
#endif

        //
        personState = PersonState.Entered;
        FSMPerson();

    }

    void FSMPerson()
    {
#if DEBUG_CC
        print("CC.FSMCar:state="+carState+",ID="+this.gameObject.GetInstanceID());
#endif
        switch (personState)
        {
            case PersonState.None: //do nothing - shouldn't happen
                break;
            case PersonState.Entered:
                DoEntered();
                break;
            case PersonState.InService:
                DoInService();
                break;
            case PersonState.Serviced:
                DoServiced();
                break;
            default:
                print("personState unknown!:" + personState);
                break;

        }
    }
    void DoEntered()
    {
        targetPeople = targetWindow;

        queueManager = GameObject.FindGameObjectWithTag("ATM").GetComponent<QueueManager>();
        queueManager.Add(this.gameObject);

        navMeshAgent.SetDestination(targetPeople.position);
        navMeshAgent.isStopped = false;


    }
    private void Update()
    {
        if(reachedEndOfLine)
        {
            return;
        }
        navMeshAgent.SetDestination(targetPeople.position);
        navMeshAgent.isStopped = false;
        if (Vector3.Distance(targetPeople.position, this.transform.position) <= 0.1f)
        {
            GetNextWaypoint();
        }
    }
    void DoInService()
    {
        navMeshAgent.isStopped = true;
        ATMServiceTimer();
        //this.transform.position = targetWindow.position;
        //this.transform.rotation = Quaternion.identity;
    }
    void DoServiced()
    {
        navMeshAgent.SetDestination(targetExit.position);
        targetPeople = targetExit;
        navMeshAgent.isStopped = false;
    }
    void ATMServiceTimer()
    {
        if (reachedATM == false)
        {
            atmServiceChoice = Random.Range(1, 3);
            switch (atmServiceChoice)
            {
                case 1:
                    timer = Random.Range(45, 51);
                    timerOn = true;
                    reachedATM = true;
                    break;
                case 2:
                    timer = Random.Range(20, 31);
                    timerOn = true;
                    reachedATM = true;
                    break;
                default:
                    timer = Random.Range(20, 51);
                    timerOn = true;
                    reachedATM = true;
                    break;
            }
        }
    }
    public void ChangeState(PersonState newPersonState)
    {
        this.personState = newPersonState;
        FSMPerson();
    }

    public void FixedUpdate()
    {
        if (reachedEndOfLine)
        {
            CheckNextWaypoint();
        }
        if(timerOn)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                ChangeState(PersonState.Serviced);
                timer = 50f;
                timerOn = false;
            }
        }    
    }
    public void SetInService(bool value)
    {
        //Chaneg        InService = value;
        //if (InService)
        //{
        //    navMeshAgent.isStopped=true;
        //}
    }
    public void ExitService(Transform target)
    {
        //this.SetInService(false);
        
        queueManager.PopFirst();
        ChangeState(PersonState.Serviced);
        //targetExit = target;

        //navMeshAgent.SetDestination(target.position);
        //navMeshAgent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_CC
        Debug.LogFormat("CarController(this={0}).OnTriggerEnter:other={1}",this.gameObject.GetInstanceID(), other.gameObject.tag);
#endif
        if (other.gameObject.tag == "person")
        {
            //this.navMeshAgent.desiredVelocity.
            //if (targetCar == null)
            //{
                //targetCar = other.gameObject.transform;
                //navMeshAgent.SetDestination(targetCar.position);
            //}
        }
        else if (other.gameObject.tag == "ATM")
        {
            ChangeState(PersonState.InService);
            other.GetComponent<ServiceProcess>().inService = true;
            //SetInService(true);
        }
        else if (other.gameObject.tag == "Exit")
        {
            Destroy(this.gameObject);
        }
        else if(other.gameObject.tag == "Waypoints")
        {
            GetNextWaypoint();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ATM")
        {
            other.GetComponent<ServiceProcess>().inService = false;
            other.GetComponent<ServiceProcess>().generateServices = false;
        }
    }
    void GetNextWaypoint()
    {
        if (reachedEndOfLine || wavepointIndex >= Waypoints.points.Length - 1)
        {
            return;
        }
        wavepointIndex++;
        targetPeople = Waypoints.points[wavepointIndex];
        GameObject[] peoples = GameObject.FindGameObjectsWithTag("person");
        foreach (GameObject people in peoples)
        {
            if(Vector3.Distance(people.transform.position,targetPeople.transform.position) <=0.1f)
            {
                reachedEndOfLine = true;
                wavepointIndex--;
                targetPeople = Waypoints.points[wavepointIndex];
                navMeshAgent.isStopped = true;
                break;
            }
        }
        navMeshAgent.SetDestination(targetPeople.position);
    }
    void CheckNextWaypoint()
    {
        GameObject[] peoples = GameObject.FindGameObjectsWithTag("person");
        int count = 0;
        foreach (GameObject person in peoples)
        {
            count++;
            if(Vector3.Distance(person.transform.position, targetPeople.transform.position) <= 0.1f)
            {
                count--;
                break;
            }
        }
        Debug.Log("Foreach loop done");
        if(count != peoples.Length)
        {
            targetPeople = Waypoints.points[wavepointIndex + 1];
            navMeshAgent.SetDestination(targetPeople.position);
            navMeshAgent.isStopped = false;
        }

    }

}
