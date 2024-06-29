using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public Camera mainCamera;

    public Vector3 cameraPosition; // 0, 0, -10

    private Coroutine shakeChecker;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        mainCamera = Camera.main;
        cameraPosition = mainCamera.transform.position;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_time">�����ð�</param>
    /// <param name="_magnitude">��������</param>
    /// <param name="_lerpTime">���������ð�</param>
    public void ShakeCamera(float _time = 0.15f, float _magnitude = 0.15f, float _lerpTime = 0.04f)
    {
        if (shakeChecker != null)
            StopCoroutine(shakeChecker);
        shakeChecker = StartCoroutine(shakeCamCo(_time, _magnitude, _lerpTime));
    }

    private IEnumerator shakeCamCo(float _time, float _magnitude, float _lerpTime)
    {
        _time -= _lerpTime;

        //ī�޶� ��鸲
        while (_time > 0)
        {
            _time -= Time.deltaTime;

            mainCamera.transform.position = new Vector3(Random.Range(-_magnitude, _magnitude),
                                                        Random.Range(-_magnitude, _magnitude), -1);

            yield return new WaitForEndOfFrame();
        }

        Vector3 endPos = mainCamera.transform.position;
        float maxLerpTime = _lerpTime * 10;
        float currentLerpTime = 0;

        //ī�޶� ��ġ ����
        while (currentLerpTime < maxLerpTime)
        {

            mainCamera.transform.position = Vector3.Lerp(endPos, cameraPosition, currentLerpTime);
            currentLerpTime += Time.deltaTime * 10;
            yield return new WaitForEndOfFrame();
        }

        mainCamera.transform.position = -Vector3.forward;
        shakeChecker = null;
        yield break;
    } 
}
