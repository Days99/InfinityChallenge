using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Vector2 offset;
    public bool flying;
    public string pieceName;

    public Vector2 newPosition;
    public bool moving;
    private Vector2 pos;
    private Vector2 offsetY = new Vector2(0,0.6f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            //While the current position is different than new position move
            if (newPosition.x + offset.x != transform.localPosition.x || newPosition.y + offset.y != transform.localPosition.z || flying)
            {
                Vector2 localPos = new Vector2(transform.localPosition.x, transform.localPosition.z);
                if (!flying)
                    pos = Vector2.MoveTowards(localPos, newPosition + offset, 5f * Time.deltaTime);
                else
                    pos = Vector2.MoveTowards(localPos, newPosition + offset + offsetY, 10f * Time.deltaTime);

                if (localPos == newPosition + offset)
                    StopMoving();

                transform.localPosition = new Vector3(pos.x, 1, pos.y);

            }
            else
            {
                StopMoving();
            }
        }
    }

    private void StopMoving()
    {
        Debug.Log("Stop Moving");
        moving = false;
        newPosition = new Vector2(transform.localPosition.x, transform.localPosition.z);
        //Check if all the lines are in the correct place
        if (CheckSolution())
        {
            Debug.Log("NextLevel");
            FindObjectOfType<GameController>().ActivateWinningEffect();
        }
    }

    public bool CheckSolution()
    {
        foreach(PieceController p in FindObjectsOfType<PieceController>())
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(p.transform.localPosition.x, 0.6f, p.transform.localPosition.z), new Vector3(0, -10, 0), out hit))
            {
                Debug.Log(hit.transform.name);
                if (!hit.transform.name.StartsWith(p.pieceName))
                    return false;
            }
        
        }
        
        return true;
    }
    //Move Towards a new position
    public void MoveTowards(Vector2 position)
    {
        newPosition = position;
        moving = true;
    }
}
