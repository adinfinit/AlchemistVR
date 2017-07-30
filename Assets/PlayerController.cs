using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SoundManager soundManager;
    private Lab lab;
    private SteamVR_TrackedController _controller;
    private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

    private void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        lab = GameObject.Find("Lab").GetComponent<Lab>();
    }


    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.005f, 0.005f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    private void Update()
    {
        DrawLine(this.transform.position, this.transform.position + this.transform.forward * 10f, Color.gray, Time.deltaTime);
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();

        _controller.TriggerClicked += HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerUnclicked;
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        _controller.TriggerUnclicked -= HandleTriggerUnclicked;
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
        GrabTiles();
        if (this.tag == "RightController") lab.HandleControllerPressRight();
        // if (this.tag == "LeftController") lab.HandleControllerPressLeft();


        // soundManager.ScorePoints(1000f);
    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        UngrabTiles();
        if (this.tag == "RightController") lab.HandleControllerUnpressRight();
    }
    
    #endregion
    
}