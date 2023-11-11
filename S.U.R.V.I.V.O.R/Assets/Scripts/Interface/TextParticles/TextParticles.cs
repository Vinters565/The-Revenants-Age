using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextParticles : MonoBehaviour
{
    private const float DURATION = 1f;
    private const int LIFTING_HEIGHT = 30;
    private const string PARTICLE_PREFAB_PATH = "Interface/Prefabs/TextParticles/TextParticle";

    public static TextParticles Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
            Destroy(gameObject);
    }

    public void LaunchParticle(float value, Vector2 position, TextMeshProUGUI tmp)
    {
        if (!tmp.gameObject.activeInHierarchy || value == 0) return;

        var particle = CreateParticle(value, position, tmp);
        StartCoroutine(MoveParticle(position, particle));
    }

    private IEnumerator MoveParticle(Vector2 position, GameObject particle)
    {
        var currentPosition = position;
        var targetPosition = position + (Vector2.up * LIFTING_HEIGHT);

        var currentTime = 0f;

        while (currentTime < DURATION)
        {
            currentTime += Time.deltaTime;
            particle.transform.position = new Vector3(currentPosition.x,
                Mathf.Lerp(currentPosition.y, targetPosition.y, currentTime / DURATION));
            yield return null;
        }

        Destroy(particle);
    }

    private GameObject CreateParticle(float value, Vector3 position, TextMeshProUGUI tmp)
    {
        var newParticle = Instantiate(Resources.Load<GameObject>(PARTICLE_PREFAB_PATH), position, Quaternion.identity);
        var particleTpm = newParticle.GetComponent<TextMeshProUGUI>();

        particleTpm.transform.SetParent(tmp.transform.parent);
        newParticle.GetComponent<RectTransform>().sizeDelta =
            (tmp.GetComponent<RectTransform>().sizeDelta * tmp.transform.localScale + (tmp.fontSize * Vector2.up));

        particleTpm.fontSize = tmp.fontSize;
        particleTpm.text = Math.Round(value).ToString();
        particleTpm.color = value > 0 ? Color.green : Color.red;

        return newParticle;
    }
}