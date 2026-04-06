namespace TourneeFutee
{
    public class Tour
    {
        // ===================== ATTRIBUTS =====================

        private List<(string source, string destination)> segments;
        private float cost;

        // ===================== CONSTRUCTEUR =====================

        public Tour()
        {
            segments = new List<(string, string)>();
            cost = 0;
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