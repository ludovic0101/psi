namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private Graph graph;
        private int nbCities;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            // TODO : implémenter
            this.graph = graph;
            this.nbCities = graph.Order;

        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // TODO : implémenter
            return new Tour();
        }

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            // TODO : implémenter

            float TotalReduction = 0;

            // Réduction des lignes 
            for(int i=0;i<m.NbRows;i++)
            {
                float minRow = float.PositiveInfinity;

                // On cherche le minimum de la ligne
                for (int j=0;j<m.NbColumns;j++)
                {
                    float val = m.GetValue(i, j);
                    if(val<minRow)
                    {
                        minRow = val;   

                    }
                }

                // Si on trouve un minimum non nulle et différent de l'infini, on effectue le code suivant
                if(minRow>0 && minRow!= float.PositiveInfinity)
                {
                    for(int j = 0;j<m.NbColumns; j++)
                    {
                        float val = m.GetValue(i, j);
                        if(val!=float.PositiveInfinity)
                        {
                            m.SetValue(i, j, val-minRow);
                        }
                    }
                    TotalReduction += minRow;
                }

                

            }
            // Réduction des colonnes
            for (int j = 0; j < m.NbColumns; j++)
            {
                float minCol = float.PositiveInfinity;

                //On cherche le minimum de la colonne
                for (int i = 0; i < m.NbRows; i++)
                {
                    float val = (m.GetValue(i, j));
                    if(val<minCol)
                    {
                        minCol= val;
                    }

                }
                // Si on trouve un minimum non nulle et différent de l'infini, on effectue le code suivant
                if (minCol>0 && minCol!= float.PositiveInfinity)
                {
                    for(int i = 0; i < m.NbRows; i++)
                    {
                        float val = m.GetValue(i, j);
                        if(val!=float.PositiveInfinity)
                        {
                            m.SetValue(i, j, val-minCol);
                        }
                        
                    }
                    TotalReduction += minCol;
                }
            }
            return  TotalReduction;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            int bestI = -1;
            int bestJ = -1;
            float maxRegret = -1.0f;

            for(int i=0;i<m.NbRows;i++)
            {
                for(int j=0;j<m.NbColumns;j++)
                {
                    if(m.GetValue(i, j)==0)
                    {
                        float minRow = GetMinInRowExcluding(m, i, j);

                        float minCol = GetMinInColumnExcluding(m, j, i);

                        float currentRegret = minCol + minRow;

                        if(currentRegret>maxRegret)
                        {
                            maxRegret = currentRegret;
                            bestI = i;
                            bestJ= j;

                        }
                    }
                }
            }
            return (bestI, bestJ, maxRegret);

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {

            // TODO : implémenter
            string currentCity = segment.destination;
            bool foundNext = true;

            while(foundNext)
            {
                foundNext = false;

                // on cherche si un segment déjà inclus part de la ville actuelle
                foreach(var s in includedSegments)
                {
                    if(s.source == currentCity)
                    {
                        currentCity = s.destination;
                        foundNext = true;
                        break;
                    }
                    
                }
            }
            if(currentCity==segment.source)
            {
                return (includedSegments.Count + 1 < nbCities);
            }
            return false;   
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

        private static float GetMinInRowExcluding(Matrix m,int row, int coltoexclude)
        {
            float min = float.PositiveInfinity;
            for(int j=0;j<m.NbColumns;j++)
            {
                if(j==coltoexclude)
                {
                    continue;
                }
                float val = m.GetValue(row,j);
                if(val<min)
                {
                    min= val;
                }
            }

            return min;
        }

        private static float GetMinInColumnExcluding(Matrix m, int col, int rowToExclude)
        {
            float min = float.PositiveInfinity;
            for(int i=0;i<m.NbRows;i++)
            {
                if(i==rowToExclude)
                {
                    continue;
                }
                float val= m.GetValue(i,col);
                if(val<min)
                {
                    min= val;
                }

            }
            return min;
        }
    }
}
