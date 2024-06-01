CREATE TABLE IF NOT EXISTS license (
licenseID INT PRIMARY KEY AUTO_INCREMENT,
acquireDate date,
licenseClass VARCHAR(20),
userID INT,
FOREIGN KEY (userID) REFERENCES user(userID);

CREATE TABLE IF NOT EXISTS vehicleType(
vehicleTypeID INT PRIMARY KEY,
brand VARCHAR(20),
model VARCHAR(20),
type VARCHAR(20),
seats INT,
fuelCapacity DECIMAL(6,2),
fuelType VARCHAR(20),
truckSpace DECIMAL(6,2),
rentalCostPerDay DECIMAL(6,2));


CREATE TABLE IF NOT EXISTS vehicle(
vehicleID INT PRIMARY KEY AUTO_INCREMENT,
licensePlate VARCHAR(10),
licenseToOperate VARCHAR(30),
vehicleTypeID INT,
FOREIGN KEY (vehicleTypeID) REFERENCES vehicleType(vehicleTypeID));

CREATE TABLE IF NOT EXISTS maintenanace(
maintainanceID INT PRIMARY KEY AUTO_INCREMENT,
vehicleID INT,
workshopStatus VARCHAR(256),
finishMaintDate DATE,
LastMaintDate DATE);

CREATE TABLE IF NOT EXISTS user (
userID INT PRIMARY KEY AUTO_INCREMENT, 
username VARCHAR(30),
userPassword NVARCHAR(200),
name VARCHAR(50),
address VARCHAR(256),
licenseID INT,
emailAddress VARCHAR(50),
phoneNo VARCHAR(20),
FOREIGN KEY (licenseID) REFERENCES license(licenseID));

CREATE TABLE IF NOT EXISTS rental (
rentalID INT PRIMARY KEY AUTO_INCREMENT,
userID INT,
vehicleID INT,
startRentalDate DATE,
endRentalDate DATE,
rentalAmount DECIMAL(6,2),
rentalAddress VARCHAR(256),
rentalLot INT,
FOREIGN KEY (userID) REFERENCES user(userID),
FOREIGN KEY (vehicleID) REFERENCES vehicle(vehicleID));
