using System.Linq;
using UnityEngine;

public class A_PlayerMovement : MonoBehaviour
{
    public float speed = 3;
    public float stoppingDistance = 0.5f;
    private Camera _camera;
    private Vector3[] _targetPath;
    private int _indexPath = 0;
    public bool isNowPlayer;
    private Vector2 _randPos = Vector2.zero;

    private void Start()
    {
        if (_camera == null && Camera.main != null)
            _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isNowPlayer)
                A_Manager.Instance.mainPlayerMove = true;
            
            if (isNowPlayer)
                SetNewTarget();
        }
        
        if (Input.GetMouseButtonDown(1) && isNowPlayer)
        {
            CreateNewObstacle();
        }

        if (isNowPlayer)
            A_Manager.Instance.mainPlayerPos = this.transform.position;
        
        if (!isNowPlayer && A_Manager.Instance.mainPlayerMove)
            SetNewTargetFromAnotherUnits();
        
        if(_targetPath == null )
            return;
        
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (_indexPath >= _targetPath.Length)
            return;

        RotateToTarget(_targetPath[_indexPath]);
        transform.position = Vector3.MoveTowards(transform.position, _targetPath[_indexPath], speed * Time.deltaTime);
        float distanceToTheNextWayPoint = Vector3.Distance(transform.position, _targetPath[_indexPath]);
        float distanceToFinaltWayPoint= Vector3.Distance(transform.position, _targetPath[_targetPath.Length - 1]);

        if (distanceToTheNextWayPoint < 0.05f)
            _indexPath++;

        if (distanceToFinaltWayPoint < stoppingDistance)
        {
            if (isNowPlayer)
                A_Manager.Instance.mainPlayerMove = false;

            if (!isNowPlayer && _randPos != Vector2.zero)
                _randPos = Vector2.zero;
            
            _indexPath = _targetPath.Length;
        }
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

    private void CreateNewObstacle()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 newHitPoint = new Vector3(hit.point.x, 0.5f, hit.point.z);
            GameObject newObstacleCreated = Instantiate(A_Manager.Instance.newObstacle, newHitPoint, Quaternion.identity);
            newObstacleCreated.transform.SetParent(A_Manager.Instance.obstacleParent.transform);
            A_Manager.Instance.obstaclesList.Add(newObstacleCreated);
            
            var cachedArray = Save_Load_Call.Instance.saveObjectObstacle;
            
            Save_Load_Call.Instance.saveObjectObstacle = new Save_Object_Obstacle[Save_Load_Call.Instance.saveObjectObstacle.Length + 1];
            Save_Load_Call.Instance.saveObjectObstacle[^1] = new Save_Object_Obstacle();
            Save_Load_Call.Instance.saveObjectObstacle[^1].posX = newObstacleCreated.transform.position.x;
            Save_Load_Call.Instance.saveObjectObstacle[^1].posY = newObstacleCreated.transform.position.y;
            Save_Load_Call.Instance.saveObjectObstacle[^1].posZ = newObstacleCreated.transform.position.z;

            for (int i = 0; i < Save_Load_Call.Instance.saveObjectObstacle.Length - 1; i++)
                Save_Load_Call.Instance.saveObjectObstacle[i] = cachedArray[i];
            
            A_Manager.Instance.CalculateWorldMapBorders();
            A_Manager.Instance.GenerateGrid();
        }
    }
    
    private void SetNewTargetFromAnotherUnits()
    {
        if (_randPos == Vector2.zero)
            _randPos = Random.insideUnitSphere * 5;
        
        Vector3 newHitPoint = new Vector3(A_Manager.Instance.mainPlayerPos.x + _randPos.x,
            A_Manager.Instance.mainPlayerPos.y, A_Manager.Instance.mainPlayerPos.z + _randPos.y);
        PathRequest pathRequest = new PathRequest(transform.position, newHitPoint, OnRequestReceived);
        A_Manager.Instance.Request(pathRequest);
    }

    private void OnRequestReceived(Vector3[] path, bool succes)
    {
        _targetPath = path;
        _indexPath = 0;
    }
}