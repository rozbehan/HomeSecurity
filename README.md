This repository contains

- The home security API service by AWS Lambda Function as a finite state machine, and
- The Http REST interface as a simple web application for test the state machine

Both system are published on AWS:

- AWS Lambda Function: [https://gh21pxvwe5.execute-api.us-east-1.amazonaws.com/Prod/](https://gh21pxvwe5.execute-api.us-east-1.amazonaws.com/Prod/)
- AWS Elastic Beanstalk web application: [http://machinetest-dev.us-east-1.elasticbeanstalk.com/](http://machinetest-dev.us-east-1.elasticbeanstalk.com/)

I have used:

- Code: Asp.net Core 2.1, C#, Bootstrap, Razor
- Tools: Visual Studio 2017, Visual Studio Code, Postman, Git, Github, AWS site

| The state machine is stored in the file JsonMachine.json and the class JsonMachine.csrun it.  {  &#39;Name&#39;: &#39;HomeSecurityFiniteStateMachine&#39;,  &#39;states&#39;: [&#39;Disarmed&#39;, &#39;Armed&#39;],  &#39;events&#39;: [&#39;Arm&#39;, &#39;EnterCode&#39;],  &#39;actions&#39;: [&#39;CodeValidation&#39;, &#39;Http200&#39;, &#39;Log&#39;, &#39;Response&#39;],  &#39;transitions&#39;:   [   {&#39;Name&#39;: &#39;TransitionOne&#39;, &#39;From&#39;: &#39;Disarmed&#39;, &#39;Event&#39;: &#39;Arm&#39;, &#39;GuardCodeValidation&#39;: &#39;False&#39;,    &#39;To&#39;: &#39;Disarmed&#39;, &#39;EntryAction&#39;: &#39;&#39;, &#39;ExitAction&#39;: &#39;Http200&#39;, &#39;InsideAction&#39;: &#39;Log&#39;},   {&#39;Name&#39;: &#39;TransitionTwo&#39;, &#39;From&#39;: &#39;Disarmed&#39;, &#39;Event&#39;: &#39;EnterCode&#39;, &#39;GuardCodeValidation&#39;: &#39;True&#39;,     &#39;To&#39;:  &#39;Armed&#39;, &#39;EntryAction&#39;: &#39;CodeValidation&#39;, &#39;ExitAction&#39;: &#39;Response&#39;, &#39;InsideAction&#39;: &#39;Log&#39;},    {&#39;Name&#39;: &#39;TransitionThree&#39;, &#39;From&#39;: &#39;Armed&#39;, &#39;Event&#39;: &#39;Disarm&#39;, &#39;GuardCodeValidation&#39;: &#39;False&#39;,     &#39;To&#39;: &#39;Disarmed&#39;, &#39;EntryAction&#39;: &#39;&#39;, &#39;ExitAction&#39;: &#39;Http200&#39;, &#39;InsideAction&#39;: &#39;Log&#39;},    {&#39;Name&#39;: &#39;TransitionFour&#39;, &#39;From&#39;: &#39;Armed&#39;, &#39;Event&#39;: &#39;EnterCode&#39;, &#39;GuardCodeValidation&#39;: &#39;True&#39;,     &#39;To&#39;: &#39;Disarmed&#39;, &#39;EntryAction&#39;: &#39;CodeValidation&#39;, &#39;ExitAction&#39;: &#39;Response&#39;, &#39;InsideAction&#39;: &#39;Log&#39;} ],  &#39;InitialState&#39;: &#39;Disarmed&#39;} |
| --- |

Metadata

| states, events, actions, transitions, | From, To, Event, | EntryAction, InsideAction, ExitAction,   | GuardCodeCondition, |
| --- | --- | --- | --- |

Data

| States, | Events, | Actions, | Transisitions, |
| --- | --- | --- | --- |
| Disarmed, Armed, | Arm, Disarm, EnterCode, | Log, CodeValidation, Response, Http200, | Arm, Disarmed, EnterCode, Disarmed, Disarm, Armed, EnterCode, Armed, |

The AWS Lambda Function accepts a JSON in the message body with the keys:

- Connect (True)
- Event (events member)
- State (states member)
- Code

In addition, it accepts an extra key, in the development version:

- Reset (True)

When Reset is True, it causes the CurrentState and Code are initialized by the defaults in the code.

To change the State, the Code must be valid

Three method have been implemented to save the CurrentState and Code

- AWS Parameter Storing Service
- File
- Lambda Function Environment Variable

The final plan is using of the first one along TLS (or SSL)



- &#39;Reset&#39; has the first priority to accept and run, then &#39;Connect&#39; is at the second priority.

The message can be as the following:

- { &#39;Reset&#39;: &#39;False&#39;, &#39;Connect&#39;: &#39;False&#39;, &#39;Event&#39;:&#39;EnterCode&#39; , &#39;State&#39;:&#39;Disarmed&#39;, &#39;Code&#39;:&#39;1111&#39; }
- { &#39;Reset&#39;:&#39;True&#39; }  =\&gt; reset
- { &#39;Connect&#39;:&#39;True&#39; }  =\&gt; just to get state of the machin
- { &#39;Event&#39;:&#39;Arm&#39; , &#39;State&#39;:&#39;Disarmed&#39; , &#39;Code&#39;:&#39;1111&#39; } =\&gt; ready to accept the code (next command)
- { &#39;Event&#39;:&#39;EnterCode&#39; , &#39;State&#39;:&#39;Disarmed&#39; , &#39;Code&#39;:&#39;1111&#39; }  =\&gt; if Code is valid the state will be armed
- { &#39;Event&#39;:&#39;Disarm&#39; , &#39;State&#39;:&#39;Armed&#39; , &#39;Code&#39;:&#39;1111&#39; }  =\&gt; if Code is valid the state will be armed
- { &#39;Event&#39;:&#39;EnterCode&#39; , &#39;State&#39;:&#39;Armed&#39; , &#39;Code&#39;:&#39;1111&#39; }  =\&gt; if Code is valid the state will be armed
- ….

And the Api response has two parameters:

- State
- Response
