using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Obstacle : MonoBehaviour
{
    [SerializeField] private Transform _spawn;
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _object;
    public float _angleInDegrees;
    private float _g = Physics.gravity.y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _spawn.localEulerAngles = new Vector3(-_angleInDegrees, 0f, 0f);

        Invoke("Shot", 1f);
    }
    public void Shot()
    {
        Vector3 _fromTo = _player.position - transform.position;
        Vector3 _fromToXZ = new Vector3(_fromTo.x, 0f, _fromTo.z);

        this.transform.rotation = Quaternion.LookRotation(_fromToXZ, Vector3.up);


        float _x = _fromToXZ.magnitude;
        float _y = _fromTo.y;

        float _angleInRadians = _angleInDegrees * Mathf.PI / 180;

        float _v2 = (_g * _x * _x) / (2 * (_y - Mathf.Tan(_angleInRadians) * _x) * Mathf.Pow(Mathf.Cos(_angleInRadians), 2));
        float _v = Mathf.Sqrt(Mathf.Abs(_v2));

        GameObject newBullet = Instantiate(_object, _player.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().velocity = _player.forward * _v;
    }
}
