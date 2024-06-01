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
SET FOREIGN_KEY_CHECKS = 1;