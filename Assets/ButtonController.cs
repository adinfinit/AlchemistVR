using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {
    public float timeCycle;
    public bool triggered = false;
    public float reactivationTime = 3.0f; 

    private GameObject inner;
    private float timeTriggered;

    // Use this for initialization
    void Start () {
        inner = getChildGameObject(this.gameObject, "Inner");
        triggered = false;

}
	
	// Update is called once per frame
	void Update () {
        if (this.triggered)
        {
            inner.transform.localPosition = Vector3.Lerp(inner.transform.localPosition, new Vector3(0f, 0f, -0.08f), Time.deltaTime * 20.0f);
        }

        if (Time.time - timeTriggered > reactivationTime)
        {
            this.triggered = false;
            inner.transform.localPosition = Vector3.Lerp(inner.transform.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 20.0f);
        }
	}


    private void OnTriggerEnter(Collider other)
    {
        this.timeTriggered = Time.time;
        this.triggered = true;

        Debug.Log(other.gameObject.name);
    }
    

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}
