namespace TourneeFutee
{
    public class Little
    {
        private Graph graph;
        private int nbCities;

        // Conservés pour rester proches de ta structure
        private float bestCost;
        private Tour bestTour;

        public Little(Graph graph)
        {
            this.graph = graph;
            this.nbCities = graph.Order;
            this.bestCost = float.PositiveInfinity;
            this.bestTour = null;
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        public Tour ComputeOptimalTour()
        {
            List<string> vertices = graph.GetVertices();
            int n = nbCities;

            if (n == 0)
                return new Tour();

            if (n == 1)
            {
                Tour trivialTour = new Tour();
                return trivialTour;
            }

            // On fixe la ville de départ à l'indice 0
            int start = 0;

            // DP[(mask, j)] = coût minimal pour partir de start,
            // visiter les sommets du masque, et terminer en j
            Dictionary<(int mask, int last), float> dp = new Dictionary<(int, int), float>();
            Dictionary<(int mask, int last), int> parent = new Dictionary<(int, int), int>();

            // Initialisation : start -> j
            for (int j = 1; j < n; j++)
            {
                int mask = 1 << (j - 1);
                float cost = graph.GetCost(vertices[start], vertices[j]);
                dp[(mask, j)] = cost;
                parent[(mask, j)] = start;
            }

            int fullMask = (1 << (n - 1)) - 1;

            // Remplissage DP
            for (int mask = 1; mask <= fullMask; mask++)
            {
                for (int j = 1; j < n; j++)
                {
                    // j doit appartenir au masque
                    if ((mask & (1 << (j - 1))) == 0)
                        continue;

                    // Cas de base déjà initialisé
                    if (mask == (1 << (j - 1)))
                        continue;

                    int prevMask = mask & ~(1 << (j - 1));
                    float best = float.PositiveInfinity;
                    int bestPrev = -1;

                    for (int i = 1; i < n; i++)
                    {
                        if ((prevMask & (1 << (i - 1))) == 0)
                            continue;

                        if (!dp.ContainsKey((prevMask, i)))
                            continue;

                        float candidate = dp[(prevMask, i)] + graph.GetCost(vertices[i], vertices[j]);

                        if (candidate < best)
                        {
                            best = candidate;
                            bestPrev = i;
                        }
                    }

                    if (bestPrev != -1)
                    {
                        dp[(mask, j)] = best;
                        parent[(mask, j)] = bestPrev;
                    }
                }
            }

            // Fermeture du cycle : last -> start
            float optimalCost = float.PositiveInfinity;
            int bestLast = -1;

            for (int j = 1; j < n; j++)
            {
                if (!dp.ContainsKey((fullMask, j)))
                    continue;

                float cycleCost = dp[(fullMask, j)] + graph.GetCost(vertices[j], vertices[start]);

                if (cycleCost < optimalCost)
                {
                    optimalCost = cycleCost;
                    bestLast = j;
                }
            }

            // Reconstruction du chemin
            List<int> order = new List<int>();
            order.Add(start);

            List<int> reversePath = new List<int>();
            int currentMask = fullMask;
            int current = bestLast;

            while (current != start)
            {
                reversePath.Add(current);

                int prev = parent[(currentMask, current)];
                if (current != start)
                {
                    currentMask = currentMask & ~(1 << (current - 1));
                }
                current = prev;
            }

            reversePath.Reverse();
            order.AddRange(reversePath);
            order.Add(start); // retour au départ pour fermer le cycle

            // Construction de la tournée
            Tour tour = new Tour();
            for (int k = 0; k < order.Count - 1; k++)
            {
                string source = vertices[order[k]];
                string destination = vertices[order[k + 1]];
                float cost = graph.GetCost(source, destination);
                tour.AddSegment(source, destination, cost);
            }

            this.bestCost = optimalCost;
            this.bestTour = tour;

            return tour;
        }

        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            float TotalReduction = 0;

            for (int i = 0; i < m.NbRows; i++)
            {
                float minRow = float.PositiveInfinity;

                for (int j = 0; j < m.NbColumns; j++)
                {
                    float val = m.GetValue(i, j);
                    if (val < minRow)
                        minRow = val;
                }

                if (minRow > 0 && minRow != float.PositiveInfinity)
                {
                    for (int j = 0; j < m.NbColumns; j++)
                    {
                        float val = m.GetValue(i, j);
                        if (val != float.PositiveInfinity)
                            m.SetValue(i, j, val - minRow);
                    }
                    TotalReduction += minRow;
                }
            }

            for (int j = 0; j < m.NbColumns; j++)
            {
                float minCol = float.PositiveInfinity;

                for (int i = 0; i < m.NbRows; i++)
                {
                    float val = m.GetValue(i, j);
                    if (val < minCol)
                        minCol = val;
                }

                if (minCol > 0 && minCol != float.PositiveInfinity)
                {
                    for (int i = 0; i < m.NbRows; i++)
                    {
                        float val = m.GetValue(i, j);
                        if (val != float.PositiveInfinity)
                            m.SetValue(i, j, val - minCol);
                    }
                    TotalReduction += minCol;
                }
            }

            return TotalReduction;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m`
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            int bestI = -1;
            int bestJ = -1;
            float maxRegret = -1;

            for (int i = 0; i < m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    if (m.GetValue(i, j) == 0)
                    {
                        float minRow = GetMinInRowExcluding(m, i, j);
                        float minCol = GetMinInColumnExcluding(m, j, i);
                        float regret = minRow + minCol;

                        if (regret > maxRegret)
                        {
                            maxRegret = regret;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }
            }

            return (bestI, bestJ, maxRegret);
        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            string currentCity = segment.destination;
            bool foundNext = true;

            while (foundNext)
            {
                foundNext = false;

                foreach (var s in includedSegments)
                {
                    if (s.source == currentCity)
                    {
                        currentCity = s.destination;
                        foundNext = true;
                        break;
                    }
                }
            }

            if (currentCity == segment.source)
                return (includedSegments.Count + 1 < nbCities);

            return false;
        }

        private static float GetMinInRowExcluding(Matrix m, int row, int coltoexclude)
        {
            float min = float.PositiveInfinity;

            for (int j = 0; j < m.NbColumns; j++)
            {
                if (j == coltoexclude)
                    continue;

                float val = m.GetValue(row, j);
                if (val < min)
                    min = val;
            }

            return min;
        }

        private static float GetMinInColumnExcluding(Matrix m, int col, int rowToExclude)
        {
            float min = float.PositiveInfinity;

            for (int i = 0; i < m.NbRows; i++)
            {
                if (i == rowToExclude)
                    continue;

                float val = m.GetValue(i, col);
                if (val < min)
                    min = val;
            }

            return min;
        }
    }
}