import mysql.connector
import paho.mqtt.client as mqtt
import datetime
import configparser
import json

config = configparser.RawConfigParser()
config.read('dbconnection.properties')

connection = mysql.connector.connect(
    host = config.get('DatabaseSection', 'database.host'),
    user = config.get('DatabaseSection', 'database.user'),
    password = config.get('DatabaseSection', 'database.password'),
    database = config.get('DatabaseSection', 'database.dbname')
)

broker = "localhost"
topic = "machines/sensors/+"

print("Connected!")


cursor = connection.cursor()

def on_message(client, userdata, msg):
    payload = msg.payload.decode()
    data_json = json.loads(payload)
    try:
        Name  = (data_json ["Name"])
        Temperature  = (data_json ["Temperature"])
        Humidity = (data_json ["Humidity"])
        Methane = (data_json ["Methane"])
        c02 = (data_json ["C02"])
    except:
        print("Data not in right format")
    
    DevicesQuery = "UPDATE Devices SET Temperature = %s, Humidity = %s, Methane = %s, C02 = %s WHERE Name = %s;"
    values = (Temperature, Humidity, Methane, c02, Name)
    cursor.execute(DevicesQuery, values)
    '''cursor.execute(UpdateQuery, values)
    HistoryQuery = "INSERT INTO Devices (Name, Temperature, Humidity, Methane, C02) VALUES (%s, %s, %s, %s, %s)"'''
    
    connection.commit()

client = mqtt.Client()

client.on_message = on_message
client.connect(broker, 1883)
client.subscribe(topic)
client.loop_forever()
