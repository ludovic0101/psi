namespace TourneeFutee
{
    public class Little
    {
        private Graph graph;
        private int nbCities;

        // Stockage du meilleur résultat
        private float bestCost;
        private Tour bestTour;

        public Little(Graph graph)
        {
            this.graph = graph;
            this.nbCities = graph.Order;

            this.bestCost = float.PositiveInfinity;
            this.bestTour = null;
        }

        public Tour ComputeOptimalTour()
        {
            // Construire matrice initiale depuis le graphe
            Matrix m = graph.ToCostMatrix();

            List<(string source, string destination)> includedSegments = new List<(string, string)>();

            BranchAndBound(m, includedSegments, 0);

            return bestTour;
        }

        // ===================== ALGO PRINCIPAL =====================

        private void BranchAndBound(Matrix m, List<(string source, string destination)> includedSegments, float currentCost)
        {
            // Copie de matrice pour éviter effets de bord
            Matrix matrixCopy = m.Clone();

            // Réduction
            float reduction = ReduceMatrix(matrixCopy);
            float newCost = currentCost + reduction;

            // Coupure
            if (newCost >= bestCost)
                return;

            // Si solution complète
            if (includedSegments.Count == nbCities)
            {
                Tour t = BuildTour(includedSegments, newCost);
                if (t.Cost < bestCost)
                {
                    bestCost = t.Cost;
                    bestTour = t;
                }
                return;
            }

            // Choisir arc
            var (i, j, _) = GetMaxRegret(matrixCopy);

            string source = graph.GetVertexName(i);
            string destination = graph.GetVertexName(j);

            // ================= INCLUSION =================
            {
                Matrix includeMatrix = matrixCopy.Clone();
                var includeSegments = new List<(string, string)>(includedSegments);
                includeSegments.Add((source, destination));

                // Supprimer ligne i
                for (int col = 0; col < includeMatrix.NbColumns; col++)
                    includeMatrix.SetValue(i, col, float.PositiveInfinity);

                // Supprimer colonne j
                for (int row = 0; row < includeMatrix.NbRows; row++)
                    includeMatrix.SetValue(row, j, float.PositiveInfinity);

                // Interdire trajet inverse
                int jIndex = j;
                int iIndex = i;
                includeMatrix.SetValue(jIndex, iIndex, float.PositiveInfinity);

                // Interdire sous-tournées
                foreach (var v1 in graph.Vertices)
                {
                    foreach (var v2 in graph.Vertices)
                    {
                        if (v1 == v2) continue;

                        if (IsForbiddenSegment((v1, v2), includeSegments, nbCities))
                        {
                            int r = graph.GetVertexIndex(v1);
                            int c = graph.GetVertexIndex(v2);
                            includeMatrix.SetValue(r, c, float.PositiveInfinity);
                        }
                    }
                }

                BranchAndBound(includeMatrix, includeSegments, newCost);
            }

            // ================= EXCLUSION =================
            {
                Matrix excludeMatrix = matrixCopy.Clone();
                excludeMatrix.SetValue(i, j, float.PositiveInfinity);

                BranchAndBound(excludeMatrix, includedSegments, newCost);
            }
        }

        // ===================== CONSTRUCTION TOUR =====================

        private Tour BuildTour(List<(string source, string destination)> segments, float cost)
        {
            Tour t = new Tour();

            foreach (var s in segments)
            {
                t.AddSegment(s.source, s.destination, graph.GetCost(s.source, s.destination));
            }

            t.SetCost(cost);

            return t;
        }

        // ===================== TES MÉTHODES =====================

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
                if (j == coltoexclude) continue;
                float val = m.GetValue(row, j);
                if (val < min) min = val;
            }

            return min;
        }

        private static float GetMinInColumnExcluding(Matrix m, int col, int rowToExclude)
        {
            float min = float.PositiveInfinity;

            for (int i = 0; i < m.NbRows; i++)
            {
                if (i == rowToExclude) continue;
                float val = m.GetValue(i, col);
                if (val < min) min = val;
            }

            return min;
        }
    }
}