using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
public class Sword_Controller : MonoBehaviour
{
    [SerializeField]private GameObject cube;
    [SerializeField]private GameObject abstractPlayer;
    [SerializeField]private float speedRotate;
    [SerializeField]private float speedMove;
    [SerializeField]private Transform blade;
    [SerializeField]private Transform sword;
    [SerializeField]private float powerExplosion;
    [SerializeField]private float radiusExplosion;
    [SerializeField]private LayerMask layerMask;
    private Vector3  offset,moveDirection;
    private float swordYAngle, swordXAngle,swordZAngle;
    private float h,v;
    private float zDir;

    private void Update()
    {

    }
    private void FixedUpdate()
    {
        MoveSword();
        Slice();
    }
    private void Slice()
    {
        Collider[] hits = Physics.OverlapBox(blade.position, new Vector3(0.02611581f / 2, 0.08135936f / 2, 1.454692f / 2), blade.rotation, layerMask); ;
        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, hits[i].gameObject.GetComponent<Renderer>().material);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, hits[i].gameObject.GetComponent<Renderer>().material);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, hits[i].gameObject.GetComponent<Renderer>().material);
                AddHullComponents(bottom);
                AddHullComponents(top);
                Destroy(hits[i].gameObject);
            }
        }
    }
    private void AddHullComponents(GameObject go)
    {
        go.layer = 3;
        go.tag = "Sliced";
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;
        rb.AddExplosionForce(powerExplosion, go.transform.position,radiusExplosion);

        Destroy(go, 5);
    }
    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial=null)
    {
        if (obj.GetComponent<MeshFilter>() == null)
            return null;
        return obj.Slice(blade.position, blade.right, crossSectionMaterial);
    }
    private void MoveSword()
    {
        h = Input.GetAxis("Mouse X");
        v = Input.GetAxis("Mouse Y");
        zDir = sword.transform.position.z - Camera.main.transform.position.z;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            offset = sword.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDir));
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch touch = Input.touches[0];
            h = touch.deltaPosition.x;
            v = touch.deltaPosition.y;
            moveDirection = new Vector3(h, 0, v);
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDir);
            Vector3 curPosition = new Vector3((Camera.main.ScreenToWorldPoint(curScreenPoint).x + offset.x), (Camera.main.ScreenToWorldPoint(curScreenPoint).y + offset.y), zDir);

            float _newX = Mathf.Clamp(curPosition.x, -1.5f, 1.5f);
            float _newY = Mathf.Clamp(curPosition.y, 2f, 6f);

            sword.transform.position = Vector3.Lerp(sword.transform.position, new Vector3(_newX, _newY, abstractPlayer.transform.position.z + 2), speedMove * Time.deltaTime);
            RotateSword();
        }
        
    }
    private void RotateSword()
    {
        swordYAngle += h * speedRotate * Time.deltaTime;
        swordYAngle = Mathf.Clamp(swordYAngle, -75, 75);
        swordXAngle += v * speedRotate * -Time.deltaTime;
        swordXAngle = Mathf.Clamp(swordXAngle, -65, 65);
        Quaternion TargetRot = Quaternion.LookRotation(moveDirection);
        sword.transform.rotation = Quaternion.RotateTowards(sword.transform.rotation, TargetRot, speedRotate);
        swordZAngle = -sword.transform.eulerAngles.y; 
        sword.localRotation = Quaternion.Euler(new Vector3(swordXAngle, swordYAngle, swordZAngle));
    }
    private void Debuging()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
    private float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

}
