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
    