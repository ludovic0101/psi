using System;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
{
    // ─────────────────────────────────────────────────────────────────────
    // Attributs privés
    // ─────────────────────────────────────────────────────────────────────

    private readonly string _connectionString;

    // ─────────────────────────────────────────────────────────────────────
    // Constructeur
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Instancie un service de persistance et se connecte automatiquement
    /// à la base de données <paramref name="dbname"/> sur le serveur
    /// à l'adresse IP <paramref name="serverIp"/>.
    /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
    /// et <paramref name="pwd"/> (mot de passe).
    /// </summary>
    public ServicePersistance(string serverIp, string dbname, string user, string pwd)
    {
        _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

        // Tester la connexion dès la construction
        using (var conn = OpenConnection())
        {
            // Si OpenConnection réussit, la connexion est valide.
            // Le bloc 'using' fermera automatiquement la connexion à la sortie.
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    // Méthodes publiques
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Sauvegarde le graphe <paramref name="g"/> en base de données
    /// (sommets et arcs inclus) et renvoie son identifiant.
    /// </summary>
    public uint SaveGraph(Graph g)
    {
        uint graphId = 0;

        using (var conn = OpenConnection())
        {
            // 1. INSERT dans la table Graphe
            // On sauvegarde aussi les propriétés internes du graphe pour pouvoir le reconstruire
            string sqlGraphe = "INSERT INTO Graphe (nb_sommets, is_directed, no_edge_value) VALUES (@nb, @dir, @noEdge); SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(sqlGraphe, conn))
            {
                cmd.Parameters.AddWithValue("@nb", g.Order);
                cmd.Parameters.AddWithValue("@dir", g.Directed);
                cmd.Parameters.AddWithValue("@noEdge", g.NoEdgeValue);
                graphId = Convert.ToUInt32(cmd.ExecuteScalar());
            }

            // 2. INSERT des Sommets
            // Dictionnaire pour conserver la correspondance entre le nom du sommet (C#) et son ID en base (MySQL)
            var vertexNameToDbId = new Dictionary<string, uint>();
            foreach (string vertexName in g.Vertices)
            {
                string sqlSommet = "INSERT INTO Sommet (nom, valeur, graphe_id) VALUES (@nom, @val, @gid); SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sqlSommet, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", vertexName);
                    cmd.Parameters.AddWithValue("@val", g.GetVertexValue(vertexName));
                    cmd.Parameters.AddWithValue("@gid", graphId);
                    
                    uint sommetId = Convert.ToUInt32(cmd.ExecuteScalar());
                    vertexNameToDbId[vertexName] = sommetId;
                }
            }

            // 3. INSERT des Arcs
            foreach (string src in g.Vertices)
            {
                foreach (string dest in g.Vertices)
                {
                    if (g.ContainsEdge(src, dest))
                    {
                        // En non-orienté, la classe Graph gère déjà la symétrie. 
                        // Pour éviter les doublons en BDD, on ne sauvegarde qu'un sens si le graphe n'est pas orienté.
                        if (!g.Directed && string.Compare(src, dest) > 0)
                            continue;

                        string sqlArc = "INSERT INTO Arc (sommet_source_id, sommet_dest_id, poids, graphe_id) VALUES (@srcId, @destId, @poids, @gid);";
                        using (var cmd = new MySqlCommand(sqlArc, conn))
                        {
                            cmd.Parameters.AddWithValue("@srcId", vertexNameToDbId[src]);
                            cmd.Parameters.AddWithValue("@destId", vertexNameToDbId[dest]);
                            cmd.Parameters.AddWithValue("@poids", g.GetEdgeWeight(src, dest));
                            cmd.Parameters.AddWithValue("@gid", graphId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        return graphId;
    }

    /// <summary>
    /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
    /// et renvoie une instance de la classe <see cref="Graph"/>.
    /// </summary>
    public Graph LoadGraph(uint id)
    {
        Graph g = null;

        using (var conn = OpenConnection())
        {
            // 1. Récupération des informations du Graphe
            string sqlGraphe = "SELECT is_directed, no_edge_value FROM Graphe WHERE id = @id;";
            using (var cmd = new MySqlCommand(sqlGraphe, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool isDirected = reader.GetBoolean("is_directed");
                        float noEdgeValue = reader.GetFloat("no_edge_value");
                        g = new Graph(isDirected, noEdgeValue);
                    }
                    else
                    {
                        throw new Exception($"Aucun graphe trouvé avec l'identifiant {id}.");
                    }
                }
            }

            // 2. Récupération des Sommets
            var dbIdToVertexName = new Dictionary<uint, string>();
            string sqlSommet = "SELECT id, nom, valeur FROM Sommet WHERE graphe_id = @id ORDER BY id ASC;";
            using (var cmd = new MySqlCommand(sqlSommet, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint sommetId = reader.GetUInt32("id");
                        string nom = reader.GetString("nom");
                        float valeur = reader.GetFloat("valeur");

                        g.AddVertex(nom, valeur);
                        dbIdToVertexName[sommetId] = nom;
                    }
                }
            }

            // 3. Récupération des Arcs
            string sqlArc = "SELECT sommet_source_id, sommet_dest_id, poids FROM Arc WHERE graphe_id = @id;";
            using (var cmd = new MySqlCommand(sqlArc, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint srcId = reader.GetUInt32("sommet_source_id");
                        uint destId = reader.GetUInt32("sommet_dest_id");
                        float poids = reader.GetFloat("poids");

                        string srcName = dbIdToVertexName[srcId];
                        string destName = dbIdToVertexName[destId];

                        g.AddEdge(srcName, destName, poids);
                    }
                }
            }
        }

        return g;
    }

    /// <summary>
    /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
    /// identifié par <paramref name="graphId"/>) en base de données
    /// et renvoie son identifiant.
    /// </summary>
    public uint SaveTour(uint graphId, Tour t)
    {
        uint tourneeId = 0;

        using (var conn = OpenConnection())
        {
            // 1. INSERT dans Tournee
            string sqlTournee = "INSERT INTO Tournee (cout_total, graphe_id) VALUES (@cout, @gid); SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(sqlTournee, conn))
            {
                cmd.Parameters.AddWithValue("@cout", t.Cost);
                cmd.Parameters.AddWithValue("@gid", graphId);
                tourneeId = Convert.ToUInt32(cmd.ExecuteScalar());
            }

            // 2. Extraire la séquence ordonnée des sommets
            var segments = t.GetSegments();
            if (segments.Count > 0)
            {
                // Mappage Nom Sommet -> ID en base (pour ce graphe précis)
                var vertexNameToDbId = new Dictionary<string, uint>();
                string sqlSommets = "SELECT id, nom FROM Sommet WHERE graphe_id = @gid;";
                using (var cmdS = new MySqlCommand(sqlSommets, conn))
                {
                    cmdS.Parameters.AddWithValue("@gid", graphId);
                    using (var reader = cmdS.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vertexNameToDbId[reader.GetString("nom")] = reader.GetUInt32("id");
                        }
                    }
                }

                // Construction de la séquence ordonnée
                List<string> sequence = new List<string>();
                foreach (var segment in segments)
                {
                    sequence.Add(segment.source);
                }
                // On ajoute la destination du tout dernier segment pour boucler la tournée
                sequence.Add(segments[segments.Count - 1].destination);

                // 3. INSERT dans EtapeTournee
                int numeroOrdre = 1;
                foreach (string nomSommet in sequence)
                {
                    string sqlEtape = "INSERT INTO EtapeTournee (tournee_id, numero_ordre, sommet_id) VALUES (@tid, @ordre, @sid);";
                    using (var cmdE = new MySqlCommand(sqlEtape, conn))
                    {
                        cmdE.Parameters.AddWithValue("@tid", tourneeId);
                        cmdE.Parameters.AddWithValue("@ordre", numeroOrdre++);
                        cmdE.Parameters.AddWithValue("@sid", vertexNameToDbId[nomSommet]);
                        cmdE.ExecuteNonQuery();
                    }
                }
            }
        }

        return tourneeId;
    }

    /// <summary>
    /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
    /// et renvoie une instance de la classe <see cref="Tour"/>.
    /// </summary>
    public Tour LoadTour(uint id)
    {
        Tour t = new Tour();

        using (var conn = OpenConnection())
        {
            uint graphId = 0;
            float expectedCost = 0;

            // 1. SELECT des infos de la Tournee
            string sqlTournee = "SELECT graphe_id, cout_total FROM Tournee WHERE id = @id;";
            using (var cmd = new MySqlCommand(sqlTournee, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        graphId = reader.GetUInt32("graphe_id");
                        expectedCost = reader.GetFloat("cout_total");
                    }
                    else
                    {
                        throw new Exception($"Tournée avec l'identifiant {id} introuvable.");
                    }
                }
            }

            // 2. SELECT de la séquence des sommets via les étapes
            List<string> sequence = new List<string>();
            string sqlEtapes = @"
                SELECT s.nom 
                FROM EtapeTournee e
                JOIN Sommet s ON e.sommet_id = s.id
                WHERE e.tournee_id = @id
                ORDER BY e.numero_ordre ASC;";
            using (var cmd = new MySqlCommand(sqlEtapes, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sequence.Add(reader.GetString("nom"));
                    }
                }
            }

            // 3. Récupération des poids d'arcs pour reconstruire les segments
            // On récupère tous les arcs liés au graphe concerné
            var edgeWeights = new Dictionary<(string, string), float>();
            string sqlArcs = @"
                SELECT s1.nom AS src, s2.nom AS dest, a.poids
                FROM Arc a
                JOIN Sommet s1 ON a.sommet_source_id = s1.id
                JOIN Sommet s2 ON a.sommet_dest_id = s2.id
                WHERE a.graphe_id = @gid;";
            using (var cmd = new MySqlCommand(sqlArcs, conn))
            {
                cmd.Parameters.AddWithValue("@gid", graphId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string src = reader.GetString("src");
                        string dest = reader.GetString("dest");
                        float poids = reader.GetFloat("poids");
                        edgeWeights[(src, dest)] = poids;
                        edgeWeights[(dest, src)] = poids; // Sécurité pour les graphes non-orientés
                    }
                }
            }

            // 4. Reconstruction de l'objet Tour
            t.SetCost(0); // Assure un départ propre (le cout s'incrémente avec AddSegment)
            for (int i = 0; i < sequence.Count - 1; i++)
            {
                string source = sequence[i];
                string destination = sequence[i + 1];
                float weight = edgeWeights.ContainsKey((source, destination)) 
                               ? edgeWeights[(source, destination)] 
                               : 0;

                t.AddSegment(source, destination, weight);
            }

            // On force le SetCost final pour pallier toute perte de précision flottante lors de la reconstruction
            t.SetCost(expectedCost);
        }

        return t;
    }

    // ─────────────────────────────────────────────────────────────────────
    // Méthodes utilitaires privées
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Crée et retourne une nouvelle connexion MySQL ouverte.
    /// </summary>
    private MySqlConnection OpenConnection()
    {
        var conn = new MySqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
}
