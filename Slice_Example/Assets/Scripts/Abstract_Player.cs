using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abstract_Player : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField]private float radius;
    [SerializeField]private LayerMask layerMask;
    [SerializeField]private bool isMove;
    private Rigidbody rb;
    private Vector3 tmpVect;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMove = true;
    }
    // Update is called once per frame
    private void Update()
    {
        if(isMove)
        {
            tmpVect = new Vector3(0, 0, 1);
            tmpVect = tmpVect.normalized * speed * Time.deltaTime;
            rb.MovePosition(this.transform.position + tmpVect);
            TimeSlow();
        }
        else if(!isMove)
        {
            rb.AddForce(Vector3.zero);
        }

        
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 3 && collision.gameObject.tag !="Sliced")
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        if(collision.gameObject.tag =="Finish")
        {
            isMove = false;
        }
    }
    private void TimeSlow()
    {
        if (Physics.CheckSphere(this.transform.position, radius, layerMask))
        {
            Time.timeScale = 0.5f;
        }
        else
            Time.timeScale = 1;
       
    }
    private void OnDrawGizmos()
    {

        Gizmos.DrawSphere(this.transform.position, radius);
        Gizmos.color = Color.green;
    }
    private void OnDrawGizmosSelected()
    {
        
    }
}
