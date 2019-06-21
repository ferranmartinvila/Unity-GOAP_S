# Unity GOAP_S
## About
Unity GOAP_S is a Unity tool that provides an extremelly easy to use framework for the generation of goal oriented action planning agents.

Author: Ferran Martín Vilà

## Guide
### Create an Agent
Add the "Agent_GS" script to the desired gameobject. A custom hierarchy icon will be displayed when a gameobject is defined as agent by adding the "Agent_GS" script. 

Now we can acces to the two main canvas throu the gameobject inspector, the "Node Editor" and the "Planning" canvas.

We will use this canvas to define and configure our agent behaviour.

### Zoomable Canvas
In order to generate a more comfortable workflow the two implemented canvas are zoomable, wich means that we can zoom in/out and move around the limited canvas space.

Zoom: Mouse wheel. with the mouse scroll.
Move: Mouse drag with wheel button pressed.

### Node Editor Canvas
In this canvas we define the agent action nodes and variables.

### Define Variables
Variables are defined inside the blackboards. Each agent has a private local blackboard apart of the gloabal balckboard that all agents can access. To define a new variable click the "Add" button at the bottom of the desired blackboard and the variable definition menu will appear. Variables are basically defined by value type, name and value.

#### Variables Binding
Variables can be binded to fields properties and methods defined in a script that the agent can access. To bind a variable first we have to change the variable state to edit by clicking the left button of the variable editor. Now we can see two options, variable binding(V) and method binding(M). 
#### Property/Field binding
With variable binding we bind the variable to a field/property defined in a script inside the agent gameobject. In other words, when we get/set the blackboard variable value we get/set the value of the field/property binded to it.

#### Method Binding
With method binding we bind the variable get value to a method defined in a script inside the agent gameobject. Variables binded to a method can not be setted, because the variable value is the method definition. If we bind a variable to a method that needs input we can configure the input values by clicking the method button and selecting the desired input from the blackboards.

### Define Action nodes
Action nodes define the conditions and effects of executing an action. To add action nodes right click in the node editor canvas and select the option "Add Action Node". The action node editor has a edit mode where you can change its name and description. 

### Conditions and Effects
To add effects/conditions to an action node pulse the "Add Effect" or "Add Condition" buttons of the action node editor. This button will display a property selection where you can choose a condition/effect operation.

### Action Scripts
To define action scripts you simply have to create new scripts that inherit from the "Action_GS" script. The framework will recognize them and will be displayed in the action selection dropdown. The action node editor also provides a "Create New" action button that automatically generates a new script that inherits from "Action_GS".

Once we set the action script we can configure it by clicking the "Configure Action" button in the action node editor.

### Planning Canvas
In this canvas we define the agent behaviour script and the idle action. During execution we can also see the agent debug output.

### Agent behaviour
Behaviour scripts define the agent goals by using blackboard variables. To define a new behaviour script you have to create a new script that inherits from the "AgentBehaviour_GS" script. In this case behaviour scripts have the same workflow as action scripts. The agent behaviour editor inside the planning canvas also has a "Create New" button that automatically generates a new script that inherits from "AgentBehaviour_GS".

### Idle Action
Idle action is setted in the planning canvas. This action will be executed when the agent action plan is empty and there are not new goals that need to be reached.

### Customizable Debug
If our Unity project is beeing executed, the planning canvas displays the action plan of the selected agent. The "Action_GS" script has a public method named "XX" that can be overwritted. In this method we can display all the desired information about our action and that will appear in the planning canvas if the action is contained in the current agent plan. The action plan debugger also shows the actions state by changing the debug windows color depending of it.
