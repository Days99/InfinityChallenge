using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnClick : MonoBehaviour
{
    private GameObject heldPiece;
    private Vector3 previousPos;
    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Check if it hit something
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Check if it hit a puzzle line
                if (hit.transform.name.StartsWith("Puzzle"))
                {
                    heldPiece = hit.transform.gameObject;
                    gameController.PlaySoundEffect(2);
                    RaycastHit groundHit;
                    if (Physics.Raycast(hit.transform.localPosition, new Vector3(0, -20, 0), out groundHit))
                    {
                        if (groundHit.transform.name.Contains("Path") || groundHit.transform.name.Contains("Ground"))
                        {
                            previousPos = groundHit.transform.localPosition;
                        }
                    }
                }
                //Check if it hit a ground node with a puzzle line on top 
                else if (hit.transform.name.Contains("Path") || hit.transform.name.Contains("Ground"))
                {
                    RaycastHit groundHit;
                    int layerMask = 1 << 10;
                    if (Physics.Raycast(new Vector3(hit.transform.position.x, 100, hit.transform.position.z), new Vector3(0, -200, 0), out groundHit, 500, layerMask))
                    {

                        if (groundHit.transform.name.StartsWith("Puzzle"))
                        {
                            gameController.PlaySoundEffect(2);
                            heldPiece = groundHit.transform.gameObject;
                            previousPos = hit.transform.localPosition;
                            Debug.Log(groundHit.transform.name);

                        }
                    }
                }
            }

        }

        if (Input.GetMouseButtonUp(0) && heldPiece != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int groundLayerMask = 1 << 9;
            RaycastHit[] hits = Physics.RaycastAll(ray, 200.0f, groundLayerMask);
            PieceController piece = heldPiece.GetComponent<PieceController>();
            piece.flying = false;

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    //Check if it hit a valid ground node
                    if (hit.transform.name.Contains("Path") || hit.transform.name.Contains("Ground"))
                    {
                        RaycastHit groundHit;
                        int layerMask = 1 << 10;

                        if (Physics.Raycast(new Vector3(hit.transform.position.x, 100, hit.transform.position.z), new Vector3(0, -200, 0), out groundHit, 500, layerMask))
                        {
                            //Check if it has a Start or End node on top
                            if (groundHit.transform.name.StartsWith("0"))
                            {
                                piece.MoveTowards(new Vector2(previousPos.x, previousPos.z));
                                break;
                            }

                            if (groundHit.transform.name.StartsWith("Puzzle"))
                            {
                                //If it hits another puzzle line move to the previous position
                                if (groundHit.transform.name != heldPiece.transform.name) 
                                groundHit.transform.GetComponent<PieceController>().MoveTowards(new Vector2(previousPos.x, previousPos.z));
                            }
                        }
                        //Move held puzzle line to new position
                        piece.MoveTowards(new Vector2(hit.transform.localPosition.x, hit.transform.localPosition.z));
                        break;
                    }
                    else
                    {
                        //Did not hit ground node so return held line to previous position
                        piece.MoveTowards(new Vector2(previousPos.x, previousPos.z));
                    }
                }
            }
            else
            {
                //Did not hit anything so return held line to previous position
                piece.MoveTowards(new Vector2(previousPos.x, previousPos.z));
            }

            heldPiece = null;
        }

        if (Input.GetMouseButton(0) && heldPiece != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 8;
            if (Physics.Raycast(ray, out hit, 200, layerMask)) 
            {
                //Set held piece to flying and move to new position
                heldPiece.GetComponent<PieceController>().flying = true;
                heldPiece.GetComponent<PieceController>().MoveTowards(new Vector2(hit.point.x, hit.point.z));

            }
        }
    }
}
