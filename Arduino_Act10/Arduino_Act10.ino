#include <Servo.h>

const int sensorPin = A0;   // Pin donde está conectado el sensor LM35
const int servoPin = 9;     // Pin donde está conectado el servomotor

Servo myServo;              // Objeto para controlar el servomotor

void setup() {
  Serial.begin(9600);       
  myServo.attach(servoPin); 
  myServo.write(90);       
}

void loop() {
  // Leer el valor del sensor LM35
  int sensorValue = analogRead(sensorPin);
  float voltage = sensorValue * (5.0 / 1023.0); // Convierte el valor a voltaje
  float temperatureC = voltage * 100;           // LM35 da 10mV por cada grado Celsius
  float temperatureF = (temperatureC * 9.0 / 5.0) + 32.0; // Convertir a Fahrenheit

  
  Serial.println(temperatureF);

  
  if (Serial.available() > 0) {
    char option = Serial.read();
    if (option == 'e') {
      myServo.write(90); // Establece el ángulo del servomotor a 90 grados (Encender)
      Serial.println("Encendido"); 
    }
    else if (option == 'f') {
      myServo.write(0);  // Establece el ángulo del servomotor a 0 grados (Apagar)
      Serial.println("Apagado"); 
    }
  }

  delay(1000); 
}
//