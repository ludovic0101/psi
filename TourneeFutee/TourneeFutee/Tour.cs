namespace TourneeFutee
{
    public class Tour
    {
        // ===================== ATTRIBUTS =====================

        private List<(string source, string destination)> segments;
        private float cost;

        // ===================== CONSTRUCTEURS =====================

        // Constructeur par défaut (conservé pour la classe Little)
        public Tour()
        {
            segments = new List<(string, string)>();
            cost = 0;
        }

        // NOUVEAU : Constructeur requis par les tests de persistance
        public Tour(List<string> sequence, float totalCost)
        {
            segments = new List<(string, string)>();
            cost = totalCost;

            // On construit les segments à partir de la séquence fournie
            if (sequence != null && sequence.Count > 1)
            {
                for (int i = 0; i < sequence.Count - 1; i++)
                {
                    segments.Add((sequence[i], sequence[i + 1]));
                }
            }
        }

        // ===================== PROPRIÉTÉS =====================

        public float Cost
        {
            get { return cost; }
        }

        public int NbSegments
        {
            get { return segments.Count; }
        }

        // NOUVEAU : Propriété requise par les tests pour vérifier la séquence
        public IList<string> Vertices
        {
            get
            {
                List<string> vertices = new List<string>();
                if (segments.Count > 0)
                {
                    // On ajoute la source de chaque segment
                    foreach (var s in segments)
                    {
                        vertices.Add(s.source);
                    }
                    // On n'oublie pas la destination finale du dernier segment pour boucler
                    vertices.Add(segments[segments.Count - 1].destination);
                }
                return vertices;
            }
        }

        // ===================== MÉTHODES =====================

        public bool ContainsSegment((string source, string destination) segment)
        {
            return segments.Contains(segment);
        }

        public void Print()
        {
            Console.WriteLine($"Coût total : {cost}");
            Console.WriteLine("Segments :");

            foreach (var s in segments)
            {
                Console.WriteLine($"{s.source} -> {s.destination}");
            }
        }

        // ===================== MÉTHODES UTILES =====================

        public void AddSegment(string source, string destination, float weight)
        {
            segments.Add((source, destination));
            cost += weight;
        }

        public void SetCost(float cost)
        {
            this.cost = cost;
        }

        public List<(string source, string destination)> GetSegments()
        {
            return new List<(string, string)>(segments);
        }
    }
}