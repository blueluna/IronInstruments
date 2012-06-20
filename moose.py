import clr

class Mammal(object):
	
	def __init__(self):
		self._sound = "silence..."
	
	def sound(self):
		return self._sound

class Moose(Mammal):
	def __init__(self):
		self._sound = "mooose!"

moose = Moose()
