using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinningEffectController : MonoBehaviour
{
    public List<Vector2> pathPositions;
    private int currentIndex = 0;

    public Vector2 newPosition;
    public bool moving;
    private Vector2 pos;

    private GeneticAlgorith geneticAlgorith;
    private GameController gameController;

    // Start is called before the first frame update
    public void Init()
    {
        geneticAlgorith = FindObjectOfType<GeneticAlgorith>();
        gameController = FindObjectOfType<GameController>();
        Vector2Int center = new Vector2Int(geneticAlgorith.currentIndividual.width / 2, geneticAlgorith.currentIndividual.heigth / 2);
        pathPositions = new List<Vector2>();
        foreach (Vector2Int path in geneticAlgorith.levelSO.path)
        {
            pathPositions.Add(new Vector2((float)(path.x - center.x + 0.5f), (float)(path.y - center.y + 0.5f)));
        }

    }
    public void StartMoving()
    {
        gameObject.SetActive(true);
        transform.localPosition = pathPositions[currentIndex];
        moving = true;
        MoveTowards(pathPositions[currentIndex++]);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            //While the current position is different than new position move
            if (newPosition.x != transform.localPosition.x || newPosition.y  != transform.localPosition.z)
            {
                Vector2 localPos = new Vector2(transform.localPosition.x, transform.localPosition.z);
                pos = Vector2.MoveTowards(localPos, newPosition, 5f * Time.deltaTime);

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
        moving = false;
        if (!MoveToNextPosition())
            newPosition = new Vector2(transform.localPosition.x, transform.localPosition.z);
        //else
        //gameController.LoadNextLevel();
        //Check if all the lines are in the correct place
        if (currentIndex >= pathPositions.Count - 1)
            gameController.LoadNextLevel();
    }

    public bool MoveToNextPosition()
    {
        if (currentIndex < pathPositions.Count - 1)
        {
            currentIndex++;
            MoveTowards(new Vector2(pathPositions[currentIndex].x, pathPositions[currentIndex].y));
            //transform.LookAt(pathPositions[currentIndex]);
            return true;
        }
        else
            return false;
    }

    public void MoveTowards(Vector2 position)
    {
        newPosition = position;
        moving = true;
    }
}
