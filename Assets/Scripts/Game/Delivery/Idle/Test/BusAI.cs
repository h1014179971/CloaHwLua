using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BusAI : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Seeker _seeker;
    private List<Vector3> _vectorPath = new List<Vector3>();
    private Vector3 _nextPoint;
    private Vector3 _offset;
    private float _speed = 2;
    private int _pathIndex;
    private float _moveNextDist = 0.1f;//1等于100Pixel 
    public Transform target;
    public SpriteRenderer Sprite
    {
        get {
            if (_sprite == null)
                _sprite = GetComponent<SpriteRenderer>();
            return _sprite;
        }
    }
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _seeker.pathCallback += OnPathComplete;
        _seeker.StartPath(transform.position,target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _seeker.StartPath(transform.position, worldPoint); 
        }
        if (_pathIndex < _vectorPath.Count)
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed, Time.deltaTime);
            transform.position += v3;
            if ((_nextPoint - transform.position).sqrMagnitude < _moveNextDist)//长度平方小于一定值判断到达目标点
            {
                _pathIndex++;
                if(_pathIndex < _vectorPath.Count)
                {
                    NextPoint();
                }
            }
        }
    }
    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            Debug.Log($"vectorPath ==={p.vectorPath}==={p.vectorPath.Count}");
            _vectorPath = p.vectorPath;
            _pathIndex = 0;
            NextPoint();
        }
    }
    private void NextPoint()
    {
        _nextPoint = _vectorPath[_pathIndex];
        _offset = _nextPoint - transform.position;
        _nextPoint.z = 0;
        _offset.z = 0;
        if (_offset.x > 0)
            Sprite.flipX = false;
        else
            Sprite.flipX = true;
    }

    private void OnDestroy()
    {
        _seeker.pathCallback -= OnPathComplete;
    }

}
