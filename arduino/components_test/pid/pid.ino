/**
 * Thermal Sensor Test
 * 
 * Source : https://create.arduino.cc/projecthub/TheGadgetBoy/ds18b20-digital-temperature-sensor-and-arduino-9cc806
 */

/********************************************************************/
// First we include the libraries
#include <OneWire.h> 
#include <DallasTemperature.h>
/********************************************************************/
// Data wire is plugged into pin 2 on the Arduino 
#define ONE_WIRE_BUS 2 
/********************************************************************/
// Setup a oneWire instance to communicate with any OneWire devices  
// (not just Maxim/Dallas temperature ICs) 
OneWire oneWire(ONE_WIRE_BUS);
/********************************************************************/
// Pass our oneWire reference to Dallas Temperature. 
DallasTemperature sensors(&oneWire);
/********************************************************************/

double todo_temp = 28 ;
double ambiant_temp = 25;
double actual_temp;

int activity = 0;
int loop_occurence = 10;
int loop_numberOccurence = 10;
int loop_onSameTemp = 0;
double loop_lastTemp;

int trans = 8;

void setup(void)
{
  Serial.begin(9600);
  pinMode(trans, OUTPUT);
  sensors.begin();
  digitalWrite(trans, LOW);
}
void loop(void)
{
  proportional_control();
}

// V1 Propotional Control Algo
void proportional_control(){
  double diff = todo_temp - ambiant_temp;
  if (loop_numberOccurence >= loop_occurence){
    loop_numberOccurence = 0;
    
    sensors.requestTemperatures(); // Send the command to get temperature readings 
    actual_temp = sensors.getTempCByIndex(0);
    //Serial.println(activity);
    
    Serial.print(actual_temp);
    Serial.print(", ");
    Serial.print(activity);
    Serial.print("\n");
    if(todo_temp < actual_temp){
      activity = 0;
      return;
    }
    double actual_diff = todo_temp - actual_temp;
    if(actual_diff > (20*diff/100)){
      activity = 1000;
    }
    else{
      int temp_activity = (int)(actual_diff * 100 / (20*diff/100));

      if(actual_temp == loop_lastTemp){
        loop_onSameTemp ++;
        if(loop_onSameTemp > 2){
          temp_activity -= 2;
        }
      } else {
        loop_onSameTemp = 0;
      }
      
      if(temp_activity < 0){
        activity = 0;
      }
      else {
        activity = (int)temp_activity*10+5;
      }
    }
    loop_lastTemp = actual_temp;
    return;
  }
  
  if(loop_numberOccurence <= activity){
    digitalWrite(trans, HIGH);
  }
  else{
     digitalWrite(trans, LOW);
  }
  loop_numberOccurence++;
}

