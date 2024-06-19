CREATE DATABASE  IF NOT EXISTS `vehicledb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `vehicledb`;
-- MySQL dump 10.13  Distrib 8.0.31, for Win64 (x86_64)
--
-- Host: localhost    Database: vehicledb
-- ------------------------------------------------------
-- Server version	8.0.31

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
-- Table structure for table `vehicletype`
--

DROP TABLE IF EXISTS `vehicletype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicletype` (
  `vehicleTypeID` int NOT NULL AUTO_INCREMENT,
  `brand` varchar(20) DEFAULT NULL,
  `model` varchar(20) DEFAULT NULL,
  `type` varchar(20) DEFAULT NULL,
  `seats` int DEFAULT NULL,
  `fuelCapacity` decimal(6,2) DEFAULT NULL,
  `fuelType` varchar(20) DEFAULT NULL,
  `trunkSpace` decimal(6,2) DEFAULT NULL,
  `rentalCostPerDay` decimal(6,2) DEFAULT NULL,
  PRIMARY KEY (`vehicleTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicletype`
--

LOCK TABLES `vehicletype` WRITE;
/*!40000 ALTER TABLE `vehicletype` DISABLE KEYS */;
INSERT INTO `vehicletype` VALUES (1,'Nissan','GT-R','Sports Car',2,74.00,'Petrol',315.00,110.00),(2,'Volkswagen','Virtus','Sedan',5,45.00,'Petrol',521.00,60.00),(3,'Honda','Amaze','Compact Sedan',5,35.00,'Petrol',420.00,55.00),(4,'Volkswagen','Tiguan','SUV',5,60.00,'Petrol',616.00,80.00),(5,'Honda','WR-V','Compact SUV',5,40.00,'Petrol',363.00,65.00),(6,'Honda','Jazz','Hatchback',5,40.00,'Petrol',354.00,50.00),(7,'Nissan','Kicks','SUV',5,50.00,'Petrol',400.00,80.00),(8,'Citroen','C5 Aircross','SUV',5,52.50,'Diesel',580.00,90.00),(9,'Honda','City 5th Gen','Sedan',5,40.00,'Petrol',506.00,60.00),(10,'Hyundai','Grand i10 Nios','Hatchback',5,37.00,'Petrol',260.00,40.00),(11,'Hyundai','Aura','Compact Sedan',5,37.00,'Petrol',402.00,45.00),(12,'Toyota','Fortuner','SUV',7,80.00,'Diesel',296.00,80.00),(13,'Kia','Sonet','Compact SUV',5,45.00,'Petrol',392.00,65.00),(14,'Tata','Safari','SUV',7,50.00,'Diesel',447.00,70.00),(15,'Kia','Seltos','SUV',5,50.00,'Petrol',433.00,80.00),(16,'Toyota','Urban Cruiser','Compact SUV',5,48.00,'Petrol',328.00,65.00),(17,'Toyota','Fortuner Legender','SUV',7,80.00,'Diesel',296.00,80.00),(18,'Honda','City 4th Gen','Sedan',5,40.00,'Petrol',510.00,60.00),(19,'Honda','City e-HEV','Sedan',5,40.00,'Petrol Hybrid',506.00,60.00),(20,'Hyundai','Creta','SUV',5,50.00,'Diesel',433.00,70.00),(21,'Hyundai','i20','Hatchback',5,37.00,'Petrol',326.00,40.00),(22,'Hyundai','Alcazar','SUV',6,50.00,'Diesel',180.00,70.00),(23,'Toyota','Glanza','Hatchback',5,37.00,'Petrol',318.00,50.00),(24,'Hyundai','Verna','Sedan',5,45.00,'Petrol',480.00,50.00),(25,'Hyundai','i20 N-Line','Hatchback',5,37.00,'Petrol',326.00,40.00),(26,'Hyundai','Venue','Compact SUV',5,45.00,'Petrol',350.00,55.00),(27,'Hyundai','Venue N-Line','Compact SUV',5,45.00,'Petrol',350.00,55.00),(28,'Kia','Carens','MUV',6,45.00,'Diesel',216.00,70.00),(29,'Tata','Harrier','SUV',5,50.00,'Diesel',425.00,70.00),(30,'Kia','Carnival','MPV',7,60.00,'Diesel',540.00,75.00),(31,'Renault','Kiger','Compact SUV',5,40.00,'Petrol',405.00,55.00),(32,'Citroen','C3','Hatchback',5,30.00,'Petrol',315.00,60.00),(33,'Renault','Kwid','Hatchback',5,28.00,'Petrol',279.00,40.00),(34,'Renault','Triber','MUV',7,40.00,'Petrol',625.00,60.00),(35,'Volkswagen','Taigun','Mid Size SUV',5,50.00,'Petrol',385.00,75.00),(36,'Nissan','Magnite','Compact SUV',5,40.00,'Petrol',336.00,65.00);
/*!40000 ALTER TABLE `vehicletype` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-19 13:30:47
