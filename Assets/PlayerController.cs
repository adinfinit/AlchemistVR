using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SoundManager soundManager;
    private SteamVR_TrackedController _controller;
    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

    private void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();

        _controller.TriggerClicked += HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerUnclicked;
        _controller.PadClicked += HandlePadClicked;
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        _controller.TriggerUnclicked -= HandleTriggerUnclicked;
        _controller.PadClicked -= HandlePadClicked;
    }


    private void UngrabTiles()
    {
        soundManager.stopDragging();
    }

    private void GrabTiles()
    {
        // todo: validselection
        bool validSelection = true;
        if (validSelection)
        {
            soundManager.startDragging();
        }
    }

    #region Primitive Spawning
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        SpawnCurrentPrimitiveAtController();
        GrabTiles();

        soundManager.ScorePoints(1000f);
    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        UngrabTiles();
    }


    private void SpawnCurrentPrimitiveAtController()
    {
        var spawnedPrimitive = GameObject.CreatePrimitive(_currentPrimitiveType);
        spawnedPrimitive.transform.position = transform.position;
        spawnedPrimitive.transform.rotation = transform.rotation;

        spawnedPrimitive.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        spawnedPrimitive.AddComponent<Rigidbody>();
        if (_currentPrimitiveType == PrimitiveType.Plane)
            spawnedPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }
    #endregion

    #region Primitive Selection
    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padY < 0)
            SelectPreviousPrimitive();
        else
            SelectNextPrimitive();
    }

    private void SelectNextPrimitive()
    {
        _currentPrimitiveType++;
        if (_currentPrimitiveType > PrimitiveType.Quad)
            _currentPrimitiveType = PrimitiveType.Sphere;
    }

    private void SelectPreviousPrimitive()
    {
        _currentPrimitiveType--;
        if (_currentPrimitiveType < PrimitiveType.Sphere)
            _currentPrimitiveType = PrimitiveType.Quad;
    }
    #endregion
}