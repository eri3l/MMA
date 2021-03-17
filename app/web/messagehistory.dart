library messagehistory;
import 'dart:html';
import 'package:polymer/polymer.dart';

@CustomTag('message-history')
class MessageHistory extends PolymerElement {
  @published ObservableList data;

  MessageHistory.created() : super.created();

  @override ready() {
    this.data = new ObservableList();
    this.fire('message-history-ready');
    print('MessageHistory.ready');
  }
}

class MessageHistoryItem extends Observable {
  @observable final String name;
  @observable final String message;
  @observable final String timestamp;

  MessageHistoryItem({this.name, this.message, this.timestamp});
}
