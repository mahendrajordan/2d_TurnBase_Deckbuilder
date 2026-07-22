using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InfoSpawner : MonoBehaviour
{
    Vector2 moveSpeed;
    TextMeshPro textMeshPro;


    void Update()
    {
        Vector2 currentPos = transform.position;
        currentPos += moveSpeed * Time.deltaTime;
        this.transform.position = currentPos;
    }

    public void Setup(string info, Vector2 speedVector, Color color)
    {
        textMeshPro = GetComponent<TextMeshPro>();
        
        textMeshPro.text = info;
        textMeshPro.color = color;
        moveSpeed = speedVector;     

        StartCoroutine(DestroyDelay());
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(.5f);

        float duration = .5f;
        float timer = 0;
        float lerpPoint = 0;

        Color startColor = textMeshPro.color;
        Color endColor = startColor;
        endColor.a = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer/duration;
            textMeshPro.color = Color.Lerp(startColor, endColor, lerpPoint);

            yield return null;
        }while(lerpPoint<1);

        Destroy(this.gameObject);
    }
    
}
