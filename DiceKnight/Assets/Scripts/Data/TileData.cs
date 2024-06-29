using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData : MonoBehaviour
{
    //������ �ϴ� ������
    // x, y, �ֻ��� ���� ����, Ÿ���� ����

    public int x;
    public int y;
    public bool attackTargeted { get; set; }

    private SpriteRenderer spriteRenderer;

    private Coroutine targetedCo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public (int, int) GetXY()
    {
        return (x, y);
    }

    public void SetBlinking()
    {
        attackTargeted = true;
        if (targetedCo != null) StopCoroutine(targetedCo);
        targetedCo = StartCoroutine(targetedBlinkCo());
    }

    public void UnsetBlinking()
    {
        attackTargeted = false;
        if (targetedCo != null)
        {
            StopCoroutine(targetedCo);
            targetedCo = null;
        }
        spriteRenderer.color = Color.white;
    }

    //���� ����� ������� ȿ��
    public void Shatter(Vector3 _direction)
    {
        StartCoroutine(shatterTileCo(_direction));
    }

    private IEnumerator targetedBlinkCo()
    {
        float time = 0f;
        bool switcher = true;
        while (attackTargeted)
        {
            time += Time.deltaTime * 2f;

            if (switcher)
            {
                spriteRenderer.color = Color.Lerp(Color.white, Color.red, time);
            }
            else
            {
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, time);
            }

            if (time >= 1f)
            {
                time = 0f;
                switcher = !switcher;
            }

            yield return new WaitForEndOfFrame();
        }

        spriteRenderer.color = Color.white;
        targetedCo = null;
        yield break;
    }

    //���� ����� Ÿ�� �ı�
    private IEnumerator shatterTileCo(Vector3 _direction)
    {
        float time = 0;
        Vector3 startPos = transform.position;

        while (true)
        {
            time += Time.deltaTime * 0.5f;

            transform.position = Vector3.Lerp(startPos, _direction * 100, time);

            if (time >= 1f) break;

            yield return new WaitForEndOfFrame();
        }

        transform.position = _direction * 100;

        yield break;
    }
}
