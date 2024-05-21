using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform currentCubeTransform;
    [SerializeField] public float scale;
    [SerializeField] public float speed;

    [SerializeField] public bool _transitioning;

    [SerializeField] bool _YcatchUp;
    [SerializeField] public Vector3 targetVector;



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(currentCubeTransform.position.y)));

        
            if (!_YcatchUp &&
                Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(currentCubeTransform.position.y)) >= scale / 2)
            {
                _YcatchUp = true;
            }

            if (_YcatchUp &&
                Mathf.Abs(Mathf.Abs(transform.position.y) - Mathf.Abs(currentCubeTransform.position.y)) <= 0.1f * scale)
            {
                _YcatchUp = false;
            }

            if (!_YcatchUp &&
                Vector3.Distance(transform.position, currentCubeTransform.position) > 0.1f * scale)
            {
                targetVector = Vector3.Lerp(targetVector, new Vector3(currentCubeTransform.position.x, targetVector.y, currentCubeTransform.position.z), Time.deltaTime * speed);
            }
            else if (Vector3.Distance(transform.position, currentCubeTransform.position) > 0.1f * scale)
            {
                targetVector = Vector3.Lerp(targetVector, currentCubeTransform.position, Time.deltaTime * speed);
            }


            transform.position = Vector3.Lerp(targetVector, currentCubeTransform.position, Time.deltaTime * speed);
        
    }
}
