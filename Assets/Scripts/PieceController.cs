using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Vector2 offset;
    public bool flying;
    private Vector2 newPosition;
    private bool moving;
    private Vector2 pos;
    private float offsetY = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            if (newPosition.x != transform.localPosition.x || newPosition.y != transform.localPosition.z || flying)
            {
                //Debug.Log("Moving To " + newPosition);
                pos = Vector2.MoveTowards(new Vector2(transform.localPosition.x, transform.localPosition.z), newPosition, 5f * Time.deltaTime);
                if(flying)
                    transform.localPosition = new Vector3(pos.x, offsetY, pos.y);
                else
                    transform.localPosition = new Vector3(pos.x, 1, pos.y);

            }
            else
            {
                moving = false;
                newPosition = new Vector2(transform.localPosition.x, transform.localPosition.z);
            }
        }
    }

    public void MoveTowards(Vector2 position)
    {
        newPosition = position;
        moving = true;
    }
}
