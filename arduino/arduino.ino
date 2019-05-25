/**
   Bastien Crettenand

   Projet pratique - Module 121
   Source : https://github.com/CrBast/distillation_column/

   Librairies :
    - LiquidCrystal
    - OneWire
    - DallasTemperature
*/

#include <LiquidCrystal.h>
#include <OneWire.h>
#include <DallasTemperature.h>

const int rs = 6, en = 7, d4 = 5, d5 = 4, d6 = 3, d7 = 2;
LiquidCrystal lcd(rs, en, d4, d5, d6, d7);

#define ONE_WIRE_BUS 11
OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature sensors(&oneWire);

// Variables générales
int state = 0;
int ledOnOff = 9;
int ledBusy = 8;
int btnOnOff = 13;
bool btnOnOff_lastState = false;
int btnStartWork = 12;
bool btnStartWork_lastState = true;

// Variables nécessaire pour la régulation proportionnelle 
int activity = 0;
int loop_occurence = 1000;
int loop_numberOccurence = 1000;
int loop_onSameTemp = 0;
int loop_startingProportion;
double loop_lastTemp;
// Spécifique température
double todo_temp = 28;
double ambiant_temp = 25;
double actual_temp;

int trans = 10;

long testInt = 0;

void setup(void)
{
  lcd.begin(16, 2);
  Serial.begin(9600);
  pinMode(trans, OUTPUT);
  pinMode(ledBusy, OUTPUT);
  pinMode(ledOnOff, OUTPUT);
  pinMode(btnOnOff, INPUT);
  pinMode(btnStartWork, INPUT);

  digitalWrite(trans, LOW);
  digitalWrite(ledBusy, LOW);
  digitalWrite(ledOnOff, LOW);

  sensors.begin();

  sensors.requestTemperatures(); // Send the command to get temperature readings
  ambiant_temp = 25; //sensors.getTempCByIndex(0);
  Serial.println("-s " + (String)state);
  bool btnOnOff_lastState = false;
}
void loop(void)
{
  switch (state)
  {
    case 0:
      if (digitalRead(btnOnOff) == HIGH && btnOnOff_lastState != true)
      {
        btnOnOff_lastState = true;
        state = 1;
        Serial.println("-s " + (String)state);
        digitalWrite(ledOnOff, HIGH);
      }
      if(digitalRead(btnOnOff) == LOW){
        btnOnOff_lastState = false;
      }
      break;
    case 1:
      sensors.requestTemperatures(); // Send the command to get temperature readings
      actual_temp = sensors.getTempCByIndex(0);

      Serial.println("-i " + (String)actual_temp);
      ldcSetTextByLine(0, "\t" + (String)actual_temp + "\tC");
      ldcSetTextByLine(1, "\t" + (String)state);

      if (digitalRead(btnStartWork) == HIGH)
      {
        state = 2;
        Serial.println("-s " + (String)state);
      }
      break;
    case 2:
      if(proportional_control()){ // Wait the sensor go to <todo_temp>
        state = 3;
        Serial.println("-s " + (String)state);
      }
      break;
    case 3:
      digitalWrite(ledBusy, HIGH);
      
      if(testInt <= 76433){ // 76433 ~= 60 seconds
        testInt++;
        proportional_control();
      } else {
        digitalWrite(trans, LOW);
        digitalWrite(ledBusy, LOW);
        state = 1;
        Serial.println("-s " + (String)state);
        Serial.println("-p " + (String)activity);
        testInt = 0;
      }
      break;
  }
  // Evènement lors du clique bouton OFF
  if (digitalRead(btnOnOff) == HIGH && btnOnOff_lastState == false)
    {
      //Remise état "éteint"
      state = 0;
      Serial.println("-s " + (String)state);
      digitalWrite(ledOnOff, LOW);
      digitalWrite(ledBusy, LOW);
      btnOnOff_lastState = true;
      ldcSetTextByLine(0, "");
      ldcSetTextByLine(1, "");
      digitalWrite(trans, LOW);
    }
    if(digitalRead(btnOnOff) == LOW){
      btnOnOff_lastState = false;
    }
}

// V1.2 Propotional Control Algo
// One full loop = 0.785 seconds <-> 785 miliseconds
// 
// Méthode de régulation proportionnelle - Non bloquante
bool proportional_control()
{
  double diff = todo_temp - ambiant_temp;
  if (loop_numberOccurence >= loop_occurence)
  {
    if (ambiant_temp <= 25)
    {
      loop_startingProportion = (int)95 + ((25 - ambiant_temp) * 2);
    }
    else
    {
      loop_startingProportion = (int)95 - ((ambiant_temp - 25) * 2);
    }
    loop_numberOccurence = 0;

    sensors.requestTemperatures(); // Send the command to get temperature readings
    actual_temp = sensors.getTempCByIndex(0);


    /*Serial.print(", ");
      Serial.println(activity);*/
    ldcSetTextByLine(0, "\t" + (String)actual_temp + "\tC");
    ldcSetTextByLine(1, "\t" + (String)state + "\t" + (String)activity);

    Serial.println("-i " + (String)actual_temp);
    Serial.println("-s " + (String)state);
    Serial.println("-p " + (String)activity);

    int diffBtw25AndActualTemp = 25 - ambiant_temp;
    if (diffBtw25AndActualTemp <= 0) {
      diffBtw25AndActualTemp = 1;
    }

    if (actual_temp >= todo_temp)
    {
      if ((double)(actual_temp - todo_temp) >= (double)(diff * 3 / 100))
      {
        activity = 0;
      }
      else
      {
        if (actual_temp == todo_temp)
        {
          activity = 100 * diffBtw25AndActualTemp;
        }
        else
        {
          activity = 50 * diffBtw25AndActualTemp;
        }
      }
    }
    else
    {
      double actual_diff = todo_temp - actual_temp;
      if (actual_diff > ((100 - loop_startingProportion) * diff / 100))
      {
        activity = 1000;
      }
      else
      {
        int temp_activity = (int)(actual_diff * 100 / ((100 - loop_startingProportion) * diff / 100));

        if (actual_temp == loop_lastTemp && actual_temp <= todo_temp)
        {
          loop_onSameTemp++;
          if (loop_onSameTemp > 1)
          {
            temp_activity += 0.5 * diffBtw25AndActualTemp;
          }
          if (loop_onSameTemp > 2)
          {
            temp_activity += 1 * diffBtw25AndActualTemp;
          }
        }
        else
        {
          loop_onSameTemp = 0;
        }

        if (temp_activity < 0)
        {
          activity = 0;
        }
        else
        {
          if (temp_activity <= 35)
          {
            activity = (int)temp_activity * 10 - 100;
          }
          else
          {
            activity = (int)temp_activity * 10;
          }
        }
      }
      loop_lastTemp = actual_temp;
    }

    if (actual_temp >= todo_temp)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  if (loop_numberOccurence < activity)
  {
    digitalWrite(trans, HIGH);
  }
  else
  {
    digitalWrite(trans, LOW);
  }
  loop_numberOccurence++;
  return false;
}

void ldcSetTextByLine(int line, String content)
{
  lcd.setCursor(0, line);
  lcd.print("              ");
  lcd.setCursor(0, line);
  lcd.print(content);
}
