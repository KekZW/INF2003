To use the python scripts, to insert data
download:
pip install mysql-connector-python
pip install pandas 

run insert_into_database.py first,
then run generate_vehicles.py

Use mySQL workbench
Download: https://dev.mysql.com/downloads/connector/net/
Under "Manage NuGet Package" Download "MySql.Data"

Create a new database 
name: vehicleDB
server: localhost
Username:root
Password=;

Create a schema call vehicledb and run databaseInit.sql to create the table
