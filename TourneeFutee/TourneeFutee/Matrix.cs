namespace TourneeFutee
{
    public class Matrix
    {
        private int nbRows;
        private int nbColumns;
        private float defaultValue;
        private float[][] tabS;

        // Testcommit
        //Test Commit chaima 
        // TODO : ajouter tous les attributs que vous jugerez pertinents 


        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {

            if (nbRows < 0) throw new ArgumentOutOfRangeException(nameof(nbRows));
            if (nbColumns < 0) throw new ArgumentOutOfRangeException(nameof(nbColumns));

            this.nbRows = nbRows;
            this.nbColumns = nbColumns;
            this.defaultValue = defaultValue;


            tabS = new float[nbRows][];
            for (int i = 0; i < nbRows; i++)
            {
                tabS[i] = new float[nbColumns];
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
            get { return defaultValue; }
            // pas de set
        }

        // Propriété : nombre de lignes
        // Lecture seule
        public int NbRows
        {
            get { return nbRows; } // TODO : implémenter 
                                   // pas de set
        }

        // Propriété : nombre de colonnes
        // Lecture seule
        public int NbColumns
        {
            get { return nbColumns; } // TODO : implémenter
                                      // pas de set
        }
        public float[][] TabS
        {
            get { return tabS; }
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if (i < 0 || i > nbRows)
                throw new ArgumentOutOfRangeException(nameof(i));

            //  nouvelle structure avec +1 ligne
            float[][] newtabS = new float[nbRows + 1][];

            // copier les lignes avant i
            for (int r = 0; r < i; r++)
            {
                newtabS[r] = new float[nbColumns];
                for (int c = 0; c < nbColumns; c++)
                    newtabS[r][c] = tabS[r][c];
            }

            // insérer la nouvelle ligne i remplie avec defaultValue
            newtabS[i] = new float[nbColumns];
            for (int c = 0; c < nbColumns; c++)
                newtabS[i][c] = defaultValue;

            //  copier les lignes à partir de i (décalées de +1)
            for (int r = i; r < nbRows; r++)
            {
                newtabS[r + 1] = new float[nbColumns];
                for (int c = 0; c < nbColumns; c++)
                    newtabS[r + 1][c] = tabS[r][c];
            }


            tabS = newtabS;
            nbRows++;
        }




        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            if (j < 0 || j > nbColumns)
                throw new ArgumentOutOfRangeException(nameof(j));

            float[][] newtabS = new float[nbRows][];

            for (int r = 0; r < nbRows; r++)
            {
                newtabS[r] = new float[nbColumns + 1];

                // Copier les colonnes avant j
                for (int c = 0; c < j; c++)
                    newtabS[r][c] = tabS[r][c];

                // Nouvelle colonne j remplie avec defaultValue
                newtabS[r][j] = defaultValue;

                // Copier les colonnes à partir de j (décalées de +1)
                for (int c = j; c < nbColumns; c++)
                    newtabS[r][c + 1] = tabS[r][c];
            }

            tabS = newtabS;
            nbColumns++;
        }



        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            if (i < 0 || i >= nbRows)
                throw new ArgumentOutOfRangeException(nameof(i));

            float[][] newtabS = new float[nbRows - 1][];

            // Lignes avant i
            for (int r = 0; r < i; r++)
            {
                newtabS[r] = new float[nbColumns];
                for (int c = 0; c < nbColumns; c++)
                    newtabS[r][c] = tabS[r][c];
            }

            // Lignes après i (décalées de -1)
            for (int r = i + 1; r < nbRows; r++)
            {
                newtabS[r - 1] = new float[nbColumns];
                for (int c = 0; c < nbColumns; c++)
                    newtabS[r - 1][c] = tabS[r][c];
            }

            tabS = newtabS;
            nbRows--;
        }


        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            if (j < 0 || j >= nbColumns)
                throw new ArgumentOutOfRangeException(nameof(j));

            float[][] newtabS = new float[nbRows][];

            for (int r = 0; r < nbRows; r++)
            {
                newtabS[r] = new float[nbColumns - 1];

                // Colonnes avant j
                for (int c = 0; c < j; c++)
                    newtabS[r][c] = tabS[r][c];

                // Colonnes après j (décalées de -1)
                for (int c = j + 1; c < nbColumns; c++)
                    newtabS[r][c - 1] = tabS[r][c];
            }

            tabS = newtabS;
            nbColumns--;
        }


        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            if (i < 0 || i >= nbRows)
                throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= nbColumns)
                throw new ArgumentOutOfRangeException(nameof(j));

            return tabS[i][j];
        }


        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            if (i < 0 || i >= nbRows)
                throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= nbColumns)
                throw new ArgumentOutOfRangeException(nameof(j));

            tabS[i][j] = v;
        }


        // Affiche la matrice
        public void Print()
        {
            for (int r = 0; r < nbRows; r++)
            {
                for (int c = 0; c < nbColumns; c++)
                {
                    Console.Write(tabS[r][c]);
                    if (c < nbColumns - 1) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }



        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}


