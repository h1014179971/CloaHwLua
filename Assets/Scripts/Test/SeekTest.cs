using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SeekTest : MonoBehaviour
{
    public Transform _target;
    private Seeker _seeker;
    private List<Vector3> _vectorPath = new List<Vector3>();
    private int _pathIndex = 0;
    private Vector3 _nextTargetPoint;
    private Vector3 _offset;
    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _seeker.pathCallback += OnPathComplete;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        _seeker.StartPath(transform.position, _target.position);
    }
    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            _vectorPath = p.vectorPath;
            _vectorPath.Add(_target.position);
            NextPoint();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(_vectorPath.Count > 0)
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * 2, Time.deltaTime);
            if (v3.magnitude >= (_nextTargetPoint - transform.position).magnitude)
            {
                transform.position = _nextTargetPoint;
                NextPoint();
            }  
            else
                transform.position += v3;
        }
    }
    private void NextPoint()
    {
        if (_pathIndex >= _vectorPath.Count) return;
        _nextTargetPoint = _vectorPath[_pathIndex];
        _offset = (_nextTargetPoint - transform.position).normalized;
        _pathIndex++;
    }
}
