import re
import random
import string

import mysql.connector

random.seed(10)
    
def generate_license():
    prefix = 'S' + ''.join(random.choices(string.ascii_uppercase, k=2))
    number = ''.join(random.choices(string.digits, k=4))
    suffix = random.choice(string.ascii_uppercase)

    plate_number = prefix + number + suffix

    return plate_number


def retrieve_vehicle_types(cursor):
    sql = "SELECT * FROM vehicleType"
    cursor.execute(sql)

    result = cursor.fetchall()
    return result


def insert_into_table(db, cursor):
    vehicle_types = retrieve_vehicle_types(cursor)
    random_id = random.sample(range(len(vehicle_types)), 10)

    sql = "INSERT INTO vehicle (licensePlate, licenseToOperate, vehicleTypeID) VALUES (%s, %s, %s)"

    for i in random_id:
        try:
            vehicle = (generate_license(), '3A', vehicle_types[i][0])
            print("Inserting vehicle: ", vehicle)
            cursor.execute(sql, vehicle)
        except mysql.connector.Error as err:
            print("Error: {}".format(err))
            continue

    db.commit()

    print("Values successfully inserted.")


mydb = mysql.connector.connect(
    database="vehicleDB",
    host="localhost",
    user="root",
    password="",
)
dbcursor = mydb.cursor()

insert_into_table(mydb, dbcursor)

dbcursor.close()
mydb.close()
print("MySQL connection is closed.")
