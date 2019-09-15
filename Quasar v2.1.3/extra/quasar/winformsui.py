# dynamo version - [1.x.x , 2.0.x]
# author - min.naung

# quasar python winformsui module for Window Forms UI class

import clr,sys
sys.path.append(r"C:\Program Files (x86)\IronPython 2.7\Lib")
import os, System

clr.AddReference("RevitAPI")
clr.AddReference("RevitServices")
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager



version = ["2.0","1.3","1.2","1.1","1.0","0.9","0.8","0.7"]

appdata = os.getenv("AppData");
qpath = r"\Dynamo\Dynamo Revit\%s\packages\Quasar"

for i in version:
	if os.path.exists(os.path.join(appdata+qpath %i)):
		ipath = os.path.join(appdata+qpath %i)

clr.AddReference("System.Windows.Forms")
clr.AddReference("System.Drawing")


from System.Collections.Generic import List
from System.Windows.Forms import (Application, Button, ComboBox, CheckBox, Form as _Form,
CheckedListBox,Padding,FormBorderStyle, SelectionMode,Label, MessageBox,MessageBoxButtons, MessageBoxIcon)
from System.Drawing import (Font as _Font, Icon as _Icon, Point as _Point, Size as _Size,FontStyle)


class CheckedBoxListSelector(_Form):
	"""Checked Box List Selector"""
	

	def __init__(self,inputList):
		self.Icon = _Icon(r"%s\extra\icon.ico" %ipath)
		self.Size = _Size(290,435)
		self.Text = "Quasar"
		self.class1Result = [];

		# fonts
		self.gsmt_11r = _Font("Georgia", 10)
		self.gsmt_10r = _Font("Georgia",9)

		# label
		self.label = Label()
		self.label.Text = "Select Items:"
		self.label.Font = self.gsmt_11r
		self.label.Location = _Point(30,10)
		self.label.Size = _Size(220,28)
		self.Controls.Add(self.label)

		# checklist box
		self.lstbox = CheckedListBox()
		self.lstbox.Items.AddRange(tuple(inputList))
		self.lstbox.Font = self.gsmt_10r
		# padding
		self.lstbox.Padding = Padding(30)
		# select one click
		self.lstbox.CheckOnClick = True

		self.FormBorderStyle = FormBorderStyle.FixedDialog
		self.MaximizeBox = False;
		self.ManimizeBox = False;
		self.lstbox.Location = _Point(30,40)
		self.lstbox.Size = _Size(210,280)

		# auto horizontal scrollbar
		self.lstbox.HorizontalScrollbar = True
		self.Controls.Add(self.lstbox)

		# select all btn
		self.btn_all = Button()
		self.btn_all.Text = "Select All"
		self.btn_all.Font = self.gsmt_10r
		self.btn_all.Size = _Size(95,25)
		self.btn_all.Location = _Point(30,315)
		self.Controls.Add(self.btn_all)

		# select none btn
		self.btn_none = Button()
		self.btn_none.Text = "Select None"
		self.btn_none.Font = self.gsmt_10r
		self.btn_none.Size = _Size(95,25)
		self.btn_none.Location = _Point(145,315)
		self.Controls.Add(self.btn_none)

		# ok btn
		self.btn = Button()
		self.btn.Text = "OK"
		self.btn.Font = self.gsmt_10r
		self.btn.Size = _Size(80,25)
		self.btn.Location = _Point(95,350)
		self.Controls.Add(self.btn)

		# events
		self.btn_all.Click += self.click_all
		self.btn_none.Click += self.click_none
		self.btn.Click += self.click_ok 

		# pop up from center
		self.CenterToScreen()

	def click_ok(self, sender, event):
		#global res
		if len(self.lstbox.Items) < 1:
			self.Close()
		if len(self.lstbox.CheckedItems) < 1:
			return MessageBox.Show("No element selected!", "Quasar Info Box", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
			
		self.class1Result = self.lstbox.CheckedItems;
		
		return self.Close();			

	def click_all(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,True)

	def click_none(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,False);

class ComboCheckboxListSelector(_Form):
	"""Checked Box List Selector"""

	def __init__(self,title,items):
		
		# icon
		self.Icon = _Icon(r"%s\extra\icon.ico" %ipath)
		self.Size = _Size(360,430)
		self.Text = "Quasar"
		self.class2Result = [];
		
		self.title = title
		self.items = items
		# fonts
		self.gsmt_11r = _Font("Georgia", 10)
		self.gsmt_10r = _Font("Georgia",9)

		# combo box
		self.cbox = ComboBox()
		self.cbox.Font = self.gsmt_11r
		self.cbox.Location = _Point(30,10)
		self.cbox.Size = _Size(280,28)
		self.cbox.Items.AddRange(tuple(title))
		self.cbox.SelectedIndex = 0;
		self.Controls.Add(self.cbox)
		
		
		
		# checklist box
		self.lstbox = CheckedListBox()
		self.lstbox.Items.AddRange(tuple(self.items[self.cbox.SelectedIndex]))
		self.lstbox.Font = self.gsmt_10r
		# padding
		self.lstbox.Padding = Padding(30)
		# select one click
		self.lstbox.CheckOnClick = True

		self.FormBorderStyle = FormBorderStyle.FixedDialog
		self.MaximizeBox = False;
		self.ManimizeBox = False;
		self.lstbox.Location = _Point(30,45)
		self.lstbox.Size = _Size(280,300)

		# auto horizontal scrollbar
		self.lstbox.HorizontalScrollbar = True
		self.Controls.Add(self.lstbox)

		# select all btn
		self.btn_all = Button()
		self.btn_all.Text = "Select All"
		self.btn_all.Font = self.gsmt_10r
		self.btn_all.Size = _Size(95,25)
		self.btn_all.Location = _Point(30,340)
		self.Controls.Add(self.btn_all)

		# select none btn
		self.btn_none = Button()
		self.btn_none.Text = "Select None"
		self.btn_none.Font = self.gsmt_10r
		self.btn_none.Size = _Size(95,25)
		self.btn_none.Location = _Point(215,340)
		self.Controls.Add(self.btn_none)

		# ok btn
		self.btn = Button()
		self.btn.Text = "OK"
		self.btn.Font = self.gsmt_10r
		self.btn.Size = _Size(80,25)
		self.btn.Location = _Point(130,340)
		self.Controls.Add(self.btn)

		# events
		self.btn_all.Click += self.click_all
		self.btn_none.Click += self.click_none
		self.btn.Click += self.click_ok 
		self.cbox.SelectedValueChanged += self.on_class_change

		# pop up from center
		self.CenterToScreen()

	def click_ok(self, sender, event):	
	
		if len(self.lstbox.CheckedItems) < 1:
			return MessageBox.Show("No element selected!", "Quasar Info Box", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
						
		self.class2Result = self.lstbox.CheckedItems
		
		return self.Close()		

	def click_all(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,True)

	def click_none(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,False)
	
	def on_class_change(self, sender, event):
		self.lstbox.Items.Clear();
		self.lstbox.Items.AddRange(tuple(self.items[self.cbox.SelectedIndex]))