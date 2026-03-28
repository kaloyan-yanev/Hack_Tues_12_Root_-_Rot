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

topic = "machines/control/" + device + "/temp_tresh/"
broker = "localhost"

client = mqtt.Client()
client.connect(broker, 1883)
client.loop_start()
result = client.publish(topic, value)
result.wait_for_publish()
client.loop_stop()
client.disconnect()

