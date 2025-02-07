Entities

Fighter 
- Element 
- Attack points
- defense points
- health points

Attack
- Damage points
- Element

fire pal
- attack points = 10
- attack
	- dp 5
	- fire

reciever
- grass type
- defense points = 10
- hp = 100

formula:
damaged recieved = reciever.hp -  (attacker.attack.ap * typeeffectiveness * attacker.ap) / (reciever.defensePoints)

types enum 
- Fire 		grass *2, Ice  *2, water *0.5	
- Grass		fire *0.5, ground *2
- Ground		electric * 2, grass * 0.5
- Electric	ground * 0.5, water *2
- Water		fire *2, electric *0.5
- Ice			Fire *0.5, dragon *2
- Dragon		ice *0.5, dark *2
- Dark		dragon *0.5, neutral *2
- Neutral		dark *0.5

- if none above, damage  = * 1


