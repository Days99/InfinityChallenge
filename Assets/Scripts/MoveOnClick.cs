using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnClick : MonoBehaviour
{
    private GameObject heldPiece;
    private Vector3 previousPos;
    void Start() { }

    // Update is called once per frame    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                //Select stage    
                if (hit.transform.name.StartsWith("Puzzle"))
                {
                    //Debug.Log("Move " + hit.transform.name);
                    heldPiece = hit.transform.gameObject;
                    RaycastHit groundHit;
                    if (Physics.Raycast(hit.transform.localPosition, new Vector3(0, -20, 0), out groundHit))
                    {
                        if (groundHit.transform.name.StartsWith("Path") || groundHit.transform.name.StartsWith("Ground"))
                        {
                            previousPos = groundHit.transform.localPosition;
                        }
                    }
                }
        }

        if (Input.GetMouseButtonUp(0) && heldPiece != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 200.0f);
            int count = 0;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name.StartsWith("Path") || hit.transform.name.StartsWith("Ground"))
                {
                    heldPiece.GetComponent<PieceController>().flying = false;
                    RaycastHit groundHit;
                    if (Physics.Raycast(new Vector3(hit.transform.position.x, 100, hit.transform.position.z), new Vector3(0, -200, 0), out groundHit))
                    {
                        if (groundHit.transform.name.StartsWith("0"))
                            break;
                        if (groundHit.transform.name.StartsWith("Puzzle") && count < 1)
                        {
                            groundHit.transform.GetComponent<PieceController>().MoveTowards(new Vector2(previousPos.x, previousPos.z));
                            count++;
                        }
                        Debug.Log(groundHit.transform.name);
                    }
                    heldPiece.GetComponent<PieceController>().MoveTowards(new Vector2(hit.transform.localPosition.x, hit.transform.localPosition.z));
                    break;
                }

            }
            heldPiece = null;
        }

        //if (Input.GetMouseButton(0) && heldPiece != null)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        heldPiece.GetComponent<PieceController>().flying = true;
        //        heldPiece.GetComponent<PieceController>().MoveTowards(new Vector2(hit.transform.localPosition.x, hit.transform.localPosition.z));

        //    }
        //}
    }
}
