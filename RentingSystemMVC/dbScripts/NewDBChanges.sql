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

-- 16/06/2024

CREATE DEFINER=`root`@`localhost` TRIGGER `user_AFTER_INSERT` AFTER INSERT ON `user` FOR EACH ROW BEGIN

-- Retrieve last inserted userID
DECLARE new_user_id INT;

SET new_user_id = New.UserID;

-- Updates the last inserted based on the values
UPDATE license SET userID = new_user_id WHERE licenseID = (SELECT licenseID from user WHERE UserID = new_user_id);

END

ALTER TABLE `vehicledb`.`user` 
ADD UNIQUE INDEX `emailAddress_UNIQUE` (`emailAddress` ASC) VISIBLE;
;
