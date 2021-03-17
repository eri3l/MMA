library gelements;

import 'dart:html';
import 'dart:js';
import 'package:polymer/polymer.dart';
import 'package:paper_elements/paper_button.dart';
import 'package:paper_elements/paper_input.dart';
/* import 'custompaperdropdownmenu.dart'; */
import 'webdrv.aspx?proc=messagehistory.dart';

class GElement {
  GElement();

  Element create(JsObject p) => new Element.div();

  Element render(JsObject p) {
    print('GElement.render 0.1');
    Element e = this.create(p);
    print('GElement.render 0.2');
    var pp = p['parent'];
    print('GElement.render 0.3: ' + (pp == null ? 'pp == null' : 'pp != null'));
    var pe = null;

    if (pp == null || pp.callMethod('getDomElement', []) == null) {
      print('GElement.render 1.1');
      document.body.append(e);
      print('GElement.render 1.2');
    } else {
      print('GElement.render 2.1');
      pe = pp.callMethod('getDomElement', []);
      pe.append(e);
      print('GElement.render 2.2');
    }

    return e; 
  }

  void setContent(JsObject p, Element el) {
  }

  void getContent(JsObject p, Element el) {
  }
}

class GTemplate extends GElement {
  Element create(JsObject p) => new Element.div();
}

class GLabel extends GElement {
  Element create(JsObject p) => new Element.span();

  void setContent(JsObject p, Element el) {
    String value = p['value'];
    el.text = value == null ? '' : value;
  }

  void getContent(JsObject p, Element el) {
    p['value'] = el.text;
  }
}

class GTextbox extends GElement {
  Element create(JsObject p) {
    Element el = new Element.tag('paper-input');

    if (p['config']['multiline'] == 'true')
      el.setAttribute('multiline', '');

    return el;
	}

  void setContent(JsObject p, Element el) {
    if (el is PaperInput) {
      var value = p['value'];
      (el as PaperInput).inputValue = value == null ? '' : value;
    } else {
      print('GTextbox.setContent: control not ready yet');
		}
  }

  void getContent(JsObject p, Element el) {
    p['value'] = el.inputValue;
		print('GTextbox.getContent: ' + p['value']);
  }
}

class GButton extends GElement {
  Element create(JsObject p) {
    Element el = new Element.tag('paper-button');
    el.setAttribute('raised', '');
    return el;
  }

  void setContent(JsObject p, Element el) {
    var value = p['value'];
    el.innerHtml = value == null ? '' : value;
  }
}

class GGrid extends GElement {
  Element create(JsObject p) {
    String gridId = context['ngGridWrapper'].callMethod('getGridId', []);
    String ctrlId = context['ngGridWrapper'].callMethod('getCtrlId', []);
    print('Creating DOM elements for "' + ctrlId + '" controller and "' + gridId + '"');

    DivElement gridDiv = new DivElement();
    gridDiv.id = gridId;
    gridDiv.setAttribute('class', 'gridStyle');
    gridDiv.setAttribute('ng-grid', 'gridOptions');

    DivElement ctrlDiv = new DivElement();
    ctrlDiv.id = ctrlId;
    ctrlDiv.setAttribute('ng-controller', ctrlId);
    ctrlDiv.append(gridDiv);

    print('Created DOM elements for "' + ctrlId + '" controller and "' + gridId + '"');
    return ctrlDiv;
  }

  Element render(JsObject p) {
    print('GGrid.render 0.1');
    Map registerControllerOptions = new Map();
    registerControllerOptions['multiSelect'] = p['config']['multiple'] == 'true';
    var registerControllerOptionsJs = new JsObject.jsify(registerControllerOptions);
    context['ngGridWrapper'].callMethod('registerController', [registerControllerOptionsJs]);

    Element e = this.create(p);
    print('GGrid.render 0.2');
    var pp = p['parent'];
    print('GGrid.render 0.3: ' + (pp == null ? 'pp == null' : 'pp != null'));
    var pe = null;

    if (pp == null || pp.callMethod('getDomElement', []) == null) {
      print('GGrid.render 1.1');
      document.body.append(e);
      print('GGrid.render 1.2');
    } else {
      print('GGrid.render 2.1');
      pe = pp.callMethod('getDomElement', []);
      pe.append(e);
      print('GGrid.render 2.2');
    }

    String gridCtrlId = e.id;
    print('gridCtrlId: ' + gridCtrlId);
    context['ngGridWrapper'].callMethod('compileGrid', [gridCtrlId]);
    String gridId = e.firstChild.id;
    Array gridColumnDefs = this.convertDimsConfigs(p['config']['dim']);
    this.setGridColumns(gridId, gridColumnDefs);

    return e; 
  }

  void setGridColumns(String gridId, Array gridColumnDefs) {
    print('GGrid.setGridColumns');
    var jsGridColumnDefs = new JsObject.jsify(gridColumnDefs);
    context['ngGridWrapper'].callMethod('setGridColumns', [gridId, jsGridColumnDefs]);
  }

  Array convertDimsConfigs(Array dims) {
    Object boundDim = dims[0];
    Array gridColumnDefs = [];
    Array cols = boundDim['alias'];

    cols.forEach((col) {
      if (col['visible'] == 'true')
        gridColumnDefs.add({'field': col['name'], 'displayName': col['headerlabel'], 'enableCellEdit': true});
    });

    return gridColumnDefs;
  }
}

class GSelect2 extends GElement {
  Element create(JsObject p) {
    var el = new Element.tag('select');
    el.id = p['eId'];
    return el;
  }

  Element render(JsObject p) {
    print('GSelect2.render 0.1');
    Element e = this.create(p);
    print('GSelect2.render 0.2');
    var pp = p['parent'];
    print('GSelect2.render 0.3: ' + (pp == null ? 'pp == null' : 'pp != null'));
    var pe = null;

    if (pp == null || pp.callMethod('getDomElement', []) == null) {
      print('GSelect2.render 1.1');
      document.body.append(e);
      print('GSelect2.render 1.2');
    } else {
      print('GSelect2.render 2.1');
      pe = pp.callMethod('getDomElement', []);
      pe.append(e);
      print('GSelect2.render 2.2');
    }

    String eId = e.id;
    print('GSelect2 id: ' + eId);
    Map configData = new Map();
    var configDataJs = new JsObject.jsify(configData);

    context.callMethod(r'$', ["#" + eId]).callMethod('select2', [configDataJs]);
    print('GSelect2.render 3');
    return e; 
  }

  setContent(JsObject p, Element el) {
    print('GSelect2.setContent: ' + el.id);

    Array jsData = p['value'];

    if (jsData == null)
      return;

    (el as SelectElement).children.clear();

    jsData.forEach((row) {
      print('GSelect2.setContent: element value: ' + row['value'].toString() + ', label: ' + row['label'].toString());
      (el as SelectElement).children.add(new OptionElement(data: row['label'].toString(), value: row['value'].toString()));
    });
  }

  setLayout(Element el, JsObject layoutArr) {
    print('GSelect2.setLayout: ' + el.id);
    CssStyleDeclaration style = querySelector('#s2id_' + el.id).style;
    style.position = layoutArr['position'];
    var top = layoutArr['top'];
    var left = layoutArr['left'];
    var width = layoutArr['width'];

    if (top != null)
      style.top = layoutArr['top'];

    if (left != null)
      style.left = layoutArr['left'];

    if (width != null)
      style.width = layoutArr['width'];
  }

  getSelection(Element el) {
    print('GSelect2.getSelection: ' + context.callMethod(r'$', ["#" + el.id]).callMethod('select2', ['val']).toString());
    return context.callMethod(r'$', ["#" + el.id]).callMethod('select2', ['val']);
  }

  void setSelection(Element el, var value) {
    context.callMethod(r'$', ["#" + el.id]).callMethod('select2', ['val', value]);
    print('GSelect2.setSelection: ' + value);
  }
}

/*
class GCustomPaperDropdownMenu extends GElement {
  Element create(JsObject p) {
    var el = new Element.tag('custom-paper-dropdown-menu');
    el.id = p['eId'];
    return el;
  }

  setContent(JsObject p, Element el) {
    if (el is CustomPaperDropdownMenu) {
      print('GCustomPaperDropdownMenu.setContent: ' + el.id.toString());

      Array jsData = p['value'];

      if (jsData == null)
        return;

      ObservableList data = (el as CustomPaperDropdownMenu).data as ObservableList;
      print('setContent: data.length: ' + data.length.toString());
      data.clear();

      jsData.forEach((row) {
        print('setContent: element value: ' + row['value'].toString() + ', label: ' + row['label'].toString());
        data.add(new CustomPaperDropdownMenuItem(value: row['value'], label: row['label']));
      });
    } else {
      print('GCustomPaperDropdownMenu.setContent: control not ready yet');
		}
  }

  getSelection(Element el) {
    if (el is CustomPaperDropdownMenu)
        return (el as CustomPaperDropdownMenu).selected;
    else {
      print('getSelection: CustomPaperDropdownMenu control is not ready yet');
      return null;
    }
  }

  void setSelection(Element el, var value) {
    if (el is CustomPaperDropdownMenu)
       (el as CustomPaperDropdownMenu).selected = value;
    else {
      print('setSelection: CustomPaperDropdownMenu control is not ready yet');
		}
  }
}
*/

class GMessageHistory extends GElement {
  Element create(JsObject p) {
    Element el = new Element.tag('message-history');
    el.id = p['eId'];
    return el;
  }

  void setContent(JsObject p, Element el) {
    print('GMessageHistory.setContent: ' + el.id.toString());

    if (el is MessageHistory) {
      Array jsData = p['value'];

      if (jsData == null)
        return;

      ObservableList data = (el as MessageHistory).data as ObservableList;
      data.clear();

      jsData.forEach((row) {
        data.add(new MessageHistoryItem(name: row['name'], message: row['message'], timestamp: row['timestamp']));
      });
    } else {
      print('GMessageHistory.setContent: control not ready yet');
		}
  }
}

main() {
  print("dart 1");
  initPolymer();
  print("dart 2");

  GElement gElement = new GElement();
  GTemplate gTemplate = new GTemplate();
  GLabel gLabel = new GLabel();
  GTextbox gTextbox = new GTextbox();
  GButton gButton = new GButton();
  GGrid gGrid = new GGrid();
  GSelect2 gSelect2 = new GSelect2();
  /* GCustomPaperDropdownMenu gCustomPaperDropdownMenu = new GCustomPaperDropdownMenu(); */
  GMessageHistory gMessageHistory = new GMessageHistory();

  var elementclassPool = context['elementclassPool'];
  elementclassPool['GElementRender'] = gElement.render;
  elementclassPool['GTemplateRender'] = gTemplate.render;
  elementclassPool['GTemplateSetContent'] = gTemplate.setContent;
  elementclassPool['GLabelRender'] = gLabel.render;
  elementclassPool['GLabelSetContent'] = gLabel.setContent;
  elementclassPool['GLabelGetContent'] = gLabel.getContent;
  elementclassPool['GTextboxRender'] = gTextbox.render;
  elementclassPool['GTextboxSetContent'] = gTextbox.setContent;
  elementclassPool['GTextboxGetContent'] = gTextbox.getContent;
  elementclassPool['GButtonRender'] = gButton.render;
  elementclassPool['GButtonSetContent'] = gButton.setContent;
  elementclassPool['GGridRender'] = gGrid.render;
  elementclassPool['GMessageHistoryRender'] = gMessageHistory.render;
  elementclassPool['GMessageHistorySetContent'] = gMessageHistory.setContent;
  elementclassPool['GSelect2Render'] = gSelect2.render;
  elementclassPool['GSelect2SetContent'] = gSelect2.setContent;
  elementclassPool['GSelect2SetLayout'] = gSelect2.setLayout;
  elementclassPool['GSelect2GetSelection'] = gSelect2.getSelection;
  elementclassPool['GSelect2SetSelection'] = gSelect2.setSelection;
  /* elementclassPool['GCustomPaperDropdownMenuRender'] = gCustomPaperDropdownMenu.render;
  elementclassPool['GCustomPaperDropdownMenuSetContent'] = gCustomPaperDropdownMenu.setContent;
  elementclassPool['GCustomPaperDropdownMenuGetSelection'] = gCustomPaperDropdownMenu.getSelection;
  elementclassPool['GCustomPaperDropdownMenuSetSelection'] = gCustomPaperDropdownMenu.setSelection; */

  print("dart 3");
  context.callMethod('rootStart', []);
  print("dart 4");
}
