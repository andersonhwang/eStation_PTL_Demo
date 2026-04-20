# eStation PTL Edition
Welcome to eStation PTL edition! 

eStation PTL edition is designed for developers to quick integerate ETAG PTL with their projects. eStation use MQTT protocol and esay to configure/integerate.

Release Date: ----

Firmware: ---

# 1. Work with Mosquitto

If you are working with Mosquitton, you need edit the Mosquitto configure file mosquitto.conf:
```
allow_anonymous false  
password_file Your_Password_File_Path_Here  # Pasword file path
listener XXXX                               # The MQTT port
max_topic_alias 255                         # The default value is 10, need change to 255
