using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Spawner : MonoBehaviour {
	public Camera cam1; 
	public GameObject spawn_position, cam, cam2,sun, empty; // позиция спавна, камера, назначение объекта просмотра
	public Transform target;
	public GameObject OBJ;

	// управление камерой для мыши
	public Vector3 offset;
	public float sensitivity = 3; // чувствительность мышки
	public float limit = 80; // ограничение вращения по Y
	public float zoom = 0.25f; // чувствительность при увеличении, колесиком мышки
	public float zoomMax; // макс. увеличение
	public float zoomMin = 3; // мин. увеличение
	private float X, Y;
	public int zoomSpeed, y_offset;
	private float iskomoe_y, iskomoe_z; // для перехода в начальное положение

	//управление камерой для сенсора
	public float sensor_zoomSpeed;
	public float sensor_sensitivity;
	public static bool stop_rotating_i_touched;

	public Toggle expo_toggle, sun_toggle;
	public bool expo_mod_on;

	//public bool PC;

	public float nn;

    public MeshRenderer target_meshrenderer;
    Vector3 vctr;
    public float boundSize;
    public   Material m1;

	public Text t1;
    public Light l1;
    public Slider sl;

    private float firstClickTime, timeBetweenClicks;
    private bool coroutinAllowed;
    private int clickCounter;


void Start () {
     
        Screen.orientation = ScreenOrientation.AutoRotation;
        Application.targetFrameRate = 30;

		OBJ = OBJLoader.LoadOBJFile (Menu.mesh_path);
		MeshRenderer mr = OBJ.transform.GetChild (0).GetComponent<MeshRenderer> ();
		Mesh M = OBJ.transform.GetChild (0).GetComponent<MeshFilter> ().mesh;

        M.RecalculateBounds ();
		M.RecalculateNormals ();
		M.RecalculateTangents ();

     	target = OBJ.transform;

        boundSize = mr.bounds.size.y;

        if (boundSize > 1)
        {
            boundSize = 1;
        }

        zoomMax = Mathf.Sqrt(mr.bounds.size.x * mr.bounds.size.x + mr.bounds.size.y * mr.bounds.size.y) * 1.5f;
        if (zoomMax < 1 || zoomMax < 10)
        {
            zoomMax = 10;
        }
        l1.intensity = sl.value;


        limit = Mathf.Abs (limit);
				if (limit > 90)
					limit = 90;

				if (mr.bounds.size.y > 1) {
					offset = new Vector3 (offset.x, offset.y, -Mathf.Abs (mr.bounds.size.y));
				} else {
					offset = new Vector3 (offset.x, offset.y, -Mathf.Abs (mr.bounds.size.y) * 1.5f);
				}

				transform.position = target.position + offset;
				iskomoe_y = offset.y;
				iskomoe_z = offset.z;

        target_meshrenderer = target.GetComponentInChildren<MeshRenderer>();
       vctr = new Vector3(0, nn, 0);

        firstClickTime = 0f;
        timeBetweenClicks = 0.2f;
        clickCounter = 0;
        coroutinAllowed = true;
    }



    void Update() {

        
        vctr.y = nn;
        ///Debug
        t1.text = "" + 1 / Time.deltaTime;
        //t2.text = ""+Application.dataPath;
        l1.intensity = sl.value;


        if (Input.GetMouseButtonUp(0)) {
            clickCounter += 1;
        }
        if (clickCounter == 1 && coroutinAllowed) {
            firstClickTime = Time.time;
            StartCoroutine(DoubleClickDetection());
        }


        if (sun_toggle.isOn == true) {
            sun.transform.parent = cam.transform;
            sun.transform.rotation = cam.transform.rotation;
        }
        if (sun_toggle.isOn == false) {
            sun.transform.parent = null;
        }
        if (expo_toggle.isOn == true) {
            expo_mod_on = true;
        }
        if (expo_toggle.isOn == false) {
            expo_mod_on = false;
        }

        if (expo_mod_on == true) {
            cam.transform.Rotate(0, -Time.deltaTime * 10, 0);
        }

       // if (Input.touchCount > 1 || Input.GetKey(KeyCode.Mouse0)) {
        //    cam2.SetActive(true);
      //  }

        /// увеличение для компа
       
#if UNITY_EDITOR

        if (offset.z > -10) {

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                offset.z += zoom;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                offset.z -= zoom;
            offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

         } else {

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                offset.z += zoomSpeed;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                offset.z -= zoomSpeed;
            offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

        }

        if (Input.GetKey(KeyCode.Mouse0) && expo_mod_on == false && stop_rotating_i_touched == false) {
            X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            Y += Input.GetAxis("Mouse Y") * sensitivity;
            Y = Mathf.Clamp(Y, -limit, limit);
            transform.localEulerAngles = new Vector3(-Y, X, 0);
        }
#endif

/// управление для сенсора
#if !UNITY_EDITOR && UNITY_ANDROID
        if (Input.touchCount == 2) {
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevTouchDelMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float TouchDelMag = (touchZero.position - touchOne.position).magnitude;

				float deltaMagnitudeDiff = prevTouchDelMag - TouchDelMag;

				offset.z -= deltaMagnitudeDiff * sensor_zoomSpeed;
				offset.z = Mathf.Max (offset.z, -zoomMax);
                offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
        }
				
			if ((Input.touchCount == 1) && expo_mod_on == false && stop_rotating_i_touched == false) {
			
				X = transform.localEulerAngles.y + Input.GetTouch (0).deltaPosition.x * sensor_sensitivity;
				Y += Input.GetTouch (0).deltaPosition.y * sensor_sensitivity;
				Y = Mathf.Clamp (Y, -limit, limit);
				transform.localEulerAngles = new Vector3 (-Y, X, 0);

			}
#endif
		
		transform.position = transform.rotation * offset + target_meshrenderer.bounds.center + vctr;
	}
    private IEnumerator DoubleClickDetection()
    {
        coroutinAllowed = false;
        while(Time.time < firstClickTime + timeBetweenClicks)
        {
            if (clickCounter == 2) {
                cam2.SetActive(true);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        clickCounter = 0;
        firstClickTime = 0f;
        coroutinAllowed = true;
    }
		
	public void UP(){
		if (expo_mod_on == false) {
			nn += boundSize;
		}
	}
	public void RESET(){
		if (expo_mod_on == false) {
			nn = 0;
			offset = new Vector3 (offset.x, iskomoe_y, iskomoe_z);
			cam.transform.rotation = Quaternion.Euler (0, 0, 0);
			X = 0;
			Y = 0;
		}
	}
	public void DOWN(){
		if (expo_mod_on == false) {
			nn -= boundSize;
		}
	}
	public void MENU(){
		SceneManager.LoadScene (0);
	}
	public void HIDE(){
		cam2.SetActive (false);
	}

    public void ReImport() {
        GameObject model_for_screenshot = new GameObject(Menu.mesh_path);
        Mesh holderMesh = new Mesh();
        Importer newMesh = new Importer();

        holderMesh = newMesh.ImportFile(Menu.mesh_path);

        model_for_screenshot.gameObject.AddComponent<MeshRenderer>();
        model_for_screenshot.gameObject.AddComponent<MeshFilter>();
        model_for_screenshot.GetComponent<MeshFilter>().mesh = holderMesh;
        model_for_screenshot.GetComponent<Renderer>().material = m1;
    }

}
