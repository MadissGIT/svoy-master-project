"""Check the XAML requirements from homework 1 without external dependencies."""
from datetime import datetime
from pathlib import Path
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
X = '{http://schemas.microsoft.com/winfx/2009/xaml}'
tree = ET.parse(ROOT / 'MainPage.xaml')
nodes = list(tree.iter())
def tag(node):
    return node.tag.rsplit('}', 1)[-1]
def controls(kind):
    return [node for node in nodes if tag(node) == kind]
def require(condition, message):
    if not condition:
        raise AssertionError(message)

required = {
    'Label': (2, ['Text', 'FontSize', 'FontAttributes']),
    'Entry': (2, ['Placeholder', 'Keyboard', 'MaxLength']),
    'Editor': (1, ['Placeholder', 'AutoSize', 'MaxLength']),
    'Picker': (1, ['Title']),
    'RadioButton': (3, ['Content', 'GroupName', 'Value']),
    'CheckBox': (2, ['IsChecked', 'Color']),
    'Switch': (1, ['IsToggled', 'OnColor']),
    'DatePicker': (1, ['Format', 'Date', 'MinimumDate', 'MaximumDate']),
    'Stepper': (1, ['Minimum', 'Maximum', 'Value', 'Increment']),
    'Image': (1, ['Source', 'Aspect', 'HeightRequest']),
    'Button': (2, ['Text', 'Clicked']),
}
for kind, (minimum, attributes) in required.items():
    found = controls(kind)
    require(len(found) >= minimum, f'{kind}: too few controls')
    for node in found:
        require(all(prop in node.attrib for prop in attributes), f'{kind}: missing explicit properties')

visual = set(required) | {'ContentPage', 'ScrollView', 'Border', 'Grid', 'VerticalStackLayout', 'HorizontalStackLayout'}
names = []
for node in nodes:
    if tag(node) in visual:
        name = node.get(X + 'Name', '')
        require(re.fullmatch(r'_[A-Za-z][A-Za-z0-9_]*_', name), f'Missing or invalid x:Name: {tag(node)}')
        names.append(name)
require(len(set(names)) == len(names), 'Duplicate x:Name')
require(len({n.get('Keyboard') for n in controls('Entry')}) >= 2, 'Entry keyboards must differ')
radios = controls('RadioButton')
require(len({n.get('GroupName') for n in radios}) == 1, 'Radio buttons must share one group')
require(sum(n.get('IsChecked') == 'True' for n in radios) == 1, 'Choose one initial radio answer')
require(len(controls('Picker')[0].findall('.//' + X + 'String')) >= 3, 'Picker needs at least 3 choices')
require(len(controls('ScrollView')) >= 1, 'Missing ScrollView')
for node in controls('Grid'):
    require('RowDefinitions' in node.attrib and 'ColumnDefinitions' in node.attrib, 'Grid dimensions must be explicit')
    for child in node:
        require('Grid.Row' in child.attrib and 'Grid.Column' in child.attrib, 'Missing Grid position')
for node in controls('VerticalStackLayout') + controls('HorizontalStackLayout'):
    require('Spacing' in node.attrib, 'Missing explicit Spacing')
for node in controls('DatePicker'):
    low, value, high = [datetime.strptime(node.get(key), '%m/%d/%Y') for key in ['MinimumDate', 'Date', 'MaximumDate']]
    require(low <= value <= high, 'Initial date out of range')
for node in controls('Stepper'):
    require(float(node.get('Minimum')) <= float(node.get('Value')) <= float(node.get('Maximum')), 'Initial number out of range')
    require(float(node.get('Increment')) > 0, 'Invalid increment')
raw = (ROOT / 'MainPage.xaml').read_text(encoding='utf-8')
require('x:Reference' in raw and 'Path=Value' in raw and 'Mode=OneWay' in raw, 'Missing element binding')
require('Margin=' in raw and 'Padding=' in raw, 'Missing Margin or Padding')
code = (ROOT / 'MainPage.xaml.cs').read_text(encoding='utf-8')
require('_ReadyLabel_.IsVisible = HasRequiredAnswers()' in code, 'Missing conditional visibility')
events = {'Clicked', 'TextChanged', 'SelectedIndexChanged', 'CheckedChanged', 'Toggled', 'DateSelected', 'ValueChanged'}
for node in nodes:
    for event in events & node.attrib.keys():
        handler = node.get(event)
        require(re.search(r'void\s+' + re.escape(handler) + r'\s*\(', code), f'Missing code-behind handler: {handler}')
for path in ROOT.rglob('*'):
    if any(part in {'obj', 'bin', '.git'} for part in path.parts):
        continue
    if path.suffix in {'.xaml', '.xml', '.csproj', '.svg', '.plist', '.appxmanifest', '.manifest'}:
        ET.parse(path)
print(f'PASS: control counts, explicit properties, {len(names)} unique names, grids, bindings, events and ranges.')
