using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FigureAssetDatabase figureAssetDatabase;
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private Transform fieldTransform;
    [SerializeField] private Transform actionBar;

    public System.Action OnWin;
    public System.Action OnLose;

    private List<GameObject> figuresInField = new();
    private List<GameObject> figuresInActionBar = new();
    private readonly int maxActionBarSize = 7;
    private readonly Vector3[] actionBarSlots = new Vector3[7];

    private void Awake()
    {
        const float slotSpacing = 0.75f;
        for (int i = 0; i < maxActionBarSize; i++)
        {
            actionBarSlots[i] = new Vector3((i - (maxActionBarSize - 1) / 2f) * slotSpacing, 0, 0);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
        
        if (figureAssetDatabase == null || figurePrefab == null)
        {
            Debug.LogError("FigureAssetDatabase or FigurePrefab is not assigned!");
            return;
        }

        List<FigureAttributes.FigureConfig> figureConfigs = GenerateFigureSet(27);
        StartCoroutine(SpawnInitialField(figureConfigs));
    }

    private List<FigureAttributes.FigureConfig> GenerateFigureSet(int totalCount)
    {
        List<FigureAttributes.FigureConfig> configs = new();
        const int figuresPerType = 3;
        int typeCount = totalCount / figuresPerType;

        List<FigureAttributes.FigureConfig> allPossibleConfigs = FigureAttributes.GetAllPossibleConfigs()
            .OrderBy(_ => Random.value)
            .Take(typeCount)
            .ToList();

        foreach (var config in allPossibleConfigs)
        {
            for (int i = 0; i < figuresPerType; i++)
            {
                configs.Add(config);
            }
        }

        while (configs.Count < totalCount)
        {
            configs.Add(allPossibleConfigs[Random.Range(0, typeCount)]);
        }

        return configs.OrderBy(_ => Random.value).ToList();
    }

    private IEnumerator SpawnInitialField(List<FigureAttributes.FigureConfig> figureConfigs)
    {
        if (fieldTransform.GetComponent<Collider2D>() is not { } fieldCollider)
        {
            Debug.LogError("FieldTransform does not have a Collider2D component!");
            yield break;
        }

        Bounds fieldBounds = fieldCollider.bounds;
        const float padding = 2f;
        float minX = fieldBounds.min.x + padding;
        float maxX = fieldBounds.max.x - padding;
        Vector3 spawnPos = fieldTransform.position + Vector3.up * 10f;

        foreach (var config in figureConfigs)
        {
            Vector3 spawnOffset = spawnPos + Vector3.right * Random.Range(minX, maxX);
            GameObject figureObject = Instantiate(figurePrefab, spawnOffset, Quaternion.identity);
            Figure figure = figureObject.GetComponent<Figure>();
            figure.Initialize(config, figureAssetDatabase, this);
            Rigidbody2D rb = figureObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
            figuresInField.Add(figureObject);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void HandleFigureClick(GameObject clickedFigureObject)
    {
        if (figuresInActionBar.Count >= maxActionBarSize)
        {
            OnLose?.Invoke();
            return;
        }

        figuresInActionBar.Add(clickedFigureObject);
        figuresInField.Remove(clickedFigureObject);
        clickedFigureObject.GetComponent<Figure>().DisablePhysics();
        StartCoroutine(MoveToActionBar(clickedFigureObject, actionBarSlots[figuresInActionBar.Count - 1]));

        foreach (var figure in figuresInField)
        {
            Rigidbody2D rb = figure.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.WakeUp();
            }
        }

        if (figuresInField.Count == 0)
        {
            OnWin?.Invoke();
        }
    }

    private IEnumerator MoveToActionBar(GameObject figure, Vector3 targetLocalPos)
    {
        figure.transform.SetParent(actionBar);
        Quaternion startRotation = figure.transform.rotation;
        Quaternion targetRotation = Quaternion.identity;
        
        Vector3 startPos = figure.transform.position;
        Vector3 targetWorldPos = actionBar.TransformPoint(targetLocalPos);
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            figure.transform.position = Vector3.Lerp(startPos, targetWorldPos, elapsed / duration);
            figure.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        figure.transform.localPosition = targetLocalPos;
        figure.transform.rotation = targetRotation;
        CheckForMatchInActionBar();
    }

    private void CheckForMatchInActionBar()
    {
        if (figuresInActionBar.Count < 3) return;

        bool foundMatch;
        do
        {
            foundMatch = false;
            var figureCount = new Dictionary<string, List<int>>();

            for (int i = 0; i < figuresInActionBar.Count; i++)
            {
                var figure = figuresInActionBar[i].GetComponent<Figure>();
                string figureKey = figure.GetKey();
                
                if (!figureCount.ContainsKey(figureKey))
                {
                    figureCount[figureKey] = new List<int>();
                }
                figureCount[figureKey].Add(i);
            }

            foreach (var kvp in figureCount)
            {
                if (kvp.Value.Count >= 3)
                {
                    List<int> indexesToRemove = kvp.Value.Take(3).OrderByDescending(idx => idx).ToList();
                    
                    foreach (int index in indexesToRemove)
                    {
                        Destroy(figuresInActionBar[index]);
                        figuresInActionBar.RemoveAt(index);
                    }

                    foundMatch = true;
                    break;
                }
            }

            for (int i = 0; i < figuresInActionBar.Count; i++)
            {
                figuresInActionBar[i].transform.localPosition = actionBarSlots[i];
            }

        } while (foundMatch && figuresInActionBar.Count >= 3);
    }


    public void RestartField()
    {
        StopAllCoroutines();
        int currentFigureCount = figuresInField.Count;
        figuresInField.ForEach(Destroy);
        figuresInField.Clear();
        List<FigureAttributes.FigureConfig> figureConfigs = GenerateFigureSet(currentFigureCount > 0 ? currentFigureCount : 27);
        StartCoroutine(SpawnInitialField(figureConfigs));
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}