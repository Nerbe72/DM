using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    public static DebugMode Instance;

    [SerializeField] private GameObject indicator;
    [SerializeField] private Button minus;
    [SerializeField] private Button plus;
    [SerializeField] private TMP_Text multiplyText;
    [SerializeField] private Button skip;

    [SerializeField] private GameObject actionBar;
    [SerializeField] private GameObject nextBtn;

    [SerializeField] private TMP_Text status;

    public int MultiplyDMG;

    private Coroutine statusCo;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        MultiplyDMG = 1;

        indicator.SetActive(false);
        plus.onClick.AddListener(Plus);
        minus.onClick.AddListener(Minus);
        skip.onClick.AddListener(SkipTurn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            indicator.SetActive(!indicator.activeSelf);
        }
    }

    private void Minus()
    {
        MultiplyDMG = Math.Clamp(MultiplyDMG - 1, 0, 30);
        multiplyText.text = "x" + MultiplyDMG;
        ShowStatus($"����{MultiplyDMG} ����");
    }

    private void Plus()
    {
        MultiplyDMG = Math.Clamp(MultiplyDMG + 1, 0, 30);
        multiplyText.text = "x" + MultiplyDMG;
        ShowStatus($"����{MultiplyDMG} ����");
    }

    private void SkipTurn()
    {
        if (StageManager.Instance.GetTurn() == Turn.PlayerAttack)
        {
            ShowStatus("�÷��̾� �������� ��ŵ�� �� �����ϴ�");
            return;
        }

        actionBar.SetActive(false);
        nextBtn.SetActive(false);
        PlayerDiceManager.Instance.UnSelectDice();
        ShowStatus($"{StageManager.Instance.GetTurn()}�� ��ŵ");
        StageManager.Instance.NextTurn();
    }

    private void ShowStatus(string _text)
    {
        if (statusCo != null)
            StopCoroutine(statusCo);
        status.text = _text;
        statusCo = StartCoroutine(statusAnimationCo());
    }

    private IEnumerator statusAnimationCo()
    {
        float time = 0;
        
        while (true)
        {
            time += Time.deltaTime * 1.2f;

            status.color = Color.Lerp(Color.red, Color.clear, time);

            yield return new WaitForEndOfFrame();

            if (time >= 1)
            {
                break;
            }
        }

        status.color = Color.clear;
        statusCo = null;
        yield break;
    }
}
