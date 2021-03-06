# Unity GOAP_S
## About
**Unity GOAP_S** is a Unity tool that provides an extremelly easy to use framework for the generation of goal oriented action planning agents.

[**Author: Ferran Martín Vilà**](https://www.linkedin.com/in/ferran-mart%C3%ADn-vil%C3%A0-9b1293165/)

## Guide
### Create an Agent
Add the "Agent_GS" script to the desired gameobject. A custom hierarchy icon will be displayed when a gameobject is defined as agent by adding the "Agent_GS" script. 

![AgentHierarhy](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/agent_hierarchy.PNG)

Now we can acces to the two main canvas throu the gameobject inspector, the "Node Editor" and the "Planning" canvas.

![AgenInspector](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/agent_inspector.PNG)

We will use this canvas to define and configure our agent behaviour.

### Zoomable Canvas
In order to generate a more comfortable workflow the two implemented canvas are zoomable, wich means that we can zoom in/out and move around the limited canvas space.

Zoom: Mouse wheel. with the mouse scroll.

Move: Mouse drag with wheel button pressed.

### Node Editor Canvas
In this canvas we define the agent action nodes and variables.

![NodeEditor](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/node_editor.PNG)

### Define Variables
Variables are defined inside the blackboards. Each agent has a private local blackboard apart of the gloabal balckboard that all agents can access. 

![Blackboard](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/blackboard.PNG)

To define a new variable click the "Add" button at the bottom of the desired blackboard and the variable definition menu will appear. Variables are basically defined by value type, name and value.

![VarSelect](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/var_select.PNG)
![VarDrop](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/var_select_drop.PNG)

#### Variables Binding
Variables can be binded to fields, properties and methods defined in a script that the agent can access. To bind a variable first we have to change the variable state to edit by clicking the left button of the variable editor. 

![VarBind](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/var_bind.PNG)

Now we can see two options, variable binding(V) and method binding(M).

![VarBindA](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/var_bind_a.PNG)

#### Property/Field binding
With variable binding we bind the variable to a field/property defined in a script inside the agent gameobject. In other words, when we get/set the blackboard variable value we get/set the value of the field/property binded to it.

#### Method Binding
With method binding we bind the variable get value to a method defined in a script inside the agent gameobject. Variables binded to a method can not be setted, because the variable value is the method definition. If we bind a variable to a method that needs input we can configure the input values by clicking the method button and selecting the desired input from the blackboards.

![MethodBind](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/method_bind.PNG)
![VarBindB](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/var_bind_b.PNG)

### Define Action nodes
Action nodes define the conditions and effects of executing an action. To add action nodes right click in the node editor canvas and select the option "Add Action Node". 

![AddNode](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/add_node.PNG)

The action node editor has a edit mode where you can change its name and description. 

![ActionNodeEdit](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/action_node_edit.PNG)

#### Conditions and Effects
To add effects/conditions to an action node pulse the "Add Effect" or "Add Condition" buttons of the action node editor.

![ActionNode](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/action_node.PNG)

This button will display a property selection where you can choose a condition/effect operation.

![PropertyDef](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/property_def.PNG)

#### Action Scripts
To define action scripts you simply have to create new scripts that inherit from the "Action_GS" script. The framework will recognize them and will be displayed in the action selection dropdown. 

![ActionIne](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/action_inherit.PNG)

The action node editor also provides a "Create New" action button that automatically generates a new script that inherits from "Action_GS".

![NewAction](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/new_action.PNG)

Once we set the action script we can configure it by clicking the "Configure Action" button in the action node editor.

![ActionConfOff](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/action_config_off.PNG)
![ActionConfOn](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/action_config_on.PNG)

### Planning Canvas
In this canvas we define the agent behaviour script and the idle action. During execution we can also see the agent debug output.

![PlanningCanvas](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/planning_canvas.PNG)

### Agent behaviour
Behaviour scripts define the agent goals by using blackboard variables. To define a new behaviour script you have to create a new script that inherits from the "AgentBehaviour_GS" script.

![BehaviourIne](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/behaviour_inherit.PNG)

In this case behaviour scripts have the same workflow as action scripts. The agent behaviour editor inside the planning canvas also has a "Create New" button that automatically generates a new script that inherits from "AgentBehaviour_GS".

![NewBehaviour](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/new_behaviour.PNG)

### Idle Action
Idle action is setted in the planning canvas. This action will be executed when the agent action plan is empty and there are not new goals that need to be reached.

### Customizable Debug
If our Unity project is beeing executed, the planning canvas displays the action plan of the selected agent. The "Action_GS" script has a public method named "BlitDebugUI" that can be overwritted. 

![DebugExample](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/debug_example.PNG)

In this method we can display all the desired information about our action and that will appear in the planning canvas if the action is contained in the current agent plan. The action plan debugger also shows the actions state by changing the debug windows color depending of it.

![Debug](https://github.com/ferranmartinvila/Unity-GOAP_S/blob/master/Screenshots/debug.PNG)


**To completely understand how this tool works internally, we recommend you to open the framework code and read the commentaries of the most important classes like Agent_GS, Action_GS and Behaviour_GS.**
