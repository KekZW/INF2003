CREATE DATABASE  IF NOT EXISTS `vehicledb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `vehicledb`;
-- MySQL dump 10.13  Distrib 8.0.36, for macos14 (arm64)
--
-- Host: localhost    Database: vehicledb
-- ------------------------------------------------------
-- Server version	8.3.0

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
-- Table structure for table `license`
--

DROP TABLE IF EXISTS `license`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `license` (
                           `licenseID` int NOT NULL AUTO_INCREMENT,
                           `acquireDate` date DEFAULT NULL,
                           `licenseClass` varchar(20) DEFAULT NULL,
                           `userID` int DEFAULT NULL,
                           PRIMARY KEY (`licenseID`),
                           KEY `userID` (`userID`),
                           CONSTRAINT `license_ibfk_1` FOREIGN KEY (`userID`) REFERENCES `user` (`userID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `license`
--

LOCK TABLES `license` WRITE;
/*!40000 ALTER TABLE `license` DISABLE KEYS */;
INSERT INTO `license` VALUES (1,'2022-06-20','3',1),(2,'2020-02-14','3',2),(3,'2024-06-11','3A',3),(4,'2024-06-22','2B',4),(5,'2024-06-04','3A',5);
/*!40000 ALTER TABLE `license` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `license_AFTER_INSERT` AFTER INSERT ON `license` FOR EACH ROW BEGIN
    UPDATE `user` SET `licenseId` = NEW.`licenseID` WHERE `userID` = NEW.`userID`;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `maintenance`
--

DROP TABLE IF EXISTS `maintenance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `maintenance` (
                               `maintenanceID` int NOT NULL AUTO_INCREMENT,
                               `vehicleID` int DEFAULT NULL,
                               `workshopStatus` varchar(256) DEFAULT NULL,
                               `startMaintDate` date DEFAULT NULL,
                               `endMaintDate` date DEFAULT NULL,
                               PRIMARY KEY (`maintenanceID`),
                               KEY `vehicleID` (`vehicleID`),
                               CONSTRAINT `maintenance_ibfk_1` FOREIGN KEY (`vehicleID`) REFERENCES `vehicle` (`vehicleID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `maintenance`
--

LOCK TABLES `maintenance` WRITE;
/*!40000 ALTER TABLE `maintenance` DISABLE KEYS */;
INSERT INTO `maintenance` VALUES (1,2,'Completed','2024-06-22','2024-06-30'),(2,12,'Completed','2024-06-22','2024-06-30'),(3,1,'Completed','2024-08-03','2024-08-06'),(4,4,'Completed','2024-08-03','2024-08-05');
/*!40000 ALTER TABLE `maintenance` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `maintenance_AFTER_INSERT` AFTER INSERT ON `maintenance` FOR EACH ROW BEGIN
DELETE FROM rental r
WHERE EXISTS (
    SELECT 1
    FROM maintenance m
    WHERE m.vehicleID = NEW.vehicleID
      AND (
          (r.startRentalDate BETWEEN NEW.startMaintDate AND NEW.endMaintDate)
          OR (r.endRentalDate BETWEEN NEW.startMaintDate AND NEW.endMaintDate)
      ) AND NEW.workshopStatus = "In Maintenance"
);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `maintenance_AFTER_UPDATE` AFTER UPDATE ON `maintenance` FOR EACH ROW BEGIN
DELETE FROM rental r
WHERE EXISTS (
    SELECT 1
    FROM maintenance m
    WHERE m.vehicleID = NEW.vehicleID
      AND (
          (r.startRentalDate BETWEEN NEW.startMaintDate AND NEW.endMaintDate)
          OR (r.endRentalDate BETWEEN NEW.startMaintDate AND NEW.endMaintDate)
      ) AND NEW.workshopStatus = "In Maintenance"
);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `maintenanceinprogress`
--

DROP TABLE IF EXISTS `maintenanceinprogress`;
/*!50001 DROP VIEW IF EXISTS `maintenanceinprogress`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `maintenanceinprogress` AS SELECT 
 1 AS `vehicleID`*/;
SET character_set_client = @saved_cs_client;

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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
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
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
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

--
-- Temporary view structure for view `rentalinprogress`
--

DROP TABLE IF EXISTS `rentalinprogress`;
/*!50001 DROP VIEW IF EXISTS `rentalinprogress`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `rentalinprogress` AS SELECT 
 1 AS `vehicleID`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
                        `userID` int NOT NULL AUTO_INCREMENT,
                        `name` varchar(50) DEFAULT NULL,
                        `emailAddress` varchar(50) DEFAULT NULL,
                        `userPassword` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
                        `address` varchar(256) DEFAULT NULL,
                        `licenseID` int DEFAULT NULL,
                        `phoneNo` varchar(20) DEFAULT NULL,
                        `role` enum('Admin','User','cls') DEFAULT NULL COMMENT 'ENUM',
                        PRIMARY KEY (`userID`),
                        UNIQUE KEY `emailAddress_UNIQUE` (`emailAddress`),
                        KEY `licenseID` (`licenseID`),
                        CONSTRAINT `user_ibfk_1` FOREIGN KEY (`licenseID`) REFERENCES `license` (`licenseID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'test admin','admin@gmail.com','p6KGmCl5eAez5XbBeHi0ZkSeieYER9VBd1qW63waXe0=','Blk 123 Test Avenue',1,'91829191','Admin'),(2,'test user','user@gmail.com','p6KGmCl5eAez5XbBeHi0ZkSeieYER9VBd1qW63waXe0=','Blk 123 User Avenue',2,'91819191','User'),(3,'Admin Test','admin@gg.com','py/Mf9BbfDQv3KpDBQ6U5tnOTq779CjKze0eYRZuK9M=','ABC123 ABC',3,'12345678','Admin'),(4,'User Test','user@gg.com','py/Mf9BbfDQv3KpDBQ6U5tnOTq779CjKze0eYRZuK9M=','ABC12345',4,'12345678','User'),(5,'Test User','testuser@gg.com','py/Mf9BbfDQv3KpDBQ6U5tnOTq779CjKze0eYRZuK9M=','QWERTY123',5,'12345678','User');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vehicle`
--

DROP TABLE IF EXISTS `vehicle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicle` (
                           `vehicleID` int NOT NULL AUTO_INCREMENT,
                           `licensePlate` varchar(10) DEFAULT NULL,
                           `licenseToOperate` varchar(30) DEFAULT NULL,
                           `vehicleTypeID` int DEFAULT NULL,
                           PRIMARY KEY (`vehicleID`),
                           UNIQUE KEY `licensePlate_UNIQUE` (`licensePlate`),
                           KEY `vehicle_ibfk_1` (`vehicleTypeID`),
                           KEY `idx_vehicle_covering` (`vehicleID`,`vehicleTypeID`,`licensePlate`,`licenseToOperate`),
                           CONSTRAINT `vehicle_ibfk_1` FOREIGN KEY (`vehicleTypeID`) REFERENCES `vehicletype` (`vehicleTypeID`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicle`
--

LOCK TABLES `vehicle` WRITE;
/*!40000 ALTER TABLE `vehicle` DISABLE KEYS */;
INSERT INTO `vehicle` VALUES (1,'SQE5329L','3A',3),(2,'SBW6326O','3A',28),(4,'SPB0193L','3A',1),(5,'SUG6446K','3A',14),(6,'SVE2404T','3A',15),(7,'SRN8153T','3A',27),(8,'SDZ7491L','3A',16),(9,'SLK2261D','3A',30),(10,'SGH6911Y','3A',9),(11,'SFK2356D','3A',30);
/*!40000 ALTER TABLE `vehicle` ENABLE KEYS */;
UNLOCK TABLES;

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
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicletype`
--

LOCK TABLES `vehicletype` WRITE;
/*!40000 ALTER TABLE `vehicletype` DISABLE KEYS */;
INSERT INTO `vehicletype` VALUES (1,'Nissan','GT-R','Sports Car',2,74.00,'Petrol',315.00,110.00),(2,'Volkswagen','Virtus','Sedan',5,45.00,'Petrol',521.00,60.00),(3,'Honda','Amaze','Compact Sedan',5,35.00,'Petrol',420.00,55.00),(4,'Volkswagen','Tiguan','SUV',5,60.00,'Petrol',616.00,80.00),(5,'Honda','WR-V','Compact SUV',5,40.00,'Petrol',363.00,65.00),(6,'Honda','Jazz','Hatchback',5,40.00,'Petrol',354.00,50.00),(7,'Nissan','Kicks','SUV',5,50.00,'Petrol',400.00,80.00),(8,'Citroen','C5 Aircross','SUV',5,52.50,'Diesel',580.00,90.00),(9,'Honda','City 5th Gen','Sedan',5,40.00,'Petrol',506.00,60.00),(10,'Hyundai','Grand i10 Nios','Hatchback',5,37.00,'Petrol',260.00,40.00),(11,'Hyundai','Aura','Compact Sedan',5,37.00,'Petrol',402.00,45.00),(12,'Toyota','Fortuner','SUV',7,80.00,'Diesel',296.00,80.00),(13,'Kia','Sonet','Compact SUV',5,45.00,'Petrol',392.00,65.00),(14,'Tata','Safari','SUV',7,50.00,'Diesel',447.00,70.00),(15,'Kia','Seltos','SUV',5,50.00,'Petrol',433.00,80.00),(16,'Toyota','Urban Cruiser','Compact SUV',5,48.00,'Petrol',328.00,65.00),(17,'Toyota','Fortuner Legender','SUV',7,80.00,'Diesel',296.00,80.00),(18,'Honda','City 4th Gen','Sedan',5,40.00,'Petrol',510.00,60.00),(19,'Honda','City e-HEV','Sedan',5,40.00,'Petrol Hybrid',506.00,60.00),(20,'Hyundai','Creta','SUV',5,50.00,'Diesel',433.00,70.00),(21,'Hyundai','i20','Hatchback',5,37.00,'Petrol',326.00,40.00),(22,'Hyundai','Alcazar','SUV',6,50.00,'Diesel',180.00,70.00),(23,'Toyota','Glanza','Hatchback',5,37.00,'Petrol',318.00,50.00),(24,'Hyundai','Verna','Sedan',5,45.00,'Petrol',480.00,50.00),(25,'Hyundai','i20 N-Line','Hatchback',5,37.00,'Petrol',326.00,40.00),(26,'Hyundai','Venue','Compact SUV',5,45.00,'Petrol',350.00,55.00),(27,'Hyundai','Venue N-Line','Compact SUV',5,45.00,'Petrol',350.00,55.00),(28,'Kia','Carens','MUV',6,45.00,'Diesel',216.00,70.00),(29,'Tata','Harrier','SUV',5,50.00,'Diesel',425.00,70.00),(30,'Kia','Carnival','MPV',7,60.00,'Diesel',540.00,75.00),(31,'Renault','Kiger','Compact SUV',5,40.00,'Petrol',405.00,55.00),(32,'Citroen','C3','Hatchback',5,30.00,'Petrol',315.00,60.00),(33,'Renault','Kwid','Hatchback',5,28.00,'Petrol',279.00,40.00),(34,'Renault','Triber','MUV',7,40.00,'Petrol',625.00,60.00),(35,'Volkswagen','Taigun','Mid Size SUV',5,50.00,'Petrol',385.00,75.00),(36,'Nissan','Magnite','Compact SUV',5,40.00,'Petrol',336.00,65.00);
/*!40000 ALTER TABLE `vehicletype` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'vehicledb'
--

--
-- Final view structure for view `maintenanceinprogress`
--

/*!50001 DROP VIEW IF EXISTS `maintenanceinprogress`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `maintenanceinprogress` AS select distinct `m`.`vehicleID` AS `vehicleID` from `maintenance` `m` where ((`m`.`startMaintDate` <= curdate()) and (`m`.`workshopStatus` <> 'Completed')) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `rentalinprogress`
--

/*!50001 DROP VIEW IF EXISTS `rentalinprogress`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `rentalinprogress` AS select distinct `rental`.`vehicleID` AS `vehicleID` from `rental` where (curdate() between `rental`.`startRentalDate` and `rental`.`endRentalDate`) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-08-03 21:25:06
