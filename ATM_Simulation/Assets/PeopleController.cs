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

    public bool InService { get; set; }
    public GameObject ATM;
    public QueueManager queueManager;

    public enum PersonState
    {
        None=-1,
        Entered,  //going towards the DriveThruWindow (don't bump into fron cars)
        InService,
        Serviced
    }
    public PersonState personState = PersonState.None;
    // Start is called before the first frame update
    void Start()
    {
        ATM = GameObject.FindGameObjectWithTag("ATM");
        targetWindow = ATM.transform;
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
        //        //queueManager = driveThruWindow.GetComponent<QueueManager>();
        //        GameObject goLast = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>().Last();
        //        if (goLast)
        //        {
        //#if DEBUG_CC
        //            print("CC.DoEntered: goLast.ID=" + goLast.GetInstanceID());
        //#endif
        //            targetCar = goLast.transform;
        //        }
        //        else
        //        {
        //            targetCar = targetWindow;
        //        }

        targetPeople = targetWindow;

        queueManager = GameObject.FindGameObjectWithTag("ATM").GetComponent<QueueManager>();
        queueManager.Add(this.gameObject);

        navMeshAgent.SetDestination(targetPeople.position);
        navMeshAgent.isStopped = false;
    }
    void DoInService()
    {
        navMeshAgent.isStopped = true;
        //this.transform.position = targetWindow.position;
        //this.transform.rotation = Quaternion.identity;
    }
    void DoServiced()
    {
        navMeshAgent.SetDestination(targetExit.position);
        navMeshAgent.isStopped = false;
    }
    public void ChangeState(PersonState newPersonState)
    {
        this.personState = newPersonState;
        FSMPerson();
    }

    public void FixedUpdate()
    {

//        if (carState == CarState.Entered)
//        {
//            if (targetCar == null)
//            {
//#if DEBUG_CC
//            print("***** CarController.FixedUpdate:targetCar.pos=" + targetCar.position);
//#endif
//                targetCar = targetWindow;
//                //navMeshAgent.SetDestination(targetCar.position);
//                navMeshAgent.isStopped = false;
//            }
//        }

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
            //SetInService(true);
        }
        else if (other.gameObject.tag == "Exit")
        {
            Destroy(this.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
#if DEBUG_CC
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetCar.gameObject.GetInstanceID());
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetExit.gameObject.GetInstanceID());

#endif
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, targetWindow.transform.position);
        if (targetPeople)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, targetPeople.transform.position);

        }
        if (targetExit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetExit.transform.position);

        }


    }

}
