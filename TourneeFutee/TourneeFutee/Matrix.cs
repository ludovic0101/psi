namespace TourneeFutee
{
    public class Matrix
    {
        private int nbRows;
        private int nbColumns;
        private float defaultValue;
        private float[][] tabS;

        // Testcommit
        // TODO : ajouter tous les attributs que vous jugerez pertinents 


        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            if (nbRows < 0 || nbColumns < 0)
            {
                throw new ArgumentException();
            }
            else
            {
                this.nbRows = nbRows;
                this.nbColumns = nbColumns;
                this.defaultValue = defaultValue;
            }
            this.tabS = new tab[nbRows][];
            for (int i = 0; i < nbRows; i++) 
            {
                for (int j = 0; j < nbColumns; j++) 
                {
                    tabS[i][j] = defaultValue;
                }
            }
        }

        // Propriété : valeur par défaut utilisée pour remplir les nouvelles cases
        // Lecture seule
        public float DefaultValue
        {
            get; // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get; // TODO : implémenter
                 // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get; // TODO : implémenter
                 // pas de set
        }
        public float[][] tabS { get; set; }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if (i < 0 || i > nbRows)
            {
                throw new ArgumentException();
            }
            else 
            {
                listeS.Add(new List<float>(nbColums));
                for (int j = i; j < nbRows + 1; j++)
                {
                    List<float> liste = listeS[j];
                    listeS[j] = listeS[i]


                }
                    for (int j = i; j < nbRows + 1; j++) 
                {
                    for (int k = 0; k < nbColumns; k++) 
                    {
                        if (j == i) 
                        {
                            listeS.Add(new List<float>());
                        }
                    }
                }
            }
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            // TODO : implémenter
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            // TODO : implémenter
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            // TODO : implémenter
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            // TODO : implémenter
            return 0.0f;
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            // TODO : implémenter
        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}
