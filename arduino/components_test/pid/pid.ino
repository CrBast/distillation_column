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
int loop_occurence = 1000;
int loop_numberOccurence = 1000;
int loop_onSameTemp = 0;
double loop_lastTemp;

int trans = 8;

long testInt = 0;

void setup(void)
{
  Serial.begin(9600);
  pinMode(trans, OUTPUT);
  sensors.begin();
  digitalWrite(trans, LOW);
}
void loop(void)
{
  //sensors.requestTemperatures(); // Send the command to get temperature readings 
  ambiant_temp = 25; //sensors.getTempCByIndex(0);
  do{
    
  }while(!proportional_control());// Wait the sensor go to <todo_temp>
  Serial.println("Start");
  do{
    testInt++;
    proportional_control();
  }while(testInt <= 76433); // 76433 ~= 60 seconds
  Serial.println("Stop");
  testInt=0;
}

// V1.2 Propotional Control Algo
// One full loop = 0.785 seconds <-> 785 miliseconds
bool proportional_control(){
  double diff = todo_temp - ambiant_temp;
  if (loop_numberOccurence >= loop_occurence){
    loop_numberOccurence = 0;
    
    sensors.requestTemperatures(); // Send the command to get temperature readings 
    actual_temp = sensors.getTempCByIndex(0);
    
    
    Serial.println(actual_temp);
    Serial.println(activity);
  
    if(actual_temp >= todo_temp){
      if((double)(actual_temp - todo_temp) >= (double)(diff*3/100)){
        activity = 0;
      } else {
        if(actual_temp == todo_temp){
          activity = 100;
        } else{
          activity = 50;
        }
      }
    }else{
      double actual_diff = todo_temp - actual_temp;
      if(actual_diff > (20*diff/100)){
        activity = 1000;
      }
      else{
        int temp_activity = (int)(actual_diff * 100 / (20*diff/100));
  
        if(actual_temp == loop_lastTemp && actual_temp <= todo_temp){
          loop_onSameTemp ++;
          if(loop_onSameTemp > 1){
            temp_activity += 0.5;
          }
          if(loop_onSameTemp > 2){
            temp_activity += 1;
          }
        } else {
          loop_onSameTemp = 0;
        }
        
        if(temp_activity < 0){
          activity = 0;
        }
        else {
          if(temp_activity <= 35){
            activity = (int)temp_activity*100-100;
          }
          else{
            activity = (int)temp_activity*100;
          }
        }
      }
      loop_lastTemp = actual_temp;
    }
    
    if(actual_temp >= todo_temp) {
      return true;
    }
    else{
      return false;
    }
  }
  
  if(loop_numberOccurence <= activity){
    digitalWrite(trans, HIGH);
  }
  else{
    digitalWrite(trans, LOW);
  }
  loop_numberOccurence++;
  return false;
}