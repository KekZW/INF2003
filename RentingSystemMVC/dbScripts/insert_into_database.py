import pandas as pd
import mysql.connector

db_host='localhost'
db_user="root"

df = pd.read_csv("Cars_India_dataset.csv")
new_df = df.drop(df[df['Maker'] == 'Mahindra'].index)
new_df = new_df.drop_duplicates(subset=['Model'])
new_df = new_df.dropna()

base_rates = {
    'Sports Car': 100,
    'SUV': 60,
    'Sedan': 40,
    'Compact Sedan': 35,
    'Compact SUV': 45,
    'Hatchback': 30,
    'MPV': 55,
    'MUV': 50,
    'Mid Size SUV': 55
}

seat_adjustments = {
    '2-4': 0,
    '5-7': 10,
    '8+': 20
}

brand_premiums = {
    'Tata': 0,
    'Renault': 0,
    'Hyundai': 0,
    'Nissan': 10,
    'Volkswagen': 10,
    'Honda': 10,
    'Toyota': 10,
    'Kia': 10,
    'Citroen': 20
}

mydb = mysql.connector.connect(
    database="vehicleDB",
    host=db_host,
    user=db_user,
    password=""
)
cursor = mydb.cursor()
counter = 1
for index, row in new_df.iterrows():
    price = base_rates.get(row.Type, 0) + brand_premiums.get(row.Maker, 0)

    if 5 <= row.Seats <= 7:
        price += seat_adjustments.get('5-7')
    elif row.Seats >= 8:
        price += seat_adjustments.get('8+')
        
    # vehicleTypeID, brand, model, type, seats, fuelCapacity, fuelType,truckSpace, rentalCostPerDay
    statement = (counter, row['Maker'], row['Model'], row['Type'], row['Seats'], row['Fuel Tank Capacity'], row['Fuel'], row['Boot Space'], price)
    sql = 'INSERT INTO vehicleType VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)'
    cursor.execute(sql, statement)
    mydb.commit()

    counter += 1

cursor.close()
mydb.close()
print("MySQL connection is closed.")
