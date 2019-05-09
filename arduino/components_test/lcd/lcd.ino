/*
Arduino LDC test
*/

// include the library code:
#include <LiquidCrystal.h>

const int rs = 12, en = 11, d4 = 5, d5 = 4, d6 = 3, d7 = 2;
LiquidCrystal lcd(rs, en, d4, d5, d6, d7);

int line = 0;

void setup() {
	lcd.begin(16, 2);
}

void loop() {
	line++;
	if (line > 1) {
		line = 0;
	}
	int temp = millis() / 1000;
	ldcSetTextByLine(line, (String)temp);
	delay(1000);
}

void ldcSetTextByLine(int line, String content) {
	lcd.setCursor(0, line);
	lcd.print(content);
}