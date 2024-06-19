ALTER TABLE vehicleType
MODIFY COLUMN trunkSpace decimal(6,2);

ALTER TABLE license
RENAME COLUMN liceseClass TO licenseClass;

SET FOREIGN_KEY_CHECKS = 0;
ALTER TABLE license
ADD COLUMN userID INT;

ALTER TABLE license
ADD FOREIGN KEY (userID) REFERENCES user(userID);

ALTER TABLE license
MODIFY COLUMN licenseID INT auto_increment;

RENAME TABLE maintenanace TO maintenance;

ALTER TABLE maintenance
    RENAME COLUMN maintainanceID TO maintenanceID;

ALTER TABLE maintenance
    ADD FOREIGN KEY (vehicleID) REFERENCES vehicle(vehicleID);

ALTER TABLE vehicle
    ADD COLUMN status varchar(64);

-- 14/06/2024
SET SQL_SAFE_UPDATES = 0;
    
UPDATE vehicle
SET status="available";

SET SQL_SAFE_UPDATES = 1;


SET FOREIGN_KEY_CHECKS = 1;
    
-- 15/06/2024
ALTER TABLE user
DROP COLUMN username;

ALTER TABLE USER
ADD COLUMN role VARCHAR(5);

-- 17/06/2024

DELIMITER //

CREATE DEFINER=`root`@`localhost` TRIGGER `license_AFTER_INSERT`
AFTER INSERT ON `license`
FOR EACH ROW
BEGIN
    UPDATE `user` SET `licenseId` = NEW.`licenseID` WHERE `userID` = NEW.`userID`;
END //

DELIMITER ;

ALTER TABLE `vehicledb`.`user`
CHANGE COLUMN `name` `name` VARCHAR(50) NULL DEFAULT NULL AFTER `userID`,
CHANGE COLUMN `emailAddress` `emailAddress` VARCHAR(50) NULL DEFAULT NULL AFTER `name`,
ADD UNIQUE INDEX `emailAddress_UNIQUE` (`emailAddress` ASC) VISIBLE;

DROP TRIGGER IF EXISTS `user_AFTER_INSERT`;

CREATE VIEW RentalInProgress
AS
(SELECT DISTINCT vehicleID
FROM rental
WHERE CURRENT_DATE() BETWEEN startRentalDate AND endRentalDate);

CREATE VIEW MaintenanceInProgress
AS
(SELECT DISTINCT m.vehicleID FROM maintenance m WHERE m.finishMaintDate <= CURRENT_DATE() AND m.workshopStatus != 'Completed');

ALTER TABLE `vehicledb`.`user` 
CHANGE COLUMN `role` `role` ENUM('Admin', 'User', 'cls') NULL DEFAULT NULL COMMENT 'ENUM' ;

DROP TRIGGER IF EXISTS `vehicledb`.`rental_BEFORE_INSERT`;

DELIMITER $$
USE `vehicledb`$$
CREATE DEFINER=`root`@`localhost` TRIGGER `rental_BEFORE_INSERT` BEFORE INSERT ON `rental` FOR EACH ROW BEGIN
    Declare rentalID INT;

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
    
    -- Correspond with an error so user cannot have the same dates
	IF rentalID > 0 THEN
		SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'Error';
	END IF;
		
    
END$$
DELIMITER ;

ALTER TABLE `vehicledb`.`vehicle` 
ADD UNIQUE INDEX `licensePlate_UNIQUE` (`licensePlate` ASC) VISIBLE;
;


DROP TRIGGER IF EXISTS vehicledb.rental_BEFORE_UPDATE;

-- Drops Foreign Key and Reinsert
ALTER TABLE `vehicledb`.`vehicle` DROP FOREIGN KEY `vehicle_ibfk_1`;

ALTER TABLE `vehicledb`.`vehicletype` 
CHANGE COLUMN `vehicleTypeID` `vehicleTypeID` INT NOT NULL AUTO_INCREMENT;

ALTER TABLE `vehicledb`.`vehicle` 
ADD CONSTRAINT `vehicle_ibfk_1` FOREIGN KEY (`vehicleTypeID`) 
REFERENCES `vehicletype` (`vehicleTypeID`);

--- 17/06/2024

USE `vehicledb`;
DROP procedure IF EXISTS `Safe_Drop_Vehicle`;

USE `vehicledb`;
DROP procedure IF EXISTS `vehicledb`.`Safe_Drop_Vehicle`;
;

DELIMITER $$
USE `vehicledb`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `Safe_Drop_Vehicle`(IN vehicleID INT)
BEGIN
	DECLARE rount_count INT;
    DECLARE error_messages VARCHAR(255);
    
    SELECT COUNT(r.rentalID) into rount_count FROM rental r WHERE r.vehicleID = vehicleID AND CURRENT_DATE() <= r.endRentalDate;
	
    IF rount_count > 0 THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error: Cannot delete vehicle with active rental.';
    ELSE
		START TRANSACTION;

		SET @orig_foreign_key_checks = @@FOREIGN_KEY_CHECKS;
		SET FOREIGN_KEY_CHECKS = 0;

		-- Perform delete operation
		DELETE FROM vehicle v WHERE v.vehicleID = vehicleID;

		-- Enable foreign key checks
		SET FOREIGN_KEY_CHECKS = @orig_foreign_key_checks;
		
		COMMIT;
	END IF; 
END$$

DELIMITER ;
;

ALTER TABLE `vehicledb`.`maintenance` 
CHANGE COLUMN `finishMaintDate` `startMaintDate` DATE NULL DEFAULT NULL ,
CHANGE COLUMN `LastMaintDate` `endMaintDate` DATE NULL DEFAULT NULL ;


USE `vehicledb`;
CREATE  OR REPLACE 
    ALGORITHM = UNDEFINED 
    DEFINER = `root`@`localhost` 
    SQL SECURITY DEFINER
VIEW `maintenanceinprogress` AS
    SELECT DISTINCT
        `m`.`vehicleID` AS `vehicleID`
    FROM
        `maintenance` `m`
    WHERE
        ((`m`.`startMaintDate` <= CURDATE())
            AND (`m`.`workshopStatus` <> 'Completed'));

DROP TRIGGER IF EXISTS `vehicledb`.`maintenance_AFTER_INSERT`;

DELIMITER $$
USE `vehicledb`$$
CREATE DEFINER=`root`@`localhost` TRIGGER `maintenance_AFTER_INSERT` AFTER INSERT ON `maintenance` FOR EACH ROW BEGIN

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


END$$
DELIMITER ;


DROP TRIGGER IF EXISTS `vehicledb`.`rental_BEFORE_INSERT`;

DELIMITER $$
USE `vehicledb`$$
CREATE DEFINER=`root`@`localhost` TRIGGER `rental_BEFORE_INSERT` BEFORE INSERT ON `rental` FOR EACH ROW BEGIN
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
		
    
END$$
DELIMITER ;

DROP TRIGGER IF EXISTS `vehicledb`.`maintenance_AFTER_UPDATE`;

DELIMITER $$
USE `vehicledb`$$
CREATE DEFINER = CURRENT_USER TRIGGER `vehicledb`.`maintenance_AFTER_UPDATE` AFTER UPDATE ON `maintenance` FOR EACH ROW
BEGIN

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


END$$
DELIMITER ;
