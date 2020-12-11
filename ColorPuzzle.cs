using System.Collections.Generic;
using UnityEngine;

public class ColorPuzzle : MonoBehaviour
{
    public Camera cam;
    public Sprite sprite;
    public Transform board;
    public List<SpriteRenderer> tileSR = new List<SpriteRenderer>();
    public bool solved = false;

    void Start()
    {
        //Setup the camera
        cam = gameObject.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 2;
        cam.farClipPlane = 1500;
        cam.backgroundColor = new Color(Rand(), Rand(), Rand());

        //Import the asset
        sprite = Resources.Load<Sprite>("ART/tile");

        //Give our puzzle a parent to keep the hierarchy in the inspector clean
        board = new GameObject("Board").transform;
        board.parent = transform;

        //Create the 3x3 grid of colors
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                GameObject go = new GameObject("Tile: " + x + ", " + y);
                go.transform.position = new Vector3(x * 1.1f, y * 1.1f, 10);
                go.transform.parent = board;

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.color = RandomColor();
                tileSR.Add(sr);

                go.AddComponent<BoxCollider2D>();
            }
        }
    }

    //Color the tiles. Attempts to not make the color not black
    static Color RandomColor()
    {
        int r = 0; int g = 0; int b = 0;

        for (int i = 0; i < 4; i++)
        {
            r = Random.Range(0, 2);
            g = Random.Range(0, 2);
            b = Random.Range(0, 2);

            if (r == 1 || g == 1 || b == 1)
            { break; }
        }

        return new Color(r, g, b, 1);
    }

    //to give the camera a random dark bg color
    static float Rand()
    { return Random.Range(.1f, .15f); }

    //Finds adjacent tiles to affect
    public void FindAdjacentTiles(SpriteRenderer sr)
    {
        for (int i = 0; i < tileSR.Count; i++)
        {
            if (tileSR[i] == sr)
            {
                //affect the tile to the south
                if (i - 1 != -1 && i - 1 != 2 && i - 1 != 5)
                { AffectTile(sr.color, i - 1); }

                //affect the tile to the north
                if (i + 1 != 9 && i + 1 != 3 && i + 1 != 6)
                { AffectTile(sr.color, i + 1); }

                //affect the tile to the east
                if (i - 3 >= 0)
                { AffectTile(sr.color, i - 3); }

                //affect the tile to the west
                if (i + 3 <= 8)
                { AffectTile(sr.color, i + 3); }

                tileSR[i].color = Color.black;
                break;
            }
        }

        CheckAnswer();
    }

    //Changes the color of the effected tile
    //Essentially flicking RGB switches on or off
    void AffectTile(Color source, int reciever)
    {
        float rSource = source.r;
        float gSource = source.g;
        float bSource = source.b;

        float r = tileSR[reciever].color.r;
        float g = tileSR[reciever].color.g;
        float b = tileSR[reciever].color.b;

        if (rSource > .9f)
        {
            if (tileSR[reciever].color.r > .9f) { r = 0; }
            else { r = 1; }
        }
        if (gSource > .9f)
        {
            if (tileSR[reciever].color.g > .9f) { g = 0; }
            else { g = 1; }
        }
        if (bSource > .9f)
        {
            if (tileSR[reciever].color.b > .9f) { b = 0; }
            else { b = 1; }
        }

        tileSR[reciever].color = new Color(r, g, b, 1);
    }

    //If all tiles are black, the puzzle is solved
    void CheckAnswer()
    {
        for (int i = 0; i < tileSR.Count; i++)
        {
            Color c = tileSR[i].color;
            float r = c.r; float g = c.g; float b = c.b;
            if (r > .9f || g > .9f || b > .9f)
            { return; }
        }

        Debug.Log("SOLVED!");
        solved = true;
    }

    //The user input
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                FindAdjacentTiles(hit.collider.gameObject.GetComponent<SpriteRenderer>());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && solved)
        {
            for (int i = 0; i < tileSR.Count; i++)
            { tileSR[i].color = RandomColor(); }
            solved = false;
        }
    }
}
