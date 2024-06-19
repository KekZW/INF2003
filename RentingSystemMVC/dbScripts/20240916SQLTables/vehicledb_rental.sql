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
-- Table structure for table `rental`
--

DROP TABLE IF EXISTS `rental`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rental` (
  `rentalID` int NOT NULL AUTO_INCREMENT,
  `userID` int DEFAULT NULL,
  `vehicleID` int DEFAULT NULL,
  `startRentalDate` date DEFAULT NULL,
  `endRentalDate` date DEFAULT NULL,
  `rentalAmount` decimal(6,2) DEFAULT NULL,
  `rentalAddress` varchar(256) DEFAULT NULL,
  `rentalLot` int DEFAULT NULL,
  PRIMARY KEY (`rentalID`),
  KEY `userID` (`userID`),
  KEY `vehicleID` (`vehicleID`),
  CONSTRAINT `rental_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `user` (`userID`),
  CONSTRAINT `rental_ibfk_2` FOREIGN KEY (`vehicleID`) REFERENCES `vehicle` (`vehicleID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rental`
--

LOCK TABLES `rental` WRITE;
/*!40000 ALTER TABLE `rental` DISABLE KEYS */;
/*!40000 ALTER TABLE `rental` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `rental_BEFORE_INSERT` BEFORE INSERT ON `rental` FOR EACH ROW BEGIN
    Declare rentalID INT;
    Declare maintenanceID INT;

    IF (NEW.startRentalDate < CURRENT_DATE() OR NEW.endRentalDate < DATE_ADD(CURRENT_DATE(), INTERVAL 1 DAY)) 
    OR (NEW.endRentalDate < NEW.startRentalDate) THEN 
		SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'Error';
	END IF;
    
      -- Check if there is corresponding item between the dates 
    SELECT COUNT(r.rentalID) into rentalID FROM rental r 
    WHERE r.userID = NEW.userID AND
    ((NEW.startRentalDate BETWEEN r.startRentalDate AND r.endRentalDate)
    OR
    (NEW.endRentalDate BETWEEN r.startRentalDate AND r.endRentalDate));
    
    
	SELECT COUNT(m.maintenanceID) INTO maintenanceID
	FROM maintenance m
	WHERE m.vehicleID = NEW.vehicleID
	AND (
		(NEW.startRentalDate BETWEEN m.startMaintDate AND m.endMaintDate)
		OR
		(NEW.endRentalDate BETWEEN m.startMaintDate AND m.endMaintDate)
	)
	AND m.workshopStatus = 'In Maintenance';
    
    -- Correspond with an error so user cannot have the same dates
	IF rentalID > 0 OR maintenanceID > 0 THEN
		SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'Error';
	END IF;
		
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-19 13:30:47
