ALTER TABLE vehicleType
MODIFY COLUMN truckSpace decimal(6,2);

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