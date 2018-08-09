# dynamo version - 1.3
# author - min.naung

import clr,sys
sys.path.append(r"C:\Program Files (x86)\IronPython 2.7\Lib")
import os, System

version = ["2.0","1.3","1.2","1.1","1.0","0.9","0.8","0.7"]

appdata = os.getenv("AppData");
qpath = r"\Dynamo\Dynamo Revit\%s\packages\Quasar"

for i in version:
	if os.path.exists(os.path.join(appdata+qpath %i)):
		ipath = os.path.join(appdata+qpath %i)

clr.AddReference("System.Windows.Forms")
clr.AddReference("System.Drawing")
clr.AddReference("RevitAPI")
clr.AddReference("RevitServices")

from System.Collections.Generic import List
from System.Windows.Forms import (Application, Button, ComboBox, CheckBox, Form as _Form,
CheckedListBox,Padding,FormBorderStyle, SelectionMode,Label, MessageBox,MessageBoxButtons, MessageBoxIcon)
from System.Drawing import (Font as _Font, Icon as _Icon, Point as _Point, Size as _Size,FontStyle)


from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

doc = DocumentManager.Instance.CurrentDBDocument

class LinkSelection(_Form):
	"""Checked Box List Selector"""

	def __init__(self):

		self.linkInstances = FilteredElementCollector(doc).OfClass(RevitLinkInstance).ToElements();
		self.linkName = [i.Name.split(" : ")[0] for i in self.linkInstances];
		self.linkDict = {i.Name.split(" : ")[0] : i for i in self.linkInstances}



		#icon
		self.Icon = _Icon(r"%s\extra\icon.ico" %ipath)
		self.Size = _Size(290,435)
		self.Text = "Quasar"
		self.class1Result = "Done!"

		# fonts
		self.gsmt_11r = _Font("Georgia", 10)
		self.gsmt_10r = _Font("Georgia",9)

		# label
		self.label = Label()
		self.label.Text = "Select Links:"
		self.label.Font = self.gsmt_11r
		self.label.Location = _Point(30,10)
		self.label.Size = _Size(220,28)
		self.Controls.Add(self.label)

		# checklist box
		self.lstbox = CheckedListBox()
		self.lstbox.Items.AddRange(tuple(self.linkName))
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
		
		if len(self.lstbox.Items) < 1:
			self.Close()
		if len(self.lstbox.CheckedItems) < 1:
			return MessageBox.Show("No element selected!", "Quasar Info Box", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)	
		for i in self.lstbox.CheckedItems:
			i = self.linkDict[i];
			linkType  = doc.GetElement(i.GetTypeId());
			filepath = linkType.GetExternalFileReference().GetAbsolutePath();
			linkType.LoadFrom(filepath,None);
		
		self.class1Result = self.lstbox.CheckedItems;
		return self.Close();
			
							
	def click_all(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,True)

	def click_none(self, sender, event):
		for i in range(len(self.lstbox.Items)):
			self.lstbox.SetItemChecked(i,False)

