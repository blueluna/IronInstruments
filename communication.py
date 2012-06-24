# -*- coding: utf-8 -*-

import clr
clr.AddReference('Communication')
import Communication

class Wrapper(object):
	def __init__(self):
		self.handler = Communication.CommunicationHandler()
		self.handler.ConnectionStateChanged += self.handleConnection
		self.handler.DataReceived += self.handleData
		self.handler.ErrorReceived += self.handleError
	
	def handleConnection(self, sender, event):
		print("State: {0}: {1}".format(event.PortId, event.State))
	
	def handleData(self, sender, event):
		print("Data: {0}: ...".format(event.PortId))
	
	def handleError(self, sender, event):
		print("Error: {0}: {1}".format(event.PortId, event.ErrorMessage))

	def AddSerial(self, port, baudrate):
		self.handler.AddPort(port, Communication.PortType.Serial, port, baudrate, Communication.PortParser.None)

com = Wrapper()
