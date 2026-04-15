CREATE DATABASE  IF NOT EXISTS `tourneefutee` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `tourneefutee`;
-- MySQL dump 10.13  Distrib 8.0.43, for macos15 (arm64)
--
-- Host: localhost    Database: tourneefutee
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Arc`
--

DROP TABLE IF EXISTS `Arc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Arc` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `source_id` int unsigned DEFAULT NULL,
  `destination_id` int unsigned DEFAULT NULL,
  `poids` double DEFAULT NULL,
  `graphe_id` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `source_id` (`source_id`),
  KEY `destination_id` (`destination_id`),
  KEY `graphe_id` (`graphe_id`),
  CONSTRAINT `arc_ibfk_1` FOREIGN KEY (`source_id`) REFERENCES `Sommet` (`id`) ON DELETE CASCADE,
  CONSTRAINT `arc_ibfk_2` FOREIGN KEY (`destination_id`) REFERENCES `Sommet` (`id`) ON DELETE CASCADE,
  CONSTRAINT `arc_ibfk_3` FOREIGN KEY (`graphe_id`) REFERENCES `Graphe` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Arc`
--

LOCK TABLES `Arc` WRITE;
/*!40000 ALTER TABLE `Arc` DISABLE KEYS */;
/*!40000 ALTER TABLE `Arc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EtapeTournee`
--

DROP TABLE IF EXISTS `EtapeTournee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EtapeTournee` (
  `tournee_id` int unsigned NOT NULL,
  `sommet_id` int unsigned DEFAULT NULL,
  `ordre` int NOT NULL,
  PRIMARY KEY (`tournee_id`,`ordre`),
  KEY `sommet_id` (`sommet_id`),
  CONSTRAINT `etapetournee_ibfk_1` FOREIGN KEY (`tournee_id`) REFERENCES `Tournee` (`id`) ON DELETE CASCADE,
  CONSTRAINT `etapetournee_ibfk_2` FOREIGN KEY (`sommet_id`) REFERENCES `Sommet` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EtapeTournee`
--

LOCK TABLES `EtapeTournee` WRITE;
/*!40000 ALTER TABLE `EtapeTournee` DISABLE KEYS */;
/*!40000 ALTER TABLE `EtapeTournee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Graphe`
--

DROP TABLE IF EXISTS `Graphe`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Graphe` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `nb_sommets` int NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Graphe`
--

LOCK TABLES `Graphe` WRITE;
/*!40000 ALTER TABLE `Graphe` DISABLE KEYS */;
/*!40000 ALTER TABLE `Graphe` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Sommet`
--

DROP TABLE IF EXISTS `Sommet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Sommet` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `valeur` varchar(100) DEFAULT NULL,
  `graphe_id` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `graphe_id` (`graphe_id`),
  CONSTRAINT `sommet_ibfk_1` FOREIGN KEY (`graphe_id`) REFERENCES `Graphe` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Sommet`
--

LOCK TABLES `Sommet` WRITE;
/*!40000 ALTER TABLE `Sommet` DISABLE KEYS */;
/*!40000 ALTER TABLE `Sommet` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tournee`
--

DROP TABLE IF EXISTS `Tournee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tournee` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `cout_total` double DEFAULT NULL,
  `graphe_id` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `graphe_id` (`graphe_id`),
  CONSTRAINT `tournee_ibfk_1` FOREIGN KEY (`graphe_id`) REFERENCES `Graphe` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tournee`
--

LOCK TABLES `Tournee` WRITE;
/*!40000 ALTER TABLE `Tournee` DISABLE KEYS */;
/*!40000 ALTER TABLE `Tournee` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-15  9:01:53
