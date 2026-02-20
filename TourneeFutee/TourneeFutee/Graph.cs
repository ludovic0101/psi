using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace TourneeFutee
{
    public class Graph
    {
        // ------------------------------ Attributs ------------------------------

        private readonly bool _directed;
        private readonly float _noEdgeValue;

        // Noms + valeurs des sommets
        private readonly List<string> _vertexNames = new();
        private readonly List<float> _vertexValues = new();

        // Pour retrouver rapidement l'indice d'un sommet à partir de son nom
        private readonly Dictionary<string, int> _nameToIndex = new();

        // Matrice d'adjacence (poids)
        // Convention : adj[i][j] = poids si arc i->j existe, sinon _noEdgeValue
        private Matrix _adj;

        

        // Construit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            _directed = directed;
            _noEdgeValue = noEdgeValue;

            // Au départ : 0 sommet => matrice 0x0
            _adj = new Matrix(nbRows: 0, nbColumns: 0, defaultValue: _noEdgeValue);
        }


        // Propriété : ordre du graphe
        public int Order
        {
            get { return _vertexNames.Count; }
        }

        // Propriété : graphe orienté ou non
        public bool Directed
        {
            get { return _directed; }
        }

        // (Optionnel mais pratique)
        public float NoEdgeValue
        {
            get { return _noEdgeValue; }
        }

       

        private int IndexOfVertex(string name)
        {
            if (!_nameToIndex.TryGetValue(name, out int idx))
                throw new ArgumentException($"Sommet introuvable : '{name}'");

            return idx;
        }

        private bool EdgeExistsByIndex(int i, int j)
        {
            return _adj.GetValue(i, j) != _noEdgeValue;
        }

        private void EnsureDifferentVertices(string a, string b)
        {
            // Autoriser ou non les boucles ? Ici on autorise, donc pas de check.
            // Si tu veux interdire : if (a == b) throw new ArgumentException("Boucle interdite");
        }

        // Reconstruit la matrice d'adjacence en ajoutant 1 ligne/colonne à la fin
        private void GrowAdjMatrix()
        {
            int n = _adj.NbRows; // ancien ordre

            // 1) ajouter une ligne à la fin
            _adj.AddRow(n); // ligne remplie avec DefaultValue (= _noEdgeValue) si Matrix est cohérente avec vos tests

            // 2) ajouter une colonne à la fin
            _adj.AddColumn(n);
        }

        // Supprime la ligne/colonne d'indice k (sommet supprimé)
        private void ShrinkAdjMatrix(int k)
        {
            _adj.RemoveRow(k);
            _adj.RemoveColumn(k);
        }

        

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom du sommet ne peut pas être vide.");

            if (_nameToIndex.ContainsKey(name))
                throw new ArgumentException($"Un sommet nommé '{name}' existe déjà.");

            // Ajouter structures
            _nameToIndex[name] = _vertexNames.Count;
            _vertexNames.Add(name);
            _vertexValues.Add(value);

            // Agrandir matrice d'adjacence
            GrowAdjMatrix();
        }

        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            int idx = IndexOfVertex(name);

            // 1) retirer ligne/colonne de la matrice
            ShrinkAdjMatrix(idx);

            // 2) retirer dans listes
            _vertexNames.RemoveAt(idx);
            _vertexValues.RemoveAt(idx);

            // 3) reconstruire dictionnaire indices (car tous les indices > idx ont décalé)
            _nameToIndex.Clear();
            for (int i = 0; i < _vertexNames.Count; i++)
                _nameToIndex[_vertexNames[i]] = i;
        }

        // Renvoie la valeur du sommet de nom `name`
        public float GetVertexValue(string name)
        {
            int idx = IndexOfVertex(name);
            return _vertexValues[idx];
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        public void SetVertexValue(string name, float value)
        {
            int idx = IndexOfVertex(name);
            _vertexValues[idx] = value;
        }

        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        public List<string> GetNeighbors(string vertexName)
        {
            int i = IndexOfVertex(vertexName);
            List<string> neighborNames = new List<string>();

            for (int j = 0; j < Order; j++)
            {
                if (j == i) continue; // optionnel : enlever si tu veux compter la boucle comme voisin
                if (EdgeExistsByIndex(i, j))
                {
                    neighborNames.Add(_vertexNames[j]);
                }
            }

            return neighborNames;
        }

        

        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            EnsureDifferentVertices(sourceName, destinationName);

            int i = IndexOfVertex(sourceName);
            int j = IndexOfVertex(destinationName);

            if (EdgeExistsByIndex(i, j))
                throw new ArgumentException($"Un arc existe déjà entre '{sourceName}' -> '{destinationName}'.");

            if (!_directed && EdgeExistsByIndex(j, i))
                throw new ArgumentException($"Un arc existe déjà entre '{destinationName}' -> '{sourceName}' (graphe non orienté).");

            _adj.SetValue(i, j, weight);

            if (!_directed)
            {
                _adj.SetValue(j, i, weight);
            }
        }

        public void RemoveEdge(string sourceName, string destinationName)
        {
            // Récupération des indices correspondant aux noms des sommets
            int i = IndexOfVertex(sourceName); 
            int j = IndexOfVertex(destinationName);

            // Vérification de l'existence de la relation avant suppression
            if (!EdgeExistsByIndex(i, j))
                throw new ArgumentException($"Arc introuvable : '{sourceName}' -> '{destinationName}'.");

            // Suppression de l'arc dans la matrice d'adjacence
            _adj.SetValue(i, j, _noEdgeValue);

          
            if (!_directed)
            {
                // En non orienté, on retire aussi l'inverse
                _adj.SetValue(j, i, _noEdgeValue);
            }
        }

        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            int i = IndexOfVertex(sourceName);
            int j = IndexOfVertex(destinationName);

            if (!EdgeExistsByIndex(i, j))
                throw new ArgumentException($"Arc introuvable : '{sourceName}' -> '{destinationName}'.");

            return _adj.GetValue(i, j); 
        }

        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            int i = IndexOfVertex(sourceName);
            int j = IndexOfVertex(destinationName);

            // On exige seulement que les sommets existent (comme demandé)
            _adj.SetValue(i, j, weight);

            if (!_directed)
            {
                _adj.SetValue(j, i, weight); 
            }
        }

        // ------------------------------ Méthodes utiles (optionnelles) ------------------------------

        public bool ContainsVertex(string name) => _nameToIndex.ContainsKey(name);

        public bool ContainsEdge(string sourceName, string destinationName)
        {
            int i = IndexOfVertex(sourceName);
            int j = IndexOfVertex(destinationName);
            return EdgeExistsByIndex(i, j);
        }

        public List<string> GetVertices()
        {
            return new List<string>(_vertexNames);
        }

        public void PrintAdjacencyMatrix()
        {
            Console.WriteLine($"Adjacency matrix (Order={Order}, Directed={Directed}, NoEdgeValue={_noEdgeValue})");
            _adj.Print();
        }
    }
}
