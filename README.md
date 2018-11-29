# HomeSecurity

This repository contains 
•	The home security API service by AWS Lambda Function as a finite state machine, and 
•	The Http REST interface as a simple web application for test the state machine
Both system are published on AWS:
•	AWS Lambda Function: https://gh21pxvwe5.execute-api.us-east-1.amazonaws.com/Prod/  
•	AWS Elastic Beanstalk web application: http://machinetest-dev.us-east-1.elasticbeanstalk.com/
I have used:
•	Code: Asp.net Core 2.1, C#, Bootstrap, Razor
•	Tools: Visual Studio 2017, Visual Studio Code, Postman, Git, Github, AWS site
The state machine is stored in the file JsonMachine.json and the class JsonMachine.cs
run it.


{
  ‘Name’: ‘HomeSecurityFiniteStateMachine’,
  ‘states’: [‘Disarmed’, ‘Armed’],
  ‘events’: [‘Arm’, ‘EnterCode’],
  ‘actions’: [‘CodeValidation’, ‘Http200’, ‘Log’, ‘Response’],
  ‘transitions’: 
  [
   {‘Name’: ‘TransitionOne’, ‘From’: ‘Disarmed’, ‘Event’: ‘Arm’, ‘GuardCodeValidation’: ‘False’,
    ‘To’: ‘Disarmed’, ‘EntryAction’: ‘‘, ‘ExitAction’: ‘Http200’, ‘InsideAction’: ‘Log’},
   {‘Name’: ‘TransitionTwo’, ‘From’: ‘Disarmed’, ‘Event’: ‘EnterCode’, ‘GuardCodeValidation’: ‘True’, 
    ‘To’:  ‘Armed’, ‘EntryAction’: ‘CodeValidation’, ‘ExitAction’: ‘Response’, ‘InsideAction’: ‘Log’},
    {‘Name’: ‘TransitionThree’, ‘From’: ‘Armed’, ‘Event’: ‘Disarm’, ‘GuardCodeValidation’: ‘False’, 
    ‘To’: ‘Disarmed’, ‘EntryAction’: ‘‘, ‘ExitAction’: ‘Http200’, ‘InsideAction’: ‘Log’},
    {‘Name’: ‘TransitionFour’, ‘From’: ‘Armed’, ‘Event’: ‘EnterCode’, ‘GuardCodeValidation’: ‘True’,
     ‘To’: ‘Disarmed’, ‘EntryAction’: ‘CodeValidation’, ‘ExitAction’: ‘Response’, ‘InsideAction’: ‘Log’}
  ],
  ‘InitialState’: ‘Disarmed’
}

Metadata
states
events
actions
transitions	From
To
Event	EntryAction
InsideAction
ExitAction
	GuardCodeCondition

Data
states	events	actions	transisitions
Disarmed
Armed	Arm
Disarm
EnterCode	Log
CodeValidation
Response
Http200	Arm, Disarmed
EnterCode, Disarmed
Disarm, Armed
EnterCode, Armed

The AWS Lambda Function accepts a JSON in the message body with the keys:
•	Connect (True)
•	Event (events member)
•	State (states member)
•	Code
In addition, it accepts an extra key, in the development version:
•	Reset (True)
When Reset is True, it causes the CurrentState and Code are initialized by the defaults in the code.

To change the State, the Code must be valid
Three method have been implemented to save the CurrentState and Code
•	AWS Parameter Storing Service
•	File
•	Lambda Function Environment Variable
The final plan is using of the first one along TLS (or SSL)


•	'Reset' has the first priority to accept and run, then 'Connect' is at the second priority.

The message can be as the following:
•	{ 'Reset': 'False', 'Connect': 'False', 'Event':'EnterCode' , 'State':'Disarmed', 'Code':'1111' }
•	{ 'Reset':'True' }  => reset
•	{ 'Connect':'True' }  => just to get state of the machin
•	{ 'Event':'Arm' , 'State':'Disarmed' , 'Code':'1111' } => ready to accept the code (next command) 
•	{ 'Event':'EnterCode' , 'State':'Disarmed' , 'Code':'1111' }  => if Code is valid the state will be armed
•	{ 'Event':'Disarm' , 'State':'Armed' , 'Code':'1111' }  => if Code is valid the state will be armed
•	{ 'Event':'EnterCode' , 'State':'Armed' , 'Code':'1111' }  => if Code is valid the state will be armed
•	….


Api Response
•	State
•	Response
