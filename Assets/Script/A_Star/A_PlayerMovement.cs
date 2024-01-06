using UnityEngine;

public class A_PlayerMovement : MonoBehaviour
{
    public float speed = 3;
    public float stoppingDistance = 0.5f;
    private Camera _camera;
    private Vector3[] _targetPath;
    private int _indexPath = 0;

    private void Start()
    {
        if (_camera == null && Camera.main != null)
            _camera = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            SetNewTarget();

        if(_targetPath == null )
            return;

        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (_indexPath  >=  _targetPath.Length)
            return;

        RotateToTarget(_targetPath[_indexPath]);
        transform.position = Vector3.MoveTowards(transform.position, _targetPath[_indexPath], speed * Time.deltaTime);
        float distanceToTheNextWayPoint = Vector3.Distance(transform.position, _targetPath[_indexPath]);
        float distanceToFinaltWayPoint= Vector3.Distance(transform.position, _targetPath[_targetPath.Length - 1]);

        if (distanceToTheNextWayPoint < 0.05f)
            _indexPath++;

        if(distanceToFinaltWayPoint < stoppingDistance)
            _indexPath = _targetPath.Length;
    }

    private void RotateToTarget(Vector3 target)
    {
        transform.LookAt(target);
    }


    private void SetNewTarget()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 newHitPoint = new Vector3(hit.point.x, 0.5f, hit.point.z);
            PathRequest pathRequest = new PathRequest(transform.position, newHitPoint, OnRequestReceived);
            A_Manager.Instance.Request(pathRequest);
        }
    }

    private void OnRequestReceived(Vector3[] path, bool succes)
    {
        _targetPath = path;
        _indexPath = 0;
    }
}