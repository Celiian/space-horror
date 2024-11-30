using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class SoundPropagation : MonoBehaviour
{
    public Grid grid; // Ta grille de sol
    public Tilemap wallTilemap; // Tilemap qui contient les murs
    public Tilemap floorTilemap; // Tilemap qui contient le sol que tu veux peindre
    public TileBase wallTile; // Référence au type de Tile utilisé pour les murs
    public int maxRadius = 10; // Rayon maximal pour la propagation
    public float attenuationPerTile = 0.1f; // Atténuation du son par cellule
    public float wallAttenuation = 0.5f; // Atténuation supplémentaire pour traverser un mur
    public float updateInterval = 0.5f; // Intervalle de mise à jour en secondes
    public float minLevel = 0.01f; // Niveau sonore minimum pour la propagation
    private Coroutine propagationCoroutine;

    // List to keep track of previously painted tiles
    private List<Vector3Int> previouslyPaintedTiles = new List<Vector3Int>();

    void Start()
    {
        // Démarrer la coroutine pour mettre à jour la propagation du son toutes les 0.5 secondes
        propagationCoroutine = StartCoroutine(UpdateSoundPropagation());
    }

    IEnumerator UpdateSoundPropagation()
    {
        while (true)
        {
            // Calculer la position actuelle de la source
            Vector3Int currentSource = GetCurrentSourcePosition();

            // Clear previous sound propagation
            ClearPreviousSoundPropagation();

            // Propager le son depuis la position actuelle
            PaintTiles(currentSource);

            // Attendre l'intervalle avant la prochaine mise à jour
            yield return new WaitForSeconds(updateInterval);
        }
    }

    Vector3Int GetCurrentSourcePosition()
    {
        // Remplacer ceci par la logique pour obtenir la position actuelle de la source
        // Par exemple, si la source est un GameObject qui se déplace :
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        return new Vector3Int(cellPosition.x, cellPosition.y);
    }

   void PaintTiles(Vector3Int source)
    {
        // Utilisation d'une file pour la propagation BFS
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, float> soundLevels = new Dictionary<Vector3Int, float>();

        queue.Enqueue(source);
        soundLevels[source] = 1.0f; // Niveau sonore de départ à 100%

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            float currentLevel = soundLevels[current];

            if (currentLevel <= minLevel) continue; // Ne pas propager si le niveau sonore est trop bas

            // Peindre la tile avec une couleur basée sur le niveau sonore
            Color tileColor = Color.Lerp(Color.black, Color.red, currentLevel);
            SetTileColour(tileColor, current, floorTilemap);

            // Keep track of painted tiles
            previouslyPaintedTiles.Add(current);

            // Vérifier les voisins (haut, bas, gauche, droite)
            Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = current + direction;

                // Vérifie si la case est dans le rayon maximal
                if (Vector3Int.Distance(source, neighbor) > maxRadius) continue;

                // Calculate the distance from the source
                float distanceFromSource = Vector3Int.Distance(source, neighbor);

                // Calculate the attenuation using exponential decay
                float attenuation = Mathf.Exp(-distanceFromSource * attenuationPerTile);  // Exponential decay

                // If there's a wall, apply additional attenuation
                if (IsWall(neighbor))
                {
                    attenuation *= Mathf.Exp(-wallAttenuation);  // Apply additional exponential decay for walls
                }

                // New level of sound for the neighboring tile
                float newLevel = currentLevel * attenuation;
        
                // If the sound level is higher than what's recorded for the neighbor, update and add to the queue
                if (!soundLevels.ContainsKey(neighbor) || soundLevels[neighbor] < newLevel)
                {
                    soundLevels[neighbor] = newLevel;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }


    /// <summary>
    /// Set the colour of a tile.
    /// </summary>
    /// <param name="colour">The desired colour.</param>
    /// <param name="position">The position of the tile.</param>
    /// <param name="tilemap">The tilemap the tile belongs to.</param>
    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, indicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);

        // Set the colour.
        tilemap.SetColor(position, colour);
    }

    void ClearPreviousSoundPropagation()
    {
        // Reset the color of previously painted tiles to white or any default color
        foreach (Vector3Int tilePosition in previouslyPaintedTiles)
        {
            SetTileColour(Color.white, tilePosition, floorTilemap);
        }

        // Clear the list after resetting
        previouslyPaintedTiles.Clear();
    }

    // Vérifie si une tuile est un mur
    bool IsWall(Vector3Int position)
    {
        // Convertir la position en Vector3Int pour correspondre au système de Tilemap
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        TileBase tile = wallTilemap.GetTile(tilePosition);

        // Vérifier si la tuile est un mur
        return tile == wallTile;
    }
}
