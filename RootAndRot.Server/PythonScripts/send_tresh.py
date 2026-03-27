import sys
import paho.mqtt.client as mqtt
import argparse

parser = argparse.ArgumentParser(
        prog='Send_Tresh',
        description='Sends treshold update to esp')
parser.add_argument('device')
parser.add_argument('v', type=int)
args = parser.parse_args()

device = args.device
value = args.v

topic = "machines/control/temp_tresh/" + device
broker = "localhost"

client = mqtt.Client()
client.connect(broker, 1883)
client.publish(topic, value)

