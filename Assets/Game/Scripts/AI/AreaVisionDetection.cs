using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AreaVisionDetection : MonoBehaviour
{
	public delegate void VisionDetectionEvent(GameObject _objectDetected);
	public delegate void VisionLostEvent(GameObject _objectDetected);

	public event VisionDetectionEvent DetectionEvent;
	public event VisionLostEvent LostEvent;

	public void DispatchVisionDetectionEvent(GameObject _objectDetected)
	{
		if (DetectionEvent != null)
			DetectionEvent(_objectDetected);
	}

	public void DispatchVisionLostEvent(GameObject _objectDetected)
	{
		if (LostEvent != null)
			LostEvent(_objectDetected);
	}

	public enum TargetCharacters { PLAYER = 0, ENEMY, NPC }

	public Material Material;
	public float DetectionDistance = 8;
	public float DetectionAngle = 40;
	public float Orientation = 90;
	public float HeightToFloor = -0.75f;
	public TargetCharacters Target = TargetCharacters.PLAYER;

	private GameObject m_planeAreaVisionDetection;
	private int m_checkRadiusInstances = 10;

	private GameObject m_currentDetection = null;

	void Start()
    {
		if (m_planeAreaVisionDetection == null)
		{
			m_planeAreaVisionDetection = GameObject.CreatePrimitive(PrimitiveType.Plane);
			m_planeAreaVisionDetection.GetComponent<MeshCollider>().enabled = false;
			m_planeAreaVisionDetection.AddComponent<PlaneFromPoly>();
			m_planeAreaVisionDetection.transform.parent = this.transform;
		}

		DrawAreaVision(m_planeAreaVisionDetection, m_checkRadiusInstances, DetectionDistance, DetectionAngle, Material, HeightToFloor);
	}

	public float IsInsideCone(GameObject _source, float _angle, GameObject _objective, float _rangeDetection, float _angleDetection)
	{
		float distance = Vector3.Distance(new Vector3(_source.transform.position.x, 0, _source.transform.position.z),
										 new Vector3(_objective.transform.position.x, 0, _objective.transform.position.z));
		if (distance < _rangeDetection)
		{
			float yaw = _angle * Mathf.Deg2Rad;
			Vector2 pos = new Vector2(_source.transform.position.x, _source.transform.position.z);

			Vector2 v1 = new Vector2((float)Mathf.Cos(yaw), (float)Mathf.Sin(yaw));
			Vector2 v2 = new Vector2(_objective.transform.position.x - pos.x, _objective.transform.position.z - pos.y);

			// Angle detection
			float moduloV2 = v2.magnitude;
			if (moduloV2 == 0)
			{
				v2.x = 0.0f;
				v2.y = 0.0f;
			}
			else
			{
				v2.x = v2.x / moduloV2;
				v2.y = v2.y / moduloV2;
			}
			float angleCreated = (v1.x * v2.x) + (v1.y * v2.y);
			float angleResult = Mathf.Cos(_angleDetection * Mathf.Deg2Rad);

			if (angleCreated > angleResult)
			{
				return (distance);
			}
			else
			{
				return (-1);
			}
		}
		else
		{
			return (-1);
		}
	}

	private void DrawAreaVision(GameObject _planeAreaVision, int _checkRadiusInstances, float _viewDistance, float _angleView, Material _material, float _heightToFloor)
	{
		List<Vector3> areaDetection = new List<Vector3>();
		Vector3 posOrigin = this.gameObject.transform.position;
		posOrigin.y += _heightToFloor;
		areaDetection.Add(posOrigin);

		float totalAngle = 2 * _angleView * Mathf.Deg2Rad;
		float entryAngle = (Orientation + _angleView) * Mathf.Deg2Rad;
		float x = _viewDistance * Mathf.Cos(entryAngle);
		float z = _viewDistance * Mathf.Sin(entryAngle);

		Vector3 posTarget = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
		areaDetection.Add(posTarget);

		float thetaScale = totalAngle / _checkRadiusInstances;
		for (int i = 0; i < _checkRadiusInstances; i++)
		{
			entryAngle -= thetaScale;
			x = _viewDistance * Mathf.Cos(entryAngle);
			z = _viewDistance * Mathf.Sin(entryAngle);

			Vector3 posTargetRadius = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
			areaDetection.Add(posTargetRadius);
		}

		float endAngle = (Orientation - _angleView) * Mathf.Deg2Rad;
		x = _viewDistance * Mathf.Cos(endAngle);
		z = _viewDistance * Mathf.Sin(endAngle);
		Vector3 posTargetEnd = new Vector3(posOrigin.x + x, posOrigin.y, posOrigin.z + z);
		areaDetection.Add(posTargetEnd);
		areaDetection.Add(posOrigin);

		_planeAreaVision.GetComponent<PlaneFromPoly>().Init(areaDetection.ToArray(), _material);
		_planeAreaVision.GetComponent<PlaneFromPoly>().Logic(new Vector3(posOrigin.x, posOrigin.y, posOrigin.z), posOrigin.y);
	}

	void Update()
    {
		float angle = (Mathf.Atan2(this.transform.forward.x, -this.transform.forward.z) * Mathf.Rad2Deg) - Orientation;

		switch (Target)
        {
			case TargetCharacters.PLAYER:
				if (GameController.Instance.MyPlayer != null)
                {
					if (IsInsideCone(this.gameObject, angle, GameController.Instance.MyPlayer.gameObject, DetectionDistance, DetectionAngle) > 0)
					{
						if (m_currentDetection == null)
                        {
							Debug.Log("<color=red>PLAYER DETECTED!!!</color>");
							m_currentDetection = GameController.Instance.MyPlayer.gameObject;
							DispatchVisionDetectionEvent(GameController.Instance.MyPlayer.gameObject);
						}
					}
					else
                    {
						if (m_currentDetection != null)
                        {
							m_currentDetection = null;
							DispatchVisionLostEvent(GameController.Instance.MyPlayer.gameObject);
						}
					}
				}
				break;

			case TargetCharacters.ENEMY:
				if (LevelController.Instance != null)
                {
					for (int i = 0; i < LevelController.Instance.Enemies.Length; i++)
					{
						if (IsInsideCone(this.gameObject, angle, LevelController.Instance.Enemies[i].gameObject, DetectionDistance, DetectionAngle) > 0)
						{
							if (m_currentDetection == null)
							{
								Debug.Log("<color=red>ENEMY DETECTED!!!</color>");
								m_currentDetection = LevelController.Instance.Enemies[i].gameObject;
								DispatchVisionDetectionEvent(LevelController.Instance.Enemies[i].gameObject);
							}
						}
						else
                        {
							if (m_currentDetection != null)
							{
								if (m_currentDetection == LevelController.Instance.Enemies[i])
                                {
									m_currentDetection = null;
									DispatchVisionLostEvent(LevelController.Instance.Enemies[i].gameObject);
								}								
							}
						}
					}
				}
				break;

			case TargetCharacters.NPC:
				if (LevelController.Instance != null)
				{
					for (int i = 0; i < LevelController.Instance.NPCs.Length; i++)
					{
						if (IsInsideCone(this.gameObject, angle, LevelController.Instance.NPCs[i].gameObject, DetectionDistance, DetectionAngle) > 0)
						{
							if (m_currentDetection == null)
							{
								Debug.Log("<color=red>NPC DETECTED!!!</color>");
								m_currentDetection = LevelController.Instance.NPCs[i].gameObject;
								DispatchVisionDetectionEvent(LevelController.Instance.NPCs[i].gameObject);
							}
						}
						else
                        {
							if (m_currentDetection != null)
							{
								if (m_currentDetection == LevelController.Instance.NPCs[i])
								{
									m_currentDetection = null;
									DispatchVisionLostEvent(LevelController.Instance.NPCs[i].gameObject);
								}
							}
						}
					}
				}
				break;
        }
	}
}
